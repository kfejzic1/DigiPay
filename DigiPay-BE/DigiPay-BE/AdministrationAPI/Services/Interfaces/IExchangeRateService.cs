using AdministrationAPI.Contracts.Requests.ExchangeRates;
using AdministrationAPI.Models;

namespace AdministrationAPI.Services.Interfaces
{
    public interface IExchangeRateService
    {
        public Task<bool> AddCurrency(CurrencyRequest currencyRequest);
        public Task<bool> AddExchangeRate(ExchangeRateRequest exchangeRateRequest);
        public Task<ICollection<ExchangeRate>> GetExchangeRates();
        public Task<ICollection<Currency>> GetCurrencies();
        public Task DeleteAllExchangeRates();
    }
}