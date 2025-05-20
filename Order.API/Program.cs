using Confluent.SchemaRegistry;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Order.Application.Contracts.Producer;
using Order.Infrastructure;
using Order.Infrastructure.Kafka.Producer;
using TelemetryDrivenOrderProcessingSystem;
using TelemetryDrivenOrderProcessingSystem.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавляем OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(b =>
    {
        b.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("MyServiceName"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    });

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddOrdersDbContext(builder.Configuration);

builder.Services.AddSingleton<ISchemaRegistryClient>(sp =>
{
    var schemaRegistryConfig = new SchemaRegistryConfig
    {
        Url = "http://localhost:8081"
    };

    return new CachedSchemaRegistryClient(schemaRegistryConfig);
});

// Kafka producer
builder.Services.AddSingleton<IRequestLogProducer, KafkaRequestLogProducer>();

builder.Services.AddDistributedMemoryCache(); // Используется для хранения сессионных данных в памяти
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20); // Время бездействия до истечения сессии
    options.Cookie.HttpOnly = true; // Ограничивает доступ к cookie только через HTTP
    options.Cookie.IsEssential = true; // Указывает, что cookie является необходимым
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.ApplyMigrationsOrdersDbContext();

// Добавление middleware
app.UseRouting();
app.UseSession();
// app.UseAuthorization();

app.UseMiddleware<HttpRequestLogMiddleware>();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.Run();

namespace TelemetryDrivenOrderProcessingSystem
{
    record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}