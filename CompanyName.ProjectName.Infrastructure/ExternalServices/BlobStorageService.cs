using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Infrastructure.ExternalServices;

internal sealed class BlobStorageService(IConfiguration configuration) : IBlobStorageService
{
    private readonly string _connectionString = configuration["AzureBlobStorage:ConnectionString"]!;
    private readonly string _containerName = configuration["AzureBlobStorage:ContainerName"]!;

    public async Task<string> UploadAsync(Stream content, string fileName, string contentType)
    {
        try
        {
            var container = new BlobContainerClient(_connectionString, _containerName);
            await container.CreateIfNotExistsAsync(PublicAccessType.Blob);

            var blob = container.GetBlobClient(fileName);
            await blob.UploadAsync(content, new BlobHttpHeaders { ContentType = contentType });

            return blob.Uri.ToString();
        }
        catch (Exception ex)
        {
            throw new ExternalServiceException($"Error al subir el archivo {fileName} a Blob Storage: {ex.Message}");
        }
    }

    public async Task<bool> DeleteAsync(string fileName)
    {
        try
        {
            var container = new BlobContainerClient(_connectionString, _containerName);
            var blob = container.GetBlobClient(fileName);

            return await blob.DeleteIfExistsAsync();
        }
        catch (Exception ex)
        {
            throw new ExternalServiceException($"Error al eliminar el archivo {fileName} de Blob Storage: {ex.Message}");
        }
    }
}