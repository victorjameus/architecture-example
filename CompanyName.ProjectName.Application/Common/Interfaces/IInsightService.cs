using CompanyName.ProjectName.Domain.Enums;

namespace CompanyName.ProjectName.Application.Common.Interfaces;

public interface IInsightService
{
    void TrackEvent(string eventName, Dictionary<string, string>? properties = null);
    void TrackException(Exception exception, Dictionary<string, string>? properties = null);
    void TrackTrace(string message, InsightLevel level, Dictionary<string, string>? properties = null);
    void TrackDependency(string dependencyType, string target, string name, DateTimeOffset startTime, TimeSpan duration, bool success);
}