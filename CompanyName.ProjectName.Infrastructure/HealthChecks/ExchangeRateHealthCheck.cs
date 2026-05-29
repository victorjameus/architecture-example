namespace CompanyName.ProjectName.Infrastructure.HealthChecks;

public sealed class ExchangeRateHealthCheck(IConfiguration configuration) : IHealthCheck
{
    private readonly string _baseUrl = configuration["ExchangeRateApi:BaseUrl"]!;
    private readonly string _apiKey = configuration["ExchangeRateApi:ApiKey"]!;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var response = await $"{_baseUrl}/{_apiKey}/codes"
                .WithTimeout(5)
                .GetAsync(cancellationToken: ct);

            return response.StatusCode == 200
                ? HealthCheckResult.Healthy("ExchangeRate-API disponible.")
                : HealthCheckResult.Degraded("ExchangeRate-API respondió con error.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("ExchangeRate-API no disponible.", ex);
        }
    }
}