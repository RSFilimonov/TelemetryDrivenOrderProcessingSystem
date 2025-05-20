using TelemetryDrivenOrderProcessingSystem.Common.Domain.Models;

namespace Order.Application.Contracts.Producer;

public interface IRequestLogProducer
{
    Task SendAsync(HttpRequestLog log, CancellationToken cancellationToken = default);
}