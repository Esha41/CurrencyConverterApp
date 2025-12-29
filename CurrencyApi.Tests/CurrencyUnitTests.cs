using CurrencyConverterApp.Models;
using CurrencyConverterApp.Services.Implementations;
using CurrencyConverterApp.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Moq;

namespace CurrencyApi.Tests
{
    public class CurrencyUnitTests
    {
        private readonly Mock<IExchangeRateProviderFactory> _mockFactory;
        private readonly Mock<IExchangeRateProvider> _mockProvider;
        private readonly IMemoryCache _cache;
        private readonly CurrencyService _service;

        public CurrencyUnitTests()
        {
            _mockProvider = new Mock<IExchangeRateProvider>();
            _mockFactory = new Mock<IExchangeRateProviderFactory>();
            _mockFactory.Setup(f => f.GetProvider("frankfurter"))
                        .Returns(_mockProvider.Object);
            _cache = new MemoryCache(new MemoryCacheOptions());
            _service = new CurrencyService(_mockFactory.Object, _cache);
        }

        [Fact]
        public async Task GetLatestExchangeRates_ReturnsRates()
        {
            var response = new ExchangeRateResponse
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal> { { "EUR", 10 } }
            };
            _mockProvider.Setup(p => p.GetLatestExchangeRates("USD")).ReturnsAsync(response);

            var result = await _service.GetLatestExchangeRates("frankfurter", "USD");

            Assert.NotNull(result);
            Assert.Equal("USD", result.Base);
            Assert.True(result.Rates.ContainsKey("EUR"));
        }

        [Fact]
        public async Task ConvertCurrency_ReturnsConvertedAmount()
        {
            var ratesResponse = new ExchangeRateResponse
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal> { { "EUR", 0.9M } }
            };
            _mockProvider.Setup(p => p.ConvertCurrency("USD", "EUR")).ReturnsAsync(ratesResponse);

            var result = await _service.ConvertCurrency("frankfurter", "USD", "EUR", 100);

            Assert.NotNull(result);
            Assert.Equal(90, result.ConvertedAmount);
        }

        [Fact]
        public async Task GetHistoricalExchangeRates_ReturnsPagedRates()
        {
            var history = new HistoricalRatesResponse
            {
                Base = "USD",
                Rates = new Dictionary<string, Dictionary<string, decimal>>
    {
        { DateTime.Today.AddDays(-1).ToString("yyyy-MM-dd"), new Dictionary<string, decimal> { { "EUR", 10 } } },
        { DateTime.Today.ToString("yyyy-MM-dd"), new Dictionary<string, decimal> { { "EUR", 0.92M } } }
    }
            };

            _mockProvider.Setup(p => p.GetHistoricalExchangeRates("USD", It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                         .ReturnsAsync(history);

            var result = await _service.GetHistoricalExchangeRates("frankfurter", "USD", DateTime.Today.AddDays(-1), DateTime.Today, 1, 2);

            Assert.NotNull(result);
            Assert.Equal(2, result.TotalRecords);
            Assert.True(result.Rates.Values.All(r => r.ContainsKey("EUR")));
        }
    }
}