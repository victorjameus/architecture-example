using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Export.Commands;
using CompanyName.ProjectName.Application.Features.Export.DTOs;
using CompanyName.ProjectName.Application.Features.Export.Queries;

namespace CompanyName.ProjectName.Api.Controllers;

/// <summary>
/// Exportación del historial de conversiones a Azure Blob Storage
/// </summary>
/// <remarks>
/// Proporciona endpoints para la generación y consulta de exportaciones del historial de conversiones.
/// Los archivos se generan en formato CSV y se almacenan en Azure Blob Storage con acceso público.
/// Cada exportación queda registrada en la base de datos con su URL de descarga y metadatos asociados.
/// </remarks>
[ApiController]
[ApiVersion("1.0")]
[Route("api/conversions")]
[SwaggerTag("Exportación del historial de conversiones a Azure Blob Storage")]
public class ExportController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// GET /api/conversions/exports
    /// </summary>
    /// <remarks>
    /// Obtiene el listado de exportaciones generadas anteriormente.
    ///
    /// **Consideraciones:**
    /// - Retorna una lista vacía si no se han generado exportaciones.
    /// </remarks>
    /// <returns>
    /// Lista de <see cref="ConversionExportDto"/> envuelta en <see cref="ApiResponse{T}"/>,
    /// conteniendo la URL de descarga, fechas de filtro aplicadas y cantidad de registros de cada exportación.
    /// </returns>
    /// <response code="200">Retorna el listado de exportaciones generadas.</response>
    [HttpGet("exports")]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<ConversionExportDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await mediator.Send(new GetAllExportsQuery());

        return Ok(response);
    }

    /// <summary>
    /// POST /api/conversions/export
    /// </summary>
    /// <remarks>
    /// Genera un archivo CSV con el historial de conversiones y lo sube a Azure Blob Storage.
    ///
    /// **Información de columnas dentro del archivo:**
    /// | Columna         | Tipo        | Descripción                                          |
    /// |-----------------|-------------|------------------------------------------------------|
    /// | Id              | **int**     | Identificador único de la conversión.                |
    /// | FromCurrencyId  | **int**     | Identificador de la moneda origen.                   |
    /// | ToCurrencyId    | **int**     | Identificador de la moneda destino.                  |
    /// | Amount          | **decimal** | Monto original ingresado.                            |
    /// | ConvertedAmount | **decimal** | Monto resultante de la conversión.                   |
    /// | ExchangeRate    | **decimal** | Tasa de cambio utilizada al momento de la conversión.|
    /// | ConvertedAt     | **datetime**| Fecha y hora de la conversión en formato UTC.        |
    ///
    /// **Consideraciones:**
    /// - Si no se especifican fechas, se exportan **todos** los registros disponibles.
    /// - Si el rango no contiene registros, se genera un CSV vacío y se registra **Warning** en Application Insights.
    /// - El archivo generado es accesible públicamente mediante la URL retornada.
    /// - Incluye reintentos automáticos con backoff exponencial y circuit breaker ante fallos de Blob Storage.
    ///
    /// **Excepciones:**
    /// - **400 Bad Request** — El campo `DateFrom` es mayor que `DateTo`.
    /// - **502 Bad Gateway** — Azure Blob Storage no está disponible tras agotar los reintentos.
    /// </remarks>
    /// <param name="command">Comando con los filtros opcionales `DateFrom` y `DateTo` para el rango de exportación.</param>
    /// <returns>
    /// El <see cref="ConversionExportDto"/> recién creado envuelto en <see cref="ApiResponse{T}"/>,
    /// conteniendo el nombre del archivo, la URL pública de descarga y la cantidad de registros exportados.
    /// </returns>
    /// <response code="201">Exportación generada y subida a Blob Storage exitosamente.</response>
    /// <response code="400">Error de validación en el cuerpo de la solicitud.</response>
    /// <response code="502">Azure Blob Storage no está disponible.</response>
    [HttpPost("export")]
    [ProducesResponseType(typeof(ApiResponse<ConversionExportDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Create([FromBody] CreateConversionExportCommand command)
    {
        var response = await mediator.Send(command);

        return CreatedAtAction(nameof(GetAll), response);
    }
}