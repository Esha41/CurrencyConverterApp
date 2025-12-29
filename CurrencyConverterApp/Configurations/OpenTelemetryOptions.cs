namespace CurrencyConverterApp.Configurations
{
    public class OpenTelemetryOptions
    {
        public const string SectionName = "OpenTelemetry";
        public string ServiceName { get; set; } = "CurrencyConverterApi";
        public bool EnableConsoleExporter { get; set; } = true;
        public bool EnableJaegerExporter { get; set; } = false;
        public string JaegerHost { get; set; } = "localhost";
        public int JaegerPort { get; set; } = 6831;
    }
}
