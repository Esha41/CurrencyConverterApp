using CurrencyConverterApp.Models;

namespace CurrencyConverterApp.Services.Interfaces
{
    public interface IExchangeRateProvider
    {
        Task<ExchangeRateResponse> GetLatestExchangeRates(string baseCurrency);
        Task<ExchangeRateResponse> ConvertCurrency(string from, string to);
        Task<HistoricalRatesResponse> GetHistoricalExchangeRates(string baseCurrency, DateTime startDate, DateTime endDate);
    }
}
