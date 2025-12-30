using CurrencyConverterApp.Enums;
using CurrencyConverterApp.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterApp.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyService _currencyService;
        private readonly ILogger<CurrencyConverterController> _logger;
        public CurrencyConverterController(ICurrencyService currencyService, 
            ILogger<CurrencyConverterController> logger)
        {
            _currencyService = currencyService;
            _logger = logger;
        }

        //Fetch the latest exchange rates for a specific base currency (e.g., EUR).
        [HttpGet("latestExchangeRates")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLatestExchangeRates([FromQuery] string baseCurrency, ExchangeRateProviders provider = ExchangeRateProviders.Frankfurter)
        {
            _logger.LogInformation("Fetching latest exchange rates. BaseCurrency: {BaseCurrency}, Provider: {Provider}", 
                baseCurrency, provider);

            try
            {
                var exchangeRates = await _currencyService.GetLatestExchangeRates(provider.ToString(), 
                                                                                  baseCurrency.ToUpper());

                _logger.LogInformation("Successfully fetched exchange rates for BaseCurrency: {BaseCurrency}", 
                    baseCurrency);

                return Ok(exchangeRates);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Error occurred while fetching exchange rates. BaseCurrency: {BaseCurrency}, Provider: {Provider}",
                    baseCurrency, provider);
                return StatusCode(500, new { error = e.Message });
            }
        }

        //Convert amounts between different currencies. 
        [HttpGet("currencyConversion")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount, ExchangeRateProviders provider = ExchangeRateProviders.Frankfurter)
        {
            _logger.LogInformation("Currency conversion requested. From: {From}, To: {To}, Amount: {Amount}, Provider: {Provider}",
                from, to, amount, provider);

            if (amount <= 0)
                return BadRequest(new { error = "Amount must be greater than zero." });

            try
            {
                var result = await _currencyService.ConvertCurrency(provider.ToString(), 
                                                                    from.ToUpper(), to.ToUpper(), amount);

                _logger.LogInformation("Currency conversion successful. From: {From}, To: {To}, Amount: {Amount}", 
                    from, to, amount);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e,"Error occurred during currency conversion. From: {From}, To: {To}, Amount: {Amount}, Provider: {Provider}",
                    from, to, amount, provider);
                return StatusCode(500, new { error = e.Message });
            }
        }

        //Retrieve historical exchange rates for a given period with pagination
        [HttpGet("historicalExchangeRates")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHistoricalExchangeRates([FromQuery] string baseCurrency, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, int page = 1, int pageSize = 5, ExchangeRateProviders provider = ExchangeRateProviders.Frankfurter)
        {
            _logger.LogInformation("Fetching historical exchange rates. BaseCurrency: {BaseCurrency}, StartDate: {StartDate}, EndDate: {EndDate}, Page: {Page}, PageSize: {PageSize}, Provider: {Provider}",
                   baseCurrency, startDate, endDate, page, pageSize, provider);

            if (startDate > endDate)
                return BadRequest(new { error = "Start date must be before end date." });

            if (page <= 0)
                return BadRequest(new { error = "Page number must be greater than zero." });

            try
            {
                var result = await _currencyService.GetHistoricalExchangeRates(provider.ToString(), 
                                                    baseCurrency.ToUpper(), startDate, endDate, page, pageSize);

                _logger.LogInformation("Successfully fetched historical exchange rates for BaseCurrency: {BaseCurrency}",
                    baseCurrency);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

