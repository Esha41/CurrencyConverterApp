namespace CurrencyConverterApp.Services.Interfaces
{
    public interface IExchangeRateProviderFactory
    {
        IExchangeRateProvider GetProvider(string providerName);
    }
}
