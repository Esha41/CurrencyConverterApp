namespace CurrencyConverterApp.Models
{
    public class HistoricalRatesResponse
    {
        public string Base { get; set; }
        public Dictionary<string, Dictionary<string, decimal>> Rates { get; set; }
        public int TotalRecords { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
