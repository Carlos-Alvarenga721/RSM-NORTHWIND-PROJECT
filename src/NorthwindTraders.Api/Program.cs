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
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
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

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
