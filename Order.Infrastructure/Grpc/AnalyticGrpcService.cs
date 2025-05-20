using AnalyticService;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Order.Domain.Models;
using Order.Domain.Repositories;
using TelemetryDrivenOrderProcessingSystem.Common.Domain.Models;
using TelemetryDrivenOrderProcessingSystem.Common.Domain.Repositories;

namespace Order.Infrastructure.Grpc;

public class AnalyticGrpcService(IRequestLogRepository requestLogRepository, ILogger<AnalyticGrpcService> logger) : Analytic.AnalyticBase
{
    public override Task<AddDataResultResponse> AddData(AddDataHttpRequestLog requestLog, ServerCallContext context)
    {
        List<HttpRequestLog> records = [ new() ];

        logger.LogInformation(nameof(AnalyticGrpcService) + "Get message from gRPC\nContent: [{0},\n]", String.Join("\n, ", requestLog.Headers));

        // analyticRequestRepository.AddRequestAsync(records);
        return Task.FromResult(new AddDataResultResponse
        {
             Result = nameof(AnalyticGrpcService) + string.Format("Get message from gRPC\nContent: [{0},\n]", String.Join("\n, ", requestLog.Headers))
        });
    }
}