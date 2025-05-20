namespace AnalyticService.Contracts.Services;

public interface IRequestLogConsumer
{
    Task ConsumeBatchAsync(int maxMessages, CancellationToken cancellationToken = default);
}