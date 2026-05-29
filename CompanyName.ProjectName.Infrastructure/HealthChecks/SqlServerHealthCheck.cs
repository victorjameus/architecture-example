namespace CompanyName.ProjectName.Infrastructure.HealthChecks;

public sealed class SqlServerHealthCheck(IConfiguration configuration) : IHealthCheck
{
    private readonly string _connectionString = configuration.GetConnectionString("DefaultConnection")!;

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken ct = default)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(ct);

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            await command.ExecuteScalarAsync(ct);

            return HealthCheckResult.Healthy("SQL Server disponible.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("SQL Server no disponible.", ex);
        }
    }
}