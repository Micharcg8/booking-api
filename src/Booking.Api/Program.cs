using Serilog;
using Serilog.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Serilog: structured logging from configuration
builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", context.HostingEnvironment.ApplicationName));

// Add services to the container.
builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

// Minimal test endpoint to validate host
app.MapGet("/api/ping", () => Results.Ok(new { status = "ok", service = "booking-api" }));

// Health endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
