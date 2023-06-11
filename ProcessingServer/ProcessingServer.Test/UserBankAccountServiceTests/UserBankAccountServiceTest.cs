using AutoFixture;
using AutoMapper;
using Moq;
using ProcessingServer.BLL.DTO;
using ProcessingServer.BLL.Services;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.QualityTools.Testing.Fakes;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.DAL.Other;
using System.Security.Principal;

namespace ProcessingServer.Tests.AccountServiceTests
{
    public class UserBankAccountServiceTest
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IAdministrationService> _administrationMock = new Mock<IAdministrationService>();


        [Fact]
        public void MapToAccount_MappedToAccount()
        {
            var accountRequest = _fixture.Create<UserBankAccountRequest>();
            var account = _fixture.Create<UserBankAccount>();
            var numberOfAccounts = new List<UserBankAccount>()
            {
                new UserBankAccount(),
                new UserBankAccount(),
                new UserBankAccount()
            };

            _mapperMock.Setup(_ => _.Map<UserBankAccount>(accountRequest)).Returns(account);

            var sut = GetTestSubject();

            var result = sut.MapToAccount(accountRequest, numberOfAccounts);

            Assert.Equal(0, result.Credit);
            Assert.Equal(0, result.Debit);
            Assert.Equal(0, result.Total);

            _mapperMock.Verify(_ => _.Map<UserBankAccount>(It.IsAny<UserBankAccountRequest>()), Times.Once);

        }

        [Fact]
        public async Task ValidateCurrency_ReturnsTrue()
        {
            var listOfCurrencies = _fixture.Build<Currency>().With(_ => _.Name, "BAM").CreateMany(5).ToList();
            _administrationMock.Setup(_ => _.GetAvailableCurrenciesFromAdministrationApi(It.IsAny<string>()))
                .ReturnsAsync(listOfCurrencies);

            var sut = GetTestSubject();

            var result = await sut.ValidateCurrency("","BAM");

            Assert.True(result);
        }

        [Fact]
        public async Task ValidateCurrency_ThrowsException()
        {
            var listOfCurrencies = _fixture.Build<Currency>().With(_ => _.Name, "BAM").CreateMany(5).ToList();
            _administrationMock.Setup(_ => _.GetAvailableCurrenciesFromAdministrationApi(It.IsAny<string>()))
                .ReturnsAsync(listOfCurrencies);

            var sut = GetTestSubject();

            var exception = Assert.ThrowsAsync<Exception>(async () => await sut.ValidateCurrency("", "EUR"));

            Assert.Equal("Unavailable currency!", exception.Result.Message);
        }

        [Fact]
        public async Task GetAllAccountsForUser_ReturnsAccounts()
        {
            var owner = _fixture.Create<User>();
            var accounts = _fixture.CreateMany<UserBankAccount>().ToList();

            _unitOfWorkMock.Setup(_ => _.UserBankAccountRepository.GetAllAccountsForUser(owner.UserId)).ReturnsAsync(accounts);


            _administrationMock.Setup(_ => _.GetUserFromAdministrationApi(It.IsAny<string>(), _mapperMock.Object))
               .ReturnsAsync(owner);

            var sut = GetTestSubject();

            var result = sut.GetAllAccountsForUser("esddlj");

            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetAllAccountsForUser_0Accounts()
        {
            var owner = _fixture.Create<User>();
            var accounts = _fixture.CreateMany<UserBankAccount>(0).ToList();

            _unitOfWorkMock.Setup(_ => _.UserBankAccountRepository.GetAllAccountsForUser(owner.UserId)).ReturnsAsync(accounts);


            _administrationMock.Setup(_ => _.GetUserFromAdministrationApi(It.IsAny<string>(), _mapperMock.Object))
               .ReturnsAsync(owner);

            var sut = GetTestSubject();

            var exception = Assert.ThrowsAsync<Exception>(async () => await sut.GetAllAccountsForUser("disj"));
            Assert.Equal("User does not have any account!", exception.Result.Message);
        }



        private UserBankAccountService GetTestSubject()
        {
            return new UserBankAccountService(_unitOfWorkMock.Object, _mapperMock.Object, _administrationMock.Object);
        }

    }
}
