var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Swagger / OpenAPI (will be fully wired when SDK / packages are aligned)
builder.Services.AddEndpointsApiExplorer();

// Health checks
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

// Minimal test endpoint to validate host
app.MapGet("/api/ping", () => Results.Ok(new { status = "ok", service = "booking-api" }));

// Health endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();
