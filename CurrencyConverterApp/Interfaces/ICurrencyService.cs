using CurrencyConverterApp.Models;

namespace CurrencyConverterApp.Interfaces
{
    public interface ICurrencyService
    {
        Task<ExchangeRateResponse> GetLatestExchangeRates(string providerName, string baseCurrency);
        Task<dynamic> ConvertCurrency(string providerName, string from, string to, decimal amount);
        Task<HistoricalRatesResponse> GetHistoricalExchangeRates(string providerName, string baseCurrency, DateTime startDate, DateTime endDate, int pageNum, int pageSize);
    }
}
