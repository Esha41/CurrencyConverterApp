using CurrencyConverterApp.Services.Implementations;
using CurrencyConverterApp.Services.Interfaces;

namespace CurrencyConverterApp.Services.Implementation
{
    public class ExchangeRateProviderFactory: IExchangeRateProviderFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ExchangeRateProviderFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IExchangeRateProvider GetProvider(string providerName)
        {
            return providerName.ToLower() switch
            {
                "frankfurter" => _serviceProvider.GetRequiredService<FrankfurterExchangeRateProvider>(),
                "otherprovider" => _serviceProvider.GetRequiredService<dynamic>(),
                _ => throw new ArgumentException($"No provider found for '{providerName}'")
            };
        }
    }
}
