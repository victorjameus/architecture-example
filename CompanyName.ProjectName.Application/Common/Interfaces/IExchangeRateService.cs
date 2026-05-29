using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Common.Interfaces;

/// <summary>
/// Define el contrato para la consulta de tasas de cambio desde un servicio externo.
/// </summary>
/// <remarks>
/// <para>
/// Abstrae la comunicación con ExchangeRate-API, proporcionando métodos para consultar
/// tasas de cambio en tiempo real. Las implementaciones deben garantizar resiliencia
/// ante fallos del servicio externo.
/// </para>
/// <list type="bullet">
///   <item><description>Los códigos de moneda son convertidos automáticamente a mayúsculas antes de realizar la consulta.</description></item>
///   <item><description>Las implementaciones deben incorporar reintentos automáticos con backoff exponencial y circuit breaker.</description></item>
///   <item><description>El caché de resultados es responsabilidad de la capa de aplicación, no de las implementaciones de esta interfaz.</description></item>
/// </list>
/// </remarks>
/// <seealso cref="IBlobStorageService"/>
/// <seealso cref="IInsightService"/>
public interface IExchangeRateService
{
    /// <summary>
    /// Obtiene la tasa de cambio vigente entre dos monedas específicas.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Consulta la tasa en tiempo real desde ExchangeRate-API. A diferencia de
    /// <see cref="GetAllRatesAsync"/>, este método retorna únicamente la tasa
    /// del par solicitado, optimizando el volumen de datos transferidos.
    /// </para>
    /// <list type="bullet">
    ///   <item><description>Cada invocación genera una nueva solicitud al servicio externo. El resultado <b>no</b> es almacenado en caché por este método.</description></item>
    ///   <item><description>Si el servicio externo falla, se ejecutan hasta <b>3 reintentos</b> con backoff exponencial antes de lanzar la excepción.</description></item>
    ///   <item><description>Si el circuit breaker está abierto, la excepción se lanza inmediatamente sin realizar reintentos.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="fromCurrency">
    /// Código ISO 4217 de la moneda origen. Debe tener exactamente <b>3 caracteres</b> (ej: USD, EUR, GBP).
    /// El valor se convierte automáticamente a mayúsculas.
    /// </param>
    /// <param name="toCurrency">
    /// Código ISO 4217 de la moneda destino. Debe tener exactamente <b>3 caracteres</b> (ej: CLP, BRL, JPY).
    /// El valor se convierte automáticamente a mayúsculas.
    /// No puede ser igual a <paramref name="fromCurrency"/>.
    /// </param>
    /// <returns>
    /// Tasa de cambio vigente entre las monedas especificadas expresada como valor <c>decimal</c>.
    /// </returns>
    /// <exception cref="ExternalServiceException">
    /// Se lanza cuando ExchangeRate-API no está disponible tras agotar los reintentos configurados,
    /// o cuando el circuit breaker se encuentra en estado abierto.
    /// </exception>
    Task<decimal> GetRateAsync(string fromCurrency, string toCurrency);

    /// <summary>
    /// Obtiene todas las tasas de cambio disponibles para una moneda base.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Consulta la totalidad de las tasas vigentes desde ExchangeRate-API para una moneda base.
    /// A diferencia de <see cref="GetRateAsync"/>, este método retorna un diccionario completo
    /// con todas las monedas disponibles, lo que lo hace adecuado para consultas masivas.
    /// </para>
    /// <list type="bullet">
    ///   <item><description>El resultado es almacenado en caché durante <b>1 hora</b> por moneda base en la capa de aplicación.</description></item>
    ///   <item><description>Si el servicio externo falla, se ejecutan hasta <b>3 reintentos</b> con backoff exponencial antes de lanzar la excepción.</description></item>
    ///   <item><description>Si el circuit breaker está abierto, la excepción se lanza inmediatamente sin realizar reintentos.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="baseCurrency">
    /// Código ISO 4217 de la moneda base. Debe tener exactamente <b>3 caracteres</b> (ej: USD, EUR, CLP).
    /// El valor se convierte automáticamente a mayúsculas.
    /// </param>
    /// <returns>
    /// Diccionario donde la clave es el código ISO 4217 de la moneda destino
    /// y el valor es la tasa de cambio expresada como <c>decimal</c>.
    /// </returns>
    /// <exception cref="ExternalServiceException">
    /// Se lanza cuando ExchangeRate-API no está disponible tras agotar los reintentos configurados,
    /// o cuando el circuit breaker se encuentra en estado abierto.
    /// </exception>
    Task<Dictionary<string, decimal>> GetAllRatesAsync(string baseCurrency);
}