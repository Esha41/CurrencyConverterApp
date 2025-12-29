using CurrencyConverterApp.Models;
using CurrencyConverterApp.Services.Interfaces;
using System.Text.Json;

namespace CurrencyConverterApp.Services.Implementations
{
    public class FrankfurterExchangeRateProvider : IExchangeRateProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<FrankfurterExchangeRateProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public FrankfurterExchangeRateProvider(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor, ILogger<FrankfurterExchangeRateProvider> logger)
        {
            _httpClientFactory = httpClientFactory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ExchangeRateResponse> GetLatestExchangeRates(string baseCurrency)
        {
            var client = _httpClientFactory.CreateClient("FrankFurterApi");
            var url = $"latest?base={baseCurrency}";

            //log the corelationID and outgoing request to frankfurter API
            var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();
            client.DefaultRequestHeaders.Remove("X-Correlation-Id");
            client.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);
            _logger.LogInformation("Calling Frankfurter API | CorrelationId: {CorrelationId} | URL: {Url}", correlationId, url);

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ExchangeRateResponse>(content, JsonOptions)!;

            //log the corelationID and incoming response to frankfurter API
            _logger.LogInformation(
            "Frankfurter API response | CorrelationId: {CorrelationId} | StatusCode: {StatusCode}",
            correlationId,
            response.StatusCode);

            return data;
        }

        public async Task<HistoricalRatesResponse> GetHistoricalExchangeRates(string baseCurrency, DateTime startDate, DateTime endDate)
        {
            var url = $"{startDate:yyyy-MM-dd}..{endDate:yyyy-MM-dd}?base={baseCurrency}";
            var client = _httpClientFactory.CreateClient("FrankFurterApi");

            //log the corelationID and outgoing request to frankfurter API
            var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();
            client.DefaultRequestHeaders.Remove("X-Correlation-Id");
            client.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);
            _logger.LogInformation("Calling Frankfurter API | CorrelationId: {CorrelationId} | URL: {Url}", 
             correlationId, 
             url);

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<HistoricalRatesResponse>(content, JsonOptions)!;

            //log the corelationID and incoming response to frankfurter API
            _logger.LogInformation(
            "Frankfurter API response | CorrelationId: {CorrelationId} | StatusCode: {StatusCode}",
            correlationId,
            response.StatusCode);

            return data;
        }

        public async Task<ExchangeRateResponse> ConvertCurrency(string from, string to)
        {
            var client = _httpClientFactory.CreateClient("FrankFurterApi");
            var url = $"latest?base={from}&symbols={to}";

            //log the corelationID and outgoing request to frankfurter API
            var correlationId = _httpContextAccessor.HttpContext?.Items["CorrelationId"]?.ToString();
            client.DefaultRequestHeaders.Remove("X-Correlation-Id");
            client.DefaultRequestHeaders.Add("X-Correlation-Id", correlationId);
            _logger.LogInformation("Calling Frankfurter API | CorrelationId: {CorrelationId} | URL: {Url}",
             correlationId,
             url);

            var response = await client.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<ExchangeRateResponse>(content, JsonOptions)!;

            //log the corelationID and incoming response to frankfurter API
            _logger.LogInformation(
            "Frankfurter API response | CorrelationId: {CorrelationId} | StatusCode: {StatusCode}",
            correlationId,
            response.StatusCode);

            return data;
        }
    }
}
