using NorthwindTraders.Application.DependencyInjection;
using NorthwindTraders.Api.Middleware;
using NorthwindTraders.Infrastructure.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
const string frontendCorsPolicy = "FrontendCorsPolicy";

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy(frontendCorsPolicy, policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:9000",
                "http://127.0.0.1:9000",
                "http://192.168.56.100:9000")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Application owns use cases and validation; Infrastructure owns SQL Server, Google Maps, and report generators.
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

app.UseHttpsRedirection();

app.UseCors(frontendCorsPolicy);

// Keep controllers thin by translating known exceptions into consistent HTTP problem responses here.
app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
