namespace CompanyName.ProjectName.Application.Common.Interfaces;

public interface IBlobStorageService
{
    Task<string> UploadAsync(Stream content, string fileName, string contentType);
    Task<bool> DeleteAsync(string fileName);
}