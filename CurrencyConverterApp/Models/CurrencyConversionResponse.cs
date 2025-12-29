namespace CurrencyConverterApp.Models
{
    public class CurrencyConversionResponse
    {
        public string From { get; set; }
        public string To { get; set; }
        public decimal Amount { get; set; }
        public decimal Rate { get; set; }
        public decimal ConvertedAmount { get; set; }
    }
}
