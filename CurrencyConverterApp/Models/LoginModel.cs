using System.Text.Json.Serialization;

namespace CurrencyConverterApp.Models
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public string? Role { get; set; }
    }
}
