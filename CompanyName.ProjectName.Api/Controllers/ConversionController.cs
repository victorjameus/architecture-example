using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ConversionHistory.Commands;
using CompanyName.ProjectName.Application.Features.ConversionHistory.DTOs;
using CompanyName.ProjectName.Application.Features.ConversionHistory.Queries;

namespace CompanyName.ProjectName.Api.Controllers;

/// <summary>
/// Historial de conversiones de moneda
/// </summary>
/// <remarks>
/// Proporciona operaciones sobre el historial de conversiones de moneda.
/// Las conversiones se realizan en tiempo real consultando ExchangeRate-API
/// y la tasa utilizada queda registrada para fines de auditoría.
/// </remarks>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[SwaggerTag("Historial de conversiones de moneda")]
public class ConversionController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// GET /api/conversion
    /// </summary>
    /// <remarks>
    /// Obtiene el historial de conversiones.
    ///
    /// **Consideraciones:**
    /// - Los resultados se retornan de forma paginada. Utilice los parámetros `page` y `pageSize`
    ///   para controlar la navegación entre páginas.
    ///
    /// **Excepciones:**
    /// - **400 Bad Request** — Los parámetros `page` o `pageSize` no cumplen las reglas de validación.
    /// </remarks>
    /// <param name="page">Número de página a recuperar. Debe ser mayor a 0. Valor por defecto: 1.</param>
    /// <param name="pageSize">Cantidad de registros por página. Debe ser mayor a 0 y no superar 100. Valor por defecto: 10.</param>
    /// <returns>
    /// Un <see cref="PagedResponse{T}"/> de <see cref="ConversionHistoryDto"/> envuelto en <see cref="ApiResponse{T}"/>,
    /// incluyendo metadatos de paginación como total de registros, páginas y navegación.
    /// </returns>
    /// <response code="200">Retorna el historial paginado de conversiones.</response>
    /// <response code="400">Los parámetros de paginación no cumplen las reglas de validación.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResponse<ConversionHistoryDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var response = await mediator.Send(new GetAllConversionsQuery(page, pageSize));

        return Ok(response);
    }

    /// <summary>
    /// GET /api/conversion/{id}
    /// </summary>
    /// <remarks>
    /// Obtiene una conversión específica por su identificador único.
    ///
    /// **Excepciones:**
    /// - **404 Not Found** — No existe una conversión con el `id` especificado.
    /// </remarks>
    /// <param name="id">Identificador único de la conversión.</param>
    /// <returns>
    /// Un <see cref="ConversionHistoryDto"/> envuelto en <see cref="ApiResponse{T}"/> si existe.
    /// </returns>
    /// <response code="200">Retorna la conversión solicitada.</response>
    /// <response code="404">No se encontró una conversión con el id especificado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<ConversionHistoryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await mediator.Send(new GetConversionByIdQuery(id));

        return Ok(response);
    }

    /// <summary>
    /// POST /api/conversion
    /// </summary>
    /// <remarks>
    /// Realiza una conversión de moneda en tiempo real.
    ///
    /// **Instrucciones de uso:**
    /// - El campo `FromCurrencyId` debe corresponder a una moneda activa en el catálogo.
    /// - El campo `ToCurrencyId` debe corresponder a una moneda activa en el catálogo.
    /// - Los campos `FromCurrencyId` y `ToCurrencyId` no pueden ser iguales.
    /// - El campo `Amount` debe ser mayor a 0.
    ///
    /// **Consideraciones:**
    /// - La tasa de cambio se obtiene en tiempo real desde ExchangeRate-API.
    /// - Incluye reintentos automáticos con backoff exponencial y circuit breaker.
    /// - La tasa utilizada queda registrada para auditoría independientemente de variaciones futuras.
    ///
    /// **Excepciones:**
    /// - **400 Bad Request** — Algún campo no cumple las reglas de validación.
    /// - **404 Not Found** — La moneda origen o destino no existe en el catálogo.
    /// - **502 Bad Gateway** — ExchangeRate-API no está disponible tras agotar los reintentos.
    /// </remarks>
    /// <param name="command">Comando con los identificadores de moneda origen, destino y el monto a convertir.</param>
    /// <returns>
    /// El <see cref="ConversionHistoryDto"/> recién creado envuelto en <see cref="ApiResponse{T}"/>,
    /// incluyendo el monto convertido, la tasa de cambio utilizada y la fecha de conversión.
    /// </returns>
    /// <response code="201">Conversión realizada y registrada exitosamente.</response>
    /// <response code="400">Error de validación en el cuerpo de la solicitud.</response>
    /// <response code="404">La moneda origen o destino no existe en el catálogo.</response>
    /// <response code="502">ExchangeRate-API no está disponible.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ConversionHistoryDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> Create([FromBody] CreateConversionCommand command)
    {
        var response = await mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    /// <summary>
    /// DELETE /api/conversion/{id}
    /// </summary>
    /// <remarks>
    /// Elimina un registro del historial de conversiones.
    ///
    /// **Consideraciones:**
    /// - Esta operación realiza un borrado físico del registro.
    /// - Una vez eliminado, no es posible recuperar la información de la conversión.
    ///
    /// **Excepciones:**
    /// - **404 Not Found** — No existe una conversión con el `id` especificado.
    /// </remarks>
    /// <param name="id">Identificador único de la conversión a eliminar.</param>
    /// <returns>
    /// Un <see cref="bool"/> envuelto en <see cref="ApiResponse{T}"/> indicando el resultado de la operación.
    /// </returns>
    /// <response code="200">Conversión eliminada exitosamente.</response>
    /// <response code="404">No se encontró una conversión con el id especificado.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await mediator.Send(new DeleteConversionCommand(id));

        return Ok(response);
    }
}