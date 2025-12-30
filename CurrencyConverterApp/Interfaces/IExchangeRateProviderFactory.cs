namespace CurrencyConverterApp.Interfaces
{
    public interface IExchangeRateProviderFactory
    {
        IExchangeRateProvider GetProvider(string providerName);
    }
}
