using Confluent.SchemaRegistry;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Order.Infrastructure.DataAccess;

namespace Order.Infrastructure;

public static class DependencyInjectionExtensions
{
    public static void AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        AddKafkaComponents(services, configuration);
    }

    public static void AddOrdersDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("OrdersDb");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Connection string was not provided.");

        services.AddDbContext<OrdersDbContext>(options => options.UseNpgsql(connectionString));
    }

    private static void AddKafkaComponents(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<ISchemaRegistryClient>(provider =>
        {
            var config = new SchemaRegistryConfig
            {
                Url = configuration["SchemaRegistry:Url"]
            };

            return new CachedSchemaRegistryClient(config);
        });
    }

    public static void ApplyMigrationsOrdersDbContext(this IHost app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<OrdersDbContext>();

        if (dbContext.Database.IsRelational())
        {
            dbContext.Database.Migrate();
        }
    }
}