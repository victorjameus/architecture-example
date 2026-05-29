namespace CompanyName.ProjectName.Api.Extensions.Swagger;

public class AddVersionHeader : IDocumentFilter
{
    public void Apply(OpenApiDocument document, DocumentFilterContext context)
    {
        string version = document.Info.Version;
        List<OperationType> types = [.. Enum.GetValues<OperationType>()];

        foreach (var value in document.Paths.Values)
        {
            types.ForEach(type => UpdateParameters(type, value, version));
        }
    }

    private static void UpdateParameters(OperationType type, OpenApiPathItem value, string version)
    {
        if (value.Operations.TryGetValue(type, out var operation))
        {
            operation.Parameters ??= [];
            operation.Parameters.Add(GetParameter(version));
        }
    }

    private static OpenApiParameter GetParameter(string version)
    {
        return new()
        {
            Name = "Api-Version",
            In = ParameterLocation.Header,
            Description = "Versión de la API",
            Required = false,
            Schema = GetOpenApiSchema(version)
        };
    }

    private static OpenApiSchema GetOpenApiSchema(string version)
    {
        return new()
        {
            Type = "string",
            Default = new Microsoft.OpenApi.Any.OpenApiString(version)
        };
    }
}