using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Infrastructure.ExternalServices;
using CompanyName.ProjectName.Infrastructure.Persistence;
using CompanyName.ProjectName.Infrastructure.Telemetry;

namespace CompanyName.ProjectName.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDbConnection>(_ => new SqlConnection(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IExchangeRateService, ExchangeRateService>();
        services.AddScoped<IBlobStorageService, BlobStorageService>();
        services.AddSingleton<IInsightService, InsightService>();

        return services;
    }
}