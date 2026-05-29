using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Domain.Enums;

namespace CompanyName.ProjectName.Infrastructure.Resilience;

public static class ResiliencePolicies
{
    public static AsyncRetryPolicy GetRetryPolicy(IInsightService insightService, string serviceName)
    {
        return Policy
            .Handle<Exception>()
            .WaitAndRetryAsync
            (
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                onRetry: (exception, timeSpan, attempt, context) =>
                {
                    insightService.TrackTrace
                    (
                        $"{serviceName} - Reintento {attempt} después de {timeSpan.TotalSeconds}s. Error: {exception.Message}",
                        InsightLevel.Warning
                    );
                }
            );
    }

    public static AsyncCircuitBreakerPolicy GetCircuitBreakerPolicy(IInsightService insightService, string serviceName)
    {
        return Policy
            .Handle<Exception>()
            .CircuitBreakerAsync
            (
                exceptionsAllowedBeforeBreaking: 3,
                durationOfBreak: TimeSpan.FromSeconds(30),
                onBreak: (exception, duration) =>
                {
                    insightService.TrackTrace
                    (
                        $"{serviceName} - Circuit breaker abierto por {duration.TotalSeconds}s. Error: {exception.Message}",
                        InsightLevel.Warning
                    );
                },
                onReset: () =>
                {
                    insightService.TrackTrace
                    (
                        $"{serviceName} - Circuit breaker cerrado. Servicio recuperado.",
                        InsightLevel.Information
                    );
                },
                onHalfOpen: () =>
                {
                    insightService.TrackTrace
                    (
                        $"{serviceName} - Circuit breaker en prueba.",
                        InsightLevel.Information
                    );
                }
            );
    }
}