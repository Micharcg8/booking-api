using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Serilog: structured logging from configuration
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName));

// Add services to the container.
builder.Services.AddSingleton<Booking.Api.Services.InMemoryHoldStore>();
builder.Services.AddSingleton<Booking.Api.Services.InMemoryBookingStore>();
builder.Services.AddSingleton<Booking.Api.Services.InMemoryPaymentStore>();
builder.Services.AddSingleton<Booking.Api.Services.IdempotencyStore>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter(
                System.Text.Json.JsonNamingPolicy.CamelCase,
                allowIntegerValues: true));
    });

// CORS for frontend (e.g. Angular on localhost:4200)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// JWT authentication
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "booking-api";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "booking-fe";
if (!string.IsNullOrEmpty(jwtKey) && jwtKey.Length >= 32)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });
    builder.Services.AddAuthorization();
}

// Swagger / OpenAPI (Bearer auth supported; add token via Authorization header)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseMiddleware<Booking.Api.Middleware.CorrelationIdMiddleware>();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Minimal test endpoint to validate host
app.MapGet("/api/ping", () => Results.Ok(new { status = "ok", service = "booking-api" }));

// Health endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();

// Expose for WebApplicationFactory in integration tests
public partial class Program { }
