using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Domain.Enums;

namespace CompanyName.ProjectName.Infrastructure.Telemetry;

internal sealed class InsightService(TelemetryClient telemetryClient) : IInsightService
{
    public void TrackEvent(string eventName, Dictionary<string, string>? properties = null)
    {
        var telemetry = new EventTelemetry(eventName);

        if (properties != null)
        {
            foreach (var prop in properties)
            {
                telemetry.Properties[prop.Key] = prop.Value;
            }
        }

        telemetryClient.TrackEvent(telemetry);
    }

    public void TrackException(Exception exception, Dictionary<string, string>? properties = null)
    {
        var telemetry = new ExceptionTelemetry(exception);

        if (properties != null)
        {
            foreach (var prop in properties)
            {
                telemetry.Properties[prop.Key] = prop.Value;
            }
        }

        telemetryClient.TrackException(telemetry);
    }

    public void TrackTrace(string message, InsightLevel level, Dictionary<string, string>? properties = null)
    {
        var telemetry = new TraceTelemetry(message, MapLevel(level));

        if (properties != null)
        {
            foreach (var prop in properties)
            {
                telemetry.Properties[prop.Key] = prop.Value;
            }
        }

        telemetryClient.TrackTrace(telemetry);
    }

    public void TrackDependency(string dependencyType, string target, string name, DateTimeOffset startTime, TimeSpan duration, bool success)
    {
        var telemetry = new DependencyTelemetry(dependencyType, target, name, null)
        {
            Timestamp = startTime,
            Duration = duration,
            Success = success
        };

        telemetryClient.TrackDependency(telemetry);
    }

    private static SeverityLevel MapLevel(InsightLevel level) => level switch
    {
        InsightLevel.Verbose     => SeverityLevel.Verbose,
        InsightLevel.Information => SeverityLevel.Information,
        InsightLevel.Warning     => SeverityLevel.Warning,
        InsightLevel.Error       => SeverityLevel.Error,
        InsightLevel.Critical    => SeverityLevel.Critical,
        _                        => SeverityLevel.Information
    };
}