namespace CompanyName.ProjectName.Api.Extensions.Swagger;

public class SwaggerSettings
{
    public string Title { get; set; } = string.Empty;
    public string ContactName { get; set; } = string.Empty;
    public string ContactUrl { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string ServerPrefix { get; set; } = string.Empty;
    public string? Scheme { get; set; } = string.Empty;
    public List<string> HeadersVersions { get; set; } = [];
    public List<VersionSettings> VersionSettings { get; set; } = [];

    public OpenApiInfo GetOpenApiInformation(string version)
    {
        return new()
        {
            Contact = GetContact(),
            Description = Description,
            Title = Title,
            Version = version
        };
    }

    private OpenApiContact GetContact()
    {
        return new()
        {
            Email = Email ?? "name@example.com",
            Name = ContactName ?? "Contact Name",
            Url = new Uri(ContactUrl ?? "https://example.com")
        };
    }
}

public class VersionSettings
{
    public string Version { get; set; } = string.Empty;
    public string EndpointUrl { get; set; } = string.Empty;
    public string EndpointDescription { get; set; } = string.Empty;
}