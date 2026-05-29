namespace CompanyName.ProjectName.Api.Extensions.Swagger;

public static class SwaggerExtension
{
    public static void AddSwaggerExtension(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SwaggerSettings>(configuration.GetSection(nameof(SwaggerSettings)));
        var provider = services.BuildServiceProvider();
        var settings = GetConfiguration(provider);

        services.AddApiVersioning(options =>
        {
            var version = settings.VersionSettings.First().Version;
            options.ApiVersionReader = new HeaderApiVersionReader([.. settings.HeadersVersions]);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.DefaultApiVersion = new ApiVersion
            (
                int.Parse(version.Split('.')[0]),
                int.Parse(version.Split('.')[1])
            );
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        services.AddSwaggerGen(options =>
        {
            var versionProvider = services.BuildServiceProvider()
                .GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in versionProvider.ApiVersionDescriptions)
            {
                options.SwaggerDoc
                (
                    description.GroupName,
                    settings.GetOpenApiInformation(description.ApiVersion.ToString())
                );
            }

            options.DescribeAllParametersInCamelCase();
            options.EnableAnnotations();
            options.DocumentFilter<AddVersionHeader>();

            var fileName = $"{typeof(SwaggerExtension).Assembly.GetName().Name}.xml";
            var filePath = Path.Combine(AppContext.BaseDirectory, fileName);

            if (File.Exists(filePath))
            {
                options.IncludeXmlComments(filePath);
            }
        });
    }

    public static void UseSwaggerExtension(this IApplicationBuilder app)
    {
        var provider = app.ApplicationServices;
        var settings = GetConfiguration(provider);
        var versionProvider = provider.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UsePathBase($"/{settings.ServerPrefix}");
        app.UseStaticFiles();

        app.UseSwagger(options =>
        {
            options.PreSerializeFilters.Add((swagger, request) =>
            {
                swagger.Servers = [GetOpenApiServer(settings, request)];
            });
        });

        app.UseSwaggerUI(options =>
        {
            foreach (var description in versionProvider.ApiVersionDescriptions)
            {
                var setting = settings.VersionSettings.FirstOrDefault
                (
                    v => v.Version == description.ApiVersion.ToString()
                );

                options.SwaggerEndpoint
                (
                    setting?.EndpointUrl ?? $"../swagger/{description.GroupName}/swagger.json",
                    setting?.EndpointDescription ?? $"BaseApi {description.ApiVersion}"
                );
            }

            options.DisplayOperationId();
        });
    }

    private static OpenApiServer GetOpenApiServer(SwaggerSettings settings, HttpRequest request)
    {
        string prefix = settings is { ServerPrefix.Length: > 0 } ? $"/{settings.ServerPrefix}" : string.Empty;
        string url = $"{settings!.Scheme ?? request.Scheme}://{request.Host.Value}{prefix}";

        return new()
        {
            Description = "BaseApi",
            Url = url
        };
    }

    private static SwaggerSettings GetConfiguration(IServiceProvider provider)
    {
        return provider.GetRequiredService<IOptions<SwaggerSettings>>().Value;
    }
}