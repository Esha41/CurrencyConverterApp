using CurrencyConverterApp.Enums;
using CurrencyConverterApp.Services.Interfaces;
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

        public CurrencyConverterController(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        //Fetch the latest exchange rates for a specific base currency (e.g., EUR).
        [HttpGet("latestExchangeRates")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetLatestExchangeRates([FromQuery] string baseCurrency, ExchangeRateProviders provider = ExchangeRateProviders.Frankfurter)
        {
            try
            {
                var exchangeRates = await _currencyService.GetLatestExchangeRates(provider.ToString(), baseCurrency.ToUpper());
                return Ok(exchangeRates);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpGet("currencyConversion")]
        [Authorize(Roles = "Admin,User")]
        public async Task<IActionResult> ConvertCurrency([FromQuery] string from, [FromQuery] string to, [FromQuery] decimal amount, ExchangeRateProviders provider = ExchangeRateProviders.Frankfurter)
        {
            if (amount <= 0)
                return BadRequest(new { error = "Amount must be greater than zero." });

            try
            {
                var result = await _currencyService.ConvertCurrency(provider.ToString(), from.ToUpper(), to.ToUpper(), amount);
                return Ok(result);
            }
            catch (Exception e)
            {
                return StatusCode(500, new { error = e.Message });
            }
        }

        [HttpGet("historicalExchangeRates")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetHistoricalExchangeRates([FromQuery] string baseCurrency, [FromQuery] DateTime startDate, [FromQuery] DateTime endDate, int page = 1, int pageSize = 5, ExchangeRateProviders provider = ExchangeRateProviders.Frankfurter)
        {
            if (startDate > endDate)
                return BadRequest(new { error = "Start date must be before end date." });

            if (page <= 0)
                return BadRequest(new { error = "Page number must be greater than zero." });

            try
            {
                var result = await _currencyService.GetHistoricalExchangeRates(provider.ToString(), baseCurrency.ToUpper(), startDate, endDate, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message });
            }
        }
    }
}

