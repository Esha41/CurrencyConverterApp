using CurrencyConverterApp.Models;
using CurrencyConverterApp.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverterApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TokenService _token;
        private readonly IConfiguration _config;

        public AuthController(TokenService token, IConfiguration config)
        {
            _token = token;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login([FromBody] LoginModel login)
        {
            //generate JWT access token based on the login credentials
            var token = _token.GenerateJwtAccessToken(login.Username, login.Password);
            if (token == null)
                return Unauthorized("Invalid username or password");

            return Ok(token);
        }
    }
}
