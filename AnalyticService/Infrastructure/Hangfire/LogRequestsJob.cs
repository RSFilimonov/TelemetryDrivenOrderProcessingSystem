using AnalyticService.Contracts.Services;
using Hangfire;

namespace AnalyticService.Infrastructure.Hangfire;

public class LogRequestsJob(IRequestLogConsumer consumer)
{
    [JobDisplayName("Consume Kafka Logs")]
    public async Task Execute(CancellationToken cancellationToken)
    {
        await consumer.ConsumeBatchAsync(100, cancellationToken); // читаем 100 событий
    }
}