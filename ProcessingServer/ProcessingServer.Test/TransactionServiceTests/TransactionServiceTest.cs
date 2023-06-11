using AutoFixture;
using AutoMapper;
using Moq;
using ProcessingServer.BLL.DTO;
using ProcessingServer.BLL.Interfaces;
using ProcessingServer.BLL.Services;
using ProcessingServer.DAL.Entities;
using ProcessingServer.DAL.Interfaces;
using ProcessingServer.DAL.Other;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessingServer.Tests.TransactionServiceTests
{
    public class TransactionServiceTest
    {
        private readonly IFixture _fixture = new Fixture();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new Mock<IUnitOfWork>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();
        private readonly Mock<IAdministrationService> _administrationMock = new Mock<IAdministrationService>();


        [Fact]
        public void ValidateAmountFilterRange_ReturnsTrue()
        {
            /*
             * when we want to create object with randomly populated values we use _fixture.Create<Type>() and then all 
             * properties are random. If we want specific value for one or many properties we use syntax below
             *
             */
            

            var filter = _fixture.Build<TransactionsFilter>()
                    .With(_ => _.AmountStartFilter, "10")
                    .With(_ => _.AmountEndFilter, "100")
                    .Create();

            var sut = GetTestSubject(); //sut stands for system under test

            var result = sut.ValidateAmountFilterRange(filter);

            Assert.True(result);
        }

        [Fact]
        public void GetTransactionsForUser_ReturnsUserTransactions()
        {
            var sender = _fixture.Create<User>();
            var transactions = _fixture.CreateMany<Transaction>().ToList();
            var filter = _fixture.Build<TransactionsFilter>()
                    .With(_ => _.AmountStartFilter, "")
                    .With(_ => _.AmountEndFilter, "")
                    .Create();

            _administrationMock.Setup(_ => _.GetUserFromAdministrationApi(It.IsAny<string>(),_mapperMock.Object))
                .ReturnsAsync(sender);

            _unitOfWorkMock.Setup(_ => _.TransactionRepository.GetTransactionsForUser(sender)).ReturnsAsync(transactions);

            var sut = GetTestSubject();

            var result = sut.GetTransactionsForUser("x", "1", "2", filter, "default");

            _administrationMock.Verify(_ => _.GetUserFromAdministrationApi(It.IsAny<string>(), _mapperMock.Object), Times.Once);
            _unitOfWorkMock.Verify(_ => _.TransactionRepository.GetTransactionsForUser(sender), Times.Once);
        }

        [Fact]
        public async Task GroupTransactionsByCurrency_Grouped()
        {
            var transactions = _fixture.CreateMany<TransactionsGroup>().ToList();
            var sender = _fixture.Create<User>();

            _administrationMock.Setup(_ => _.GetUserFromAdministrationApi(It.IsAny<string>(), _mapperMock.Object))
               .ReturnsAsync(sender);

            _unitOfWorkMock.Setup(_ => _.TransactionRepository.GroupTransactionsByCurrency(sender)).ReturnsAsync(transactions);


            var sut = GetTestSubject();

            var result = sut.GroupTransactionsByCurrency("sdijd");

            Assert.NotNull(result);

        }

        [Fact]
        public async Task GroupTransactionsByType_Grouped()
        {
            var transactions = _fixture.CreateMany<TransactionsGroup>().ToList();
            var sender = _fixture.Create<User>();

            _administrationMock.Setup(_ => _.GetUserFromAdministrationApi(It.IsAny<string>(), _mapperMock.Object))
               .ReturnsAsync(sender);

            _unitOfWorkMock.Setup(_ => _.TransactionRepository.GroupTransactionsByType(sender)).ReturnsAsync(transactions);


            var sut = GetTestSubject();

            var result = sut.GroupTransactionsByType("sdijd");

            Assert.NotNull(result);
        }


        private TransactionService GetTestSubject()
        {
            return new TransactionService(_unitOfWorkMock.Object, _mapperMock.Object, _administrationMock.Object);
        }
    }
}
