using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Currencies.Commands;
using CompanyName.ProjectName.Application.Features.Currencies.DTOs;
using CompanyName.ProjectName.Application.Features.Currencies.Queries;

namespace CompanyName.ProjectName.Api.Controllers;

/// <summary>
/// Gestión del catálogo de monedas
/// </summary>
/// <remarks>
/// Proporciona operaciones CRUD sobre el catálogo de monedas.
/// Las eliminaciones son lógicas para preservar la integridad referencial con el historial de conversiones.
/// </remarks>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[SwaggerTag("Gestión del catálogo de monedas")]
public class CurrenciesController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// GET /api/currency
    /// </summary>
    /// <remarks>
    /// Obtiene todas las monedas activas del catálogo.
    ///
    /// **Consideraciones:**
    /// - El resultado se almacena en caché durante **5 minutos**.
    /// - El caché se invalida automáticamente al crear, actualizar o desactivar una moneda.
    /// </remarks>
    /// <returns>
    /// Lista de <see cref="CurrencyDto"/> envuelta en <see cref="ApiResponse{T}"/>.
    /// Retorna una lista vacía si no existen monedas activas.
    /// </returns>
    /// <response code="200">Retorna la lista de monedas activas.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<CurrencyDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var response = await mediator.Send(new GetAllCurrenciesQuery());

        return Ok(response);
    }

    /// <summary>
    /// GET /api/currency/{id}
    /// </summary>
    /// <remarks>
    /// Obtiene una moneda específica por su identificador único.
    ///
    /// **Excepciones:**
    /// - **404 Not Found** — No existe una moneda con el `id` especificado.
    /// </remarks>
    /// <param name="id">Identificador único de la moneda.</param>
    /// <returns>
    /// Un <see cref="CurrencyDto"/> envuelto en <see cref="ApiResponse{T}"/> si existe.
    /// </returns>
    /// <response code="200">Retorna la moneda solicitada.</response>
    /// <response code="404">No se encontró una moneda con el id especificado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CurrencyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var response = await mediator.Send(new GetCurrencyByIdQuery(id));

        return Ok(response);
    }

    /// <summary>
    /// POST /api/currency
    /// </summary>
    /// <remarks>
    /// Crea una nueva moneda en el catálogo.
    ///
    /// **Instrucciones de uso:**
    /// - El campo `Code` debe ser un código ISO 4217 de exactamente **3 caracteres** (ej: USD, EUR, CLP).
    /// - El campo `Code` se convierte automáticamente a mayúsculas.
    /// - El campo `Name` no debe superar los 100 caracteres.
    /// - El campo `Symbol` no debe superar los 10 caracteres.
    ///
    /// **Consideraciones:**
    /// - La creación invalida el caché del listado de monedas.
    ///
    /// **Excepciones:**
    /// - **400 Bad Request** — Algún campo no cumple las reglas de validación.
    /// - **409 Conflict** — Ya existe una moneda con el mismo `Code`.
    /// </remarks>
    /// <param name="command">Comando con los datos de la moneda a crear.</param>
    /// <returns>
    /// El <see cref="CurrencyDto"/> recién creado envuelto en <see cref="ApiResponse{T}"/>,
    /// incluyendo el identificador generado y la fecha de creación.
    /// </returns>
    /// <response code="201">Moneda creada exitosamente.</response>
    /// <response code="400">Error de validación en el cuerpo de la solicitud.</response>
    /// <response code="409">Ya existe una moneda con el mismo código.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CurrencyDto>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreateCurrencyCommand command)
    {
        var response = await mediator.Send(command);

        return CreatedAtAction(nameof(GetById), new { id = response.Data!.Id }, response);
    }

    /// <summary>
    /// PUT /api/currency/{id}
    /// </summary>
    /// <remarks>
    /// Actualiza los datos de una moneda existente en el catálogo.
    ///
    /// **Instrucciones de uso:**
    /// - El campo `Code` debe ser un código ISO 4217 de exactamente **3 caracteres**.
    /// - El campo `Code` se convierte automáticamente a mayúsculas.
    /// - El campo `Name` no debe superar los 100 caracteres.
    /// - El campo `Symbol` no debe superar los 10 caracteres.
    ///
    /// **Consideraciones:**
    /// - La actualización invalida el caché del listado de monedas.
    ///
    /// **Excepciones:**
    /// - **400 Bad Request** — Algún campo no cumple las reglas de validación.
    /// - **404 Not Found** — No existe una moneda con el `id` especificado.
    /// - **409 Conflict** — Otra moneda ya tiene el mismo `Code`.
    /// </remarks>
    /// <param name="id">Identificador único de la moneda a actualizar.</param>
    /// <param name="command">Comando con los datos actualizados de la moneda.</param>
    /// <returns>
    /// El <see cref="CurrencyDto"/> actualizado envuelto en <see cref="ApiResponse{T}"/>.
    /// </returns>
    /// <response code="200">Moneda actualizada exitosamente.</response>
    /// <response code="400">Error de validación en el cuerpo de la solicitud.</response>
    /// <response code="404">No se encontró una moneda con el id especificado.</response>
    /// <response code="409">Otra moneda ya tiene el mismo código.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<CurrencyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCurrencyCommand command)
    {
        var response = await mediator.Send(command with { Id = id });

        return Ok(response);
    }

    /// <summary>
    /// DELETE /api/currency/{id}
    /// </summary>
    /// <remarks>
    /// Desactiva una moneda del catálogo mediante borrado lógico.
    ///
    /// **Consideraciones:**
    /// - Esta operación establece `IsActive` en false. El registro no se elimina físicamente
    ///   para preservar la integridad referencial con los registros de `ConversionHistory`.
    /// - La desactivación invalida el caché del listado de monedas.
    ///
    /// **Excepciones:**
    /// - **404 Not Found** — No existe una moneda con el `id` especificado.
    /// </remarks>
    /// <param name="id">Identificador único de la moneda a desactivar.</param>
    /// <returns>
    /// Un <see cref="bool"/> envuelto en <see cref="ApiResponse{T}"/> indicando el resultado de la operación.
    /// </returns>
    /// <response code="200">Moneda desactivada exitosamente.</response>
    /// <response code="404">No se encontró una moneda con el id especificado.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var response = await mediator.Send(new DeleteCurrencyCommand(id));

        return Ok(response);
    }
}