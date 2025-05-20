using AnalyticService.Contracts.Services;
using AnalyticService.Infrastructure.DataAccess.Repositories;
using AnalyticService.Infrastructure.Hangfire;
using AnalyticService.Infrastructure.Kafka.Consumer;
using Hangfire;
using Hangfire.PostgreSql;
using Order.Infrastructure;
using Order.Infrastructure.Grpc;
using TelemetryDrivenOrderProcessingSystem.Common.Domain.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddGrpc();

// Add repositories
builder.Services.AddTransient<IRequestLogRepository, RequestLogRepository>();

// Add services
builder.Services.AddSingleton<IRequestLogConsumer, KafkaRequestLogBatchConsumer>();

builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(options =>
    {
        options.UseNpgsqlConnection(builder.Configuration.GetConnectionString("HangfireConnection"));
        // Можно также указать другие настройки, если нужно:
        // options.SchemaName = "hangfire";
        // options.PrepareSchemaIfNecessary = true;
    }));
builder.Services.AddHangfireServer();

builder.Services.AddTransient<LogRequestsJob>();

var app = builder.Build();

// Add Hangfire middleware
app.UseHangfireDashboard();
app.UseHangfireJobs();

// Configure the HTTP request pipeline.
app.MapGrpcService<AnalyticGrpcService>();
app.MapGet("/",
    () =>
        "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();

