using CurrencyConverterApp.Authentication;
using CurrencyConverterApp.Middlewares;
using CurrencyConverterApp.Services.Implementation;
using CurrencyConverterApp.Services.Implementations;
using CurrencyConverterApp.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

//configure serilog , structured logging and logs will be printed in file
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
     .WriteTo.File("logs/log.txt",            
                  rollingInterval: RollingInterval.Day,
                  outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

//Add http client for frank furter API, add policy and circuit breaker
builder.Services.AddHttpClient("FrankFurterApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["FrankFurterBaseURL"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
})
.AddPolicyHandler(GetRetryPolicy())
.AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpContextAccessor();

// Add services to the container.
builder.Services.AddScoped<FrankfurterExchangeRateProvider>();
builder.Services.AddScoped<ICurrencyService, CurrencyService>();
builder.Services.AddScoped<IExchangeRateProviderFactory, ExchangeRateProviderFactory>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Add API versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT Bearer token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});

//Memory cache service
builder.Services.AddMemoryCache();

//JWT authentication configuration
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});
builder.Services.AddAuthorization();
//Configure rate limiting, 10 requests per minute for each client
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("currency-rate-limit", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.User.Identity?.Name
                          ?? context.Connection.RemoteIpAddress?.ToString()
                          ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,         
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }));
});

var app = builder.Build();

if (app.Environment.IsDevelopment()||app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/error");
}

app.UseRateLimiter();
app.UseHttpsRedirection();
app.UseMiddleware<LoggingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers().RequireRateLimiting("currency-rate-limit");
app.Run();

// Retry policy with exponential backoff
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError() 
        .WaitAndRetryAsync(
            3, // retry 3 times
            retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)), 
            onRetry: (outcome, timespan, retryAttempt, context) =>
            {
                Console.WriteLine($"Retry {retryAttempt} after {timespan.TotalSeconds}s due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            }
        );
}

// Circuit breaker policy
static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3,   // break after 3 failures
            durationOfBreak: TimeSpan.FromSeconds(30), // stop requests for 30 seconds
            onBreak: (outcome, breakDelay) =>
            {
                Console.WriteLine($"Circuit broken for {breakDelay.TotalSeconds}s due to {outcome.Exception?.Message ?? outcome.Result.StatusCode.ToString()}");
            },
            onReset: () =>
            {
                Console.WriteLine("Circuit reset, API calls resumed.");
            }
        );
}

public partial class Program { }