namespace CurrencyConverterApp.Configurations
{
    public class RateLimitOptions
    {
        public const string SectionName = "RateLimiting";
        public int PermitLimit { get; set; } 
        public int WindowInSeconds { get; set; } 
        public int QueueLimit { get; set; } 
    }
}
