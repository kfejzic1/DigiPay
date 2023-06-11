using AdministrationAPI.Contracts.Requests;
using AdministrationAPI.Contracts.Requests.ExchangeRates;
using AdministrationAPI.Contracts.Requests.Vendors;
using AdministrationAPI.Contracts.Responses;
using AdministrationAPI.Data;
using AdministrationAPI.Models;
using AdministrationAPI.Models.Vendor;
using AdministrationAPI.Models.Voucher;
using AdministrationAPI.Services;
using AdministrationAPI.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace TestProject
{
    public class ActivationCodeTest
    {
        private User user1, user2;
        private List<EmailActivationCode> emailCodes = new List<EmailActivationCode>();
        private List<SMSActivationCode> smsCodes = new List<SMSActivationCode>();

        private readonly ITestOutputHelper _output;
        private Mock<AppDbContext> _context = new Mock<AppDbContext>();
        private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
        private readonly Mock<IActivationCodeService> _activationCodeService = new Mock<IActivationCodeService>();
        private readonly Mock<IUserService> _userService = new Mock<IUserService>();
        private Mock<UserManager<User>> _userManager;

        public ActivationCodeTest(ITestOutputHelper output)
        {
            _output = output;

            user1 = new User() { FirstName = "Elvir", LastName = "Vlahovljak", UserName = "evlahovlja1", NormalizedUserName = "EVLAHOVLJA1", ConcurrencyStamp = "1", Email = "evlahovlja1@etf.unsa.ba", NormalizedEmail = "EVLAHOVLJA1@ETF.UNSA.BA", EmailConfirmed = false, PasswordHash = "AQAAAAIAAYagAAAAEL+9sxZQaY0F4wxS0N24IGTB+z6oIeFEX8wQgqdzsskd4XC/oE+2YWgxc/LwTsx+dw==", PhoneNumber = "061904086", PhoneNumberConfirmed = false, Address = "Tamo negdje 1", TwoFactorEnabled = false, LockoutEnabled = false };
            user2 = new User() { FirstName = "Almina", LastName = "Brulic", UserName = "abrulic1", NormalizedUserName = "ABRULIC1", ConcurrencyStamp = "1", Email = "abrulic1@etf.unsa.ba", NormalizedEmail = "ABRULIC1@ETF.UNSA.BA", EmailConfirmed = false, PasswordHash = "AQAAAAIAAYagAAAAENao66CqvIXroh/6aTaoJ/uThFfjLemBtjLfuiJpP/NoWXkhJO/G8wspnWhjLJx9WQ==", PhoneNumber = "061111111", PhoneNumberConfirmed = false, Address = "Tamo negdje 1", TwoFactorEnabled = true, LockoutEnabled = false };
            emailCodes = new List<EmailActivationCode>
            {
                new EmailActivationCode { Id = Guid.NewGuid().ToString(), Code = "1111", User = user1 }
            };
            smsCodes = new List<SMSActivationCode>
            {
                new SMSActivationCode { Id = Guid.NewGuid().ToString(), Code = "1111", User = user1 }
            };

            _context.Setup(x => x.EmailActivationCodes).ReturnsDbSet(emailCodes);
            _context.Setup(x => x.SMSActivationCodes).ReturnsDbSet(smsCodes);

            _userManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
            );

            //_context.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
        }

        [Fact]
        public async Task ActivationCodeNull()
        {
            var service = new ActivationCodeService( _context.Object, _userManager.Object, _userService.Object, _configuration.Object );

            var result = await service.ActivateEmailCodeAsync("", "");
            Assert.False(result);

            result = await service.ActivateSMSCodeAsync("", "");
            Assert.False(result);
        }

        [Fact]
        public async Task EmailActivationSuccessful()
        {
            var service = new ActivationCodeService(_context.Object, _userManager.Object, _userService.Object, _configuration.Object);

            var result = await service.ActivateEmailCodeAsync("1111", "evlahovlja1");
            Assert.True(result);
            Assert.True(user1.EmailConfirmed);

            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task SMSActivationSuccessful()
        {
            var service = new ActivationCodeService(_context.Object, _userManager.Object, _userService.Object, _configuration.Object);

            var result = await service.ActivateSMSCodeAsync("1111", "evlahovlja1");
            Assert.True(result);
            Assert.True(user1.PhoneNumberConfirmed);

            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GenerateEmailCodeWithoutPreExisting()
        {
            var service = new ActivationCodeService(_context.Object, _userManager.Object, _userService.Object, _configuration.Object);

            var result = await service.GenerateEmailActivationCodeForUserAsync(user2);
            Assert.True(result);
            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GenerateSMSCodeWithoutPreExisting()
        {
            var service = new ActivationCodeService(_context.Object, _userManager.Object, _userService.Object, _configuration.Object);

            var result = await service.GenerateSMSActivationCodeForUserAsync(user2);
            Assert.False(result);
            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task GenerateEmailCodeWithPreExisting()
        {
            var service = new ActivationCodeService(_context.Object, _userManager.Object, _userService.Object, _configuration.Object);

            var result = await service.GenerateEmailActivationCodeForUserAsync(user1);
            Assert.True(result);
            _context.Verify(x => x.SaveChangesAsync(default), Times.Exactly(2));
        }

        [Fact]
        public async Task GenerateSMSCodeWithPreExisting()
        {
            var service = new ActivationCodeService(_context.Object, _userManager.Object, _userService.Object, _configuration.Object);

            var result = await service.GenerateSMSActivationCodeForUserAsync(user1);
            Assert.False(result);
            _context.Verify(x => x.SaveChangesAsync(default), Times.Exactly(2));
        }

        //[Fact]
        //public async Task DontAllowNullParameter()
        //{
        //    var service = new ExchangeRateService(_context.Object);
        //    CurrencyRequest curr = null;
        //    var created = await service.AddCurrency(curr);

        //    Assert.False(created);

        //    ExchangeRateRequest req = null;
        //    created = await service.AddExchangeRate(req);

        //    Assert.False(created);

        //    _context.Verify(x => x.SaveChanges(), Times.Never);
        //}

        //[Fact]
        //public async Task DontAllowDuplicate()
        //{
        //    var service = new ExchangeRateService(_context.Object);
        //    CurrencyRequest curr = new CurrencyRequest
        //    {
        //        Country = "BIH",
        //        Name = "BAM"
        //    };
        //    var created = await service.AddCurrency(curr);

        //    Assert.False(created);

        //    ExchangeRateRequest req = new ExchangeRateRequest
        //    {
        //        InputCurreny = "BIH (BAM)",
        //        OutputCurrency = "USA (USD)",
        //        StartDate = DateTime.Now
        //    };
        //    created = await service.AddExchangeRate(req);

        //    Assert.False(created);

        //    _context.Verify(x => x.SaveChanges(), Times.Never);
        //}

        //[Fact]
        //public async Task SuccessfulAddCurrency()
        //{

        //    IExchangeRateService service = new ExchangeRateService(_context.Object);
        //    CurrencyRequest curr = new CurrencyRequest
        //    {
        //        Country = "SWI",
        //        Name = "CHF"
        //    };
        //    var created = await service.AddCurrency(curr);

        //    Assert.True(created);
        //    _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        //}

        //[Fact]
        //public async Task SuccessfulAddExchangeRate()
        //{
        //    var service = new ExchangeRateService(_context.Object);

        //    ExchangeRateRequest req = new ExchangeRateRequest
        //    {
        //        InputCurreny = "BIH (BAM)",
        //        OutputCurrency = "CHF (SWI)",
        //        StartDate = DateTime.Now
        //    };
        //    var created = await service.AddExchangeRate(req);

        //    Assert.True(created);
        //    _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        //}

        //[Fact]
        //public async Task SuccessfulGetAllCurrencies()
        //{
        //    var service = new ExchangeRateService(_context.Object);

        //    var currencies = await service.GetCurrencies();

        //    Assert.NotNull(currencies);
        //    Assert.Equal(3, currencies.Count);
        //}

        //[Fact]
        //public async Task SuccessfulGetAllExchangeRates()
        //{
        //    var service = new ExchangeRateService(_context.Object);

        //    var exchangeRates = await service.GetExchangeRates();

        //    Assert.NotNull(exchangeRates);
        //    Assert.Equal(2, exchangeRates.Count);
        //}
    }
}