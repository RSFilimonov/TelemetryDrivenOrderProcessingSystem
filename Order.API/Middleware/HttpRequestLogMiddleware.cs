using AnalyticService.Application;
using Order.Application.Contracts.Producer;

namespace TelemetryDrivenOrderProcessingSystem.Middleware;

public class HttpRequestLogMiddleware(IRequestLogProducer producer, RequestDelegate next)
{
    public async Task Invoke(HttpContext context)
    {
        var log = HttpRequestLogFactory.Create(context);

        // отправка в Kafka (асинхронно, но не блокирует основной поток)
        await producer.SendAsync(log);

        await next(context);
    }
}