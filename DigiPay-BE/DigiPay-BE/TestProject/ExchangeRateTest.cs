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
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace TestProject
{
    public class ExchangeRateTest
    {
        private User user;
        private List<Currency> currencies = new List<Currency>();
        private List<ExchangeRate> exchangeRates = new List<ExchangeRate>();

        private readonly ITestOutputHelper _output;
        private Mock<AppDbContext> _context = new Mock<AppDbContext>();
        private readonly Mock<IConfiguration> _configuration = new Mock<IConfiguration>();
        private readonly Mock<IExchangeRateService> _exchangeRateService = new Mock<IExchangeRateService>();

        public ExchangeRateTest(ITestOutputHelper output)
        {
            _output = output;
            currencies = new List<Currency>()
            {
                new Currency() { Id = Guid.NewGuid().ToString(), Name = "BAM", Country = "BIH", ExchangeRatesAsInput = new List<ExchangeRate>(), ExchangeRatesAsOutput = new List<ExchangeRate>() },
                new Currency() { Id = Guid.NewGuid().ToString(), Name = "USD", Country = "USA", ExchangeRatesAsInput = new List<ExchangeRate>(), ExchangeRatesAsOutput = new List<ExchangeRate>() },
                new Currency() { Id = Guid.NewGuid().ToString(), Name = "EUR", Country = "DEU", ExchangeRatesAsInput = new List<ExchangeRate>(), ExchangeRatesAsOutput = new List<ExchangeRate>() },
            };

            exchangeRates = new List<ExchangeRate>()
            {
                new ExchangeRate
                {
                    Id = Guid.NewGuid().ToString(),
                    InputCurrency = currencies[0],
                    OutputCurrency = currencies[1],
                    Rate = 0.51,
                    StartDate = DateTime.Now
                },
                new ExchangeRate
                {
                    Id = Guid.NewGuid().ToString(),
                    InputCurrency = currencies[1],
                    OutputCurrency = currencies[0],
                    Rate = 1/0.51,
                    StartDate = DateTime.Now
                }
            };

            _context.Setup(x => x.Currencies).ReturnsDbSet(currencies);
            _context.Setup(x => x.ExchangeRates).ReturnsDbSet(exchangeRates);

            //_context.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);
        }

        [Fact]
        public async Task DontAllowNullParameter()
        {
            var service = new ExchangeRateService(_context.Object);
            CurrencyRequest curr = null;
            var created = await service.AddCurrency(curr);

            Assert.False(created);

            ExchangeRateRequest req = null;
            created = await service.AddExchangeRate(req);

            Assert.False(created);

            _context.Verify(x => x.SaveChanges(), Times.Never);
        }

        [Fact]
        public async Task DontAllowDuplicate()
        {
            var service = new ExchangeRateService(_context.Object);
            CurrencyRequest curr = new CurrencyRequest
            {
                Country = "BIH",
                Name = "BAM"
            };
            var created = await service.AddCurrency(curr);

            Assert.False(created);

            ExchangeRateRequest req = new ExchangeRateRequest
            {
                InputCurreny = "BIH (BAM)",
                OutputCurrency = "USA (USD)",
                StartDate = DateTime.Now
            };
            created = await service.AddExchangeRate(req);

            Assert.False(created);

            _context.Verify(x => x.SaveChanges(), Times.Never);
        }

        [Fact]
        public async Task SuccessfulAddCurrency()
        {

            IExchangeRateService service = new ExchangeRateService(_context.Object);
            CurrencyRequest curr = new CurrencyRequest
            {
                Country = "SWI",
                Name = "CHF"
            };
            var created = await service.AddCurrency(curr);

            Assert.True(created);
            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task SuccessfulAddExchangeRate()
        {
            var service = new ExchangeRateService(_context.Object);

            ExchangeRateRequest req = new ExchangeRateRequest
            {
                InputCurreny = "BIH (BAM)",
                OutputCurrency = "CHF (SWI)",
                StartDate = DateTime.Now
            };
            var created = await service.AddExchangeRate(req);

            Assert.True(created);
            _context.Verify(x => x.SaveChangesAsync(default), Times.Once);
        }

        [Fact]
        public async Task SuccessfulGetAllCurrencies()
        {
            var service = new ExchangeRateService(_context.Object);

            var currencies = await service.GetCurrencies();

            Assert.NotNull(currencies);
            Assert.Equal(3, currencies.Count);
        }

        [Fact]
        public async Task SuccessfulGetAllExchangeRates()
        {
            var service = new ExchangeRateService(_context.Object);

            var exchangeRates = await service.GetExchangeRates();

            Assert.NotNull(exchangeRates);
            Assert.Equal(2, exchangeRates.Count);
        }
    }
}