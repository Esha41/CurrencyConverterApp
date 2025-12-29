using CurrencyConverterApp.Models;
using CurrencyConverterApp.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CurrencyConverterApp.Services.Implementations
{
    public class CurrencyService : ICurrencyService
    {
        private readonly IExchangeRateProviderFactory _exchangeRateProviderfactory;
        private static readonly HashSet<string> BlockedCurrencies = new() { "TRY", "PLN", "THB", "MXN" };
        private readonly IMemoryCache _cache;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(30);

        public CurrencyService(IExchangeRateProviderFactory exchangeRateProviderfactory, IMemoryCache cache)
        {
            _exchangeRateProviderfactory = exchangeRateProviderfactory;
            _cache = cache;
        }

        public async Task<ExchangeRateResponse> GetLatestExchangeRates(string providerName, string baseCurrency)
        {
            //generate cache key and get latest exchange rates from cache if exists
            string cacheKey = $"LatestRates_{baseCurrency}_{providerName}";
            if (_cache.TryGetValue(cacheKey, out ExchangeRateResponse? cachedLatestRates) && cachedLatestRates != null)
                return cachedLatestRates;

            try
            {
                //get provider name to select the class using factory pattern
                var provider = _exchangeRateProviderfactory.GetProvider(providerName);
                var response = await provider.GetLatestExchangeRates(baseCurrency);

                if (response == null || response.Rates == null)
                    throw new Exception("No exchange rates found.");

                // Cache the result
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                };
                _cache.Set(cacheKey, response, cacheOptions);

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<dynamic> ConvertCurrency(string providerName, string from, string to, decimal amount)
        {
            try
            {
                //Exclude TRY, PLN, THB, and MXN from the response 
                if (BlockedCurrencies.Contains(from) || BlockedCurrencies.Contains(to))
                    throw new InvalidOperationException("Currency not supported.");

                //generate cache key and get converted currency rates from cache if exists
                string cacheKey = $"ConvertCurrency_{from}_{to}_{amount}_{providerName}";
                if (_cache.TryGetValue(cacheKey, out CurrencyConversionResponse? cachedConversion) && cachedConversion != null)
                    return cachedConversion;

                //get provider name to select the class using factory pattern
                var provider = _exchangeRateProviderfactory.GetProvider(providerName);
                var ratesResponse = await provider.ConvertCurrency(from, to);
                var rate = ratesResponse.Rates[to];
                var convertedAmount = Math.Round(amount * rate, 2);

                var response = new CurrencyConversionResponse
                {
                    From = from,
                    To = to,
                    Amount = amount,
                    Rate = rate,
                    ConvertedAmount = convertedAmount
                };

                //cache the result
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                };
                _cache.Set(cacheKey, response, cacheOptions);

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<HistoricalRatesResponse> GetHistoricalExchangeRates(string providerName, string baseCurrency, DateTime startDate, DateTime endDate, int pageNum, int pageSize)
        {
            //generate cache key and get historical exchange rates from cache if exists
            string cacheKey = $"HistoricalRates_{baseCurrency}_{startDate:yyyyMMdd}_{endDate:yyyyMMdd}_Page{pageNum}_Size{pageSize}_{providerName}";
            if (_cache.TryGetValue(cacheKey, out HistoricalRatesResponse? cachedHistory) && cachedHistory != null)
                return cachedHistory;

            try
            {
                //get provider name to select the class using factory pattern
                var provider = _exchangeRateProviderfactory.GetProvider(providerName);
                var history = await provider.GetHistoricalExchangeRates(baseCurrency, startDate, endDate);
                if (history?.Rates == null || !history.Rates.Any())
                    throw new Exception("No historical rates found.");

                var allRates = history.Rates.OrderBy(r => r.Key).ToList();
                var pagedRates = allRates
                    .Skip((pageNum - 1) * pageSize)
                    .Take(pageSize)
                    .ToDictionary(r => r.Key, r => r.Value);

                var response = new HistoricalRatesResponse
                {
                    Base = history.Base,
                    Rates = pagedRates,
                    TotalRecords = allRates.Count,
                    Page = pageNum,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling(allRates.Count / (double)pageSize)
                };

                //cache the result
                var cacheOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = _cacheDuration
                };
                _cache.Set(cacheKey, response, cacheOptions);

                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
