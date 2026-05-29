using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Domain.Enums;
using CompanyName.ProjectName.Domain.Exceptions;
using CompanyName.ProjectName.Infrastructure.Resilience;

namespace CompanyName.ProjectName.Infrastructure.ExternalServices;

internal sealed class BlobStorageService(IConfiguration configuration, IInsightService insightService) : IBlobStorageService
{
    private readonly string _connectionString = configuration["AzureBlobStorage:ConnectionString"]!;
    private readonly string _containerName = configuration["AzureBlobStorage:ContainerName"]!;
    private readonly AsyncPolicy _policy = Policy.WrapAsync
    (
        ResiliencePolicies.GetRetryPolicy(insightService, "BlobStorageService"),
        ResiliencePolicies.GetCircuitBreakerPolicy(insightService, "BlobStorageService")
    );

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType)
    {
        try
        {
            return await _policy.ExecuteAsync(async () =>
            {
                var container = new BlobContainerClient(_connectionString, _containerName);
                await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

                var blob = container.GetBlobClient(fileName);
                await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });

                insightService.TrackTrace
                (
                    $"Archivo {fileName} subido a Blob Storage exitosamente.",
                    InsightLevel.Information
                );

                return blob.Uri.ToString();
            });
        }
        catch (Exception ex)
        {
            insightService.TrackTrace
            (
                $"Error al subir archivo {fileName} a Blob Storage.",
                InsightLevel.Error
            );

            throw new ExternalServiceException
            (
                $"Error al subir el archivo {fileName} a Blob Storage: {ex.Message}"
            );
        }
    }

    public async Task<bool> DeleteAsync(string fileName)
    {
        try
        {
            return await _policy.ExecuteAsync(async () =>
            {
                var container = new BlobContainerClient(_connectionString, _containerName);
                var blob = container.GetBlobClient(fileName);
                var result = await blob.DeleteIfExistsAsync();

                insightService.TrackTrace
                (
                    $"Archivo {fileName} eliminado de Blob Storage.",
                    InsightLevel.Information
                );

                return result.Value;
            });
        }
        catch (Exception ex)
        {
            insightService.TrackTrace
            (
                $"Error al eliminar archivo {fileName} de Blob Storage.",
                InsightLevel.Error
            );

            throw new ExternalServiceException
            (
                $"Error al eliminar el archivo {fileName} de Blob Storage: {ex.Message}"
            );
        }
    }
}