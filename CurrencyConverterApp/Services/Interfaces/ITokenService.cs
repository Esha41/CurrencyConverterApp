namespace CurrencyConverterApp.Services.Interfaces
{
    public interface ITokenService
    {
        public string GenerateJwtAccessToken(string username, string password);
    }
}
