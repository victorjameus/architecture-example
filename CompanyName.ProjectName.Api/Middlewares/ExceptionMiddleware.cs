using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Api.Middlewares;

public class ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            NotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
            ValidationException ex => (HttpStatusCode.BadRequest, ex.Message),
            ConflictException ex => (HttpStatusCode.Conflict, ex.Message),
            ExternalServiceException ex => (HttpStatusCode.BadGateway, ex.Message),
            DomainException ex => (HttpStatusCode.BadRequest, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "Ocurrió un error inesperado.")
        };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var response = ApiResponse<object>.Fail(message);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, options));
    }
}