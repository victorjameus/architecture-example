namespace CompanyName.ProjectName.Infrastructure.HealthChecks;

public sealed class BlobStorageHealthCheck(IConfiguration configuration) : IHealthCheck
{
    private readonly string _connectionString = configuration["AzureBlobStorage:ConnectionString"]!;
    private readonly string _containerName = configuration["AzureBlobStorage:ContainerName"]!;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            var container = new BlobContainerClient(_connectionString, _containerName);
            await container.ExistsAsync(ct);

            return HealthCheckResult.Healthy("Blob Storage disponible.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Blob Storage no disponible.", ex);
        }
    }
}