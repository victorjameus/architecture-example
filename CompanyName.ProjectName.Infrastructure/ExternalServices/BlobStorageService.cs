using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Domain.Enums;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Infrastructure.ExternalServices;

internal sealed class BlobStorageService(IConfiguration configuration, IInsightService insightService) : IBlobStorageService
{
    private readonly string _connectionString = configuration["AzureBlobStorage:ConnectionString"]!;
    private readonly string _containerName = configuration["AzureBlobStorage:ContainerName"]!;

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType)
    {
        var startTime = DateTimeOffset.UtcNow;
        var success = false;

        try
        {
            var container = new BlobContainerClient(_connectionString, _containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blob = container.GetBlobClient(fileName);
            await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });

            success = true;
            insightService.TrackTrace($"Archivo {fileName} subido a Blob Storage exitosamente.", InsightLevel.Information);

            return blob.Uri.ToString();
        }
        catch (Exception ex)
        {
            insightService.TrackTrace($"Error al subir archivo {fileName} a Blob Storage.", InsightLevel.Error);
            throw new ExternalServiceException($"Error al subir el archivo {fileName} a Blob Storage: {ex.Message}");
        }
        finally
        {
            var duration = DateTimeOffset.UtcNow - startTime;
            insightService.TrackDependency("AzureBlob", _containerName, $"Upload {fileName}", startTime, duration, success);
        }
    }

    public async Task<bool> DeleteAsync(string fileName)
    {
        var startTime = DateTimeOffset.UtcNow;
        var success = false;

        try
        {
            var container = new BlobContainerClient(_connectionString, _containerName);
            var blob = container.GetBlobClient(fileName);
            var result = await blob.DeleteIfExistsAsync();

            success = true;
            insightService.TrackTrace($"Archivo {fileName} eliminado de Blob Storage.", InsightLevel.Information);

            return result;
        }
        catch (Exception ex)
        {
            insightService.TrackTrace($"Error al eliminar archivo {fileName} de Blob Storage.", InsightLevel.Error);
            throw new ExternalServiceException($"Error al eliminar el archivo {fileName} de Blob Storage: {ex.Message}");
        }
        finally
        {
            var duration = DateTimeOffset.UtcNow - startTime;
            insightService.TrackDependency("AzureBlob", _containerName, $"Delete {fileName}", startTime, duration, success);
        }
    }
}