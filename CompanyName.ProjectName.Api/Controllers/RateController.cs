using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ExchangeRate.DTOs;
using CompanyName.ProjectName.Application.Features.ExchangeRate.Queries;

namespace CompanyName.ProjectName.Api.Controllers;

/// <summary>
/// Consulta de tasas de cambio en tiempo real
/// </summary>
/// <remarks>
/// Proporciona endpoints para la consulta de tasas de cambio obtenidas en tiempo real desde ExchangeRate-API.
/// Incluye reintentos automáticos con backoff exponencial y circuit breaker ante fallos del servicio externo.
/// </remarks>
[ApiController]
[ApiVersion("1.0")]
[Route("api/[controller]")]
[SwaggerTag("Consulta de tasas de cambio en tiempo real")]
public class RateController(IMediator mediator) : ControllerBase
{
    /// <summary>
    /// GET /api/rate/{baseCurrency}
    /// </summary>
    /// <remarks>
    /// Obtiene todas las tasas de cambio disponibles para una moneda base.
    ///
    /// **Consideraciones:**
    /// - El resultado se almacena en caché durante **1 hora** por moneda base.
    /// - El parámetro `baseCurrency` se convierte automáticamente a mayúsculas.
    ///
    /// **Excepciones:**
    /// - **400 Bad Request** — El parámetro `baseCurrency` no cumple las reglas de validación.
    /// - **502 Bad Gateway** — ExchangeRate-API no está disponible tras agotar los reintentos.
    /// </remarks>
    /// <param name="baseCurrency">Código ISO 4217 de la moneda base (ej: USD, EUR, CLP). Exactamente 3 caracteres.</param>
    /// <returns>
    /// Un <see cref="ExchangeRateDto"/> envuelto en <see cref="ApiResponse{T}"/>,
    /// conteniendo la moneda base y un diccionario con todas las tasas de cambio disponibles.
    /// </returns>
    /// <response code="200">Retorna todas las tasas de cambio para la moneda base.</response>
    /// <response code="400">El código de moneda no cumple las reglas de validación.</response>
    /// <response code="502">ExchangeRate-API no está disponible.</response>
    [HttpGet("{baseCurrency}")]
    [ProducesResponseType(typeof(ApiResponse<ExchangeRateDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetAllRates(string baseCurrency)
    {
        var response = await mediator.Send(new GetAllRatesQuery(baseCurrency));

        return Ok(response);
    }

    /// <summary>
    /// GET /api/rate/{fromCurrency}/{toCurrency}
    /// </summary>
    /// <remarks>
    /// Obtiene la tasa de cambio entre un par de monedas específico.
    ///
    /// **Consideraciones:**
    /// - Los parámetros `fromCurrency` y `toCurrency` se convierten automáticamente a mayúsculas.
    /// - Este endpoint no utiliza caché, garantizando el valor más actualizado disponible.
    ///
    /// **Excepciones:**
    /// - **400 Bad Request** — Alguno de los parámetros no cumple las reglas de validación,
    ///   o `fromCurrency` y `toCurrency` son iguales.
    /// - **502 Bad Gateway** — ExchangeRate-API no está disponible tras agotar los reintentos.
    /// </remarks>
    /// <param name="fromCurrency">Código ISO 4217 de la moneda origen (ej: USD). Exactamente 3 caracteres.</param>
    /// <param name="toCurrency">Código ISO 4217 de la moneda destino (ej: CLP). Exactamente 3 caracteres.</param>
    /// <returns>
    /// Un <see cref="ExchangeRatePairDto"/> envuelto en <see cref="ApiResponse{T}"/>,
    /// conteniendo la moneda origen, destino y la tasa de cambio vigente.
    /// </returns>
    /// <response code="200">Retorna la tasa de cambio del par solicitado.</response>
    /// <response code="400">Los códigos de moneda no cumplen las reglas de validación.</response>
    /// <response code="502">ExchangeRate-API no está disponible.</response>
    [HttpGet("{fromCurrency}/{toCurrency}")]
    [ProducesResponseType(typeof(ApiResponse<ExchangeRatePairDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status502BadGateway)]
    public async Task<IActionResult> GetRatePair(string fromCurrency, string toCurrency)
    {
        var response = await mediator.Send(new GetRatePairQuery(fromCurrency, toCurrency));

        return Ok(response);
    }
}