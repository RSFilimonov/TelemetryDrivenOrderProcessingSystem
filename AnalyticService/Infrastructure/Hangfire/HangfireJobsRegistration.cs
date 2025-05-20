using Hangfire;

namespace AnalyticService.Infrastructure.Hangfire;

public static class HangfireJobsRegistration
{
    public static void UseHangfireJobs(this IApplicationBuilder app)
    {
        RegisterAnalyticJobs();
    }

    private static void RegisterAnalyticJobs()
    {
        RecurringJob.AddOrUpdate<LogRequestsJob>(
            "minutely-read-requests", // уникальный идентификатор
                task => task.Execute(CancellationToken.None),
            Cron.Minutely // расписание
        );
    }
}