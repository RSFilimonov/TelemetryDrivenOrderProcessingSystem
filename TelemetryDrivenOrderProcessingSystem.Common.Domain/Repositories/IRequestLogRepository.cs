using TelemetryDrivenOrderProcessingSystem.Common.Domain.Models;

namespace TelemetryDrivenOrderProcessingSystem.Common.Domain.Repositories;

public interface IRequestLogRepository
{
    Task SaveRequestsAsync(List<HttpRequestLog> request, CancellationToken cancellationToken = default);
}