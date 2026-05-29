using CompanyName.ProjectName.Domain.Enums;

namespace CompanyName.ProjectName.Application.Common.Interfaces;

/// <summary>
/// Define el contrato para el registro de telemetría en Application Insights.
/// </summary>
/// <remarks>
/// <para>
/// Abstrae la comunicación con Azure Application Insights, proporcionando métodos
/// para registrar eventos, trazas, excepciones y dependencias de forma estructurada.
/// Las implementaciones utilizan <c>TelemetryClient</c> del SDK de Application Insights.
/// </para>
/// <list type="bullet">
///   <item><description>Todos los eventos registrados quedan correlacionados automáticamente bajo el <c>Operation ID</c> del request HTTP en curso.</description></item>
///   <item><description>Las dependencias a servicios externos como SQL Server y Flurl son capturadas automáticamente por Application Insights sin necesidad de invocar <see cref="TrackDependency"/>.</description></item>
///   <item><description>Este servicio debe registrarse con ciclo de vida <b>Singleton</b> ya que <c>TelemetryClient</c> es Singleton por diseño.</description></item>
/// </list>
/// </remarks>
/// <seealso cref="InsightLevel"/>
public interface IInsightService
{
    /// <summary>
    /// Registra un evento de negocio personalizado en Application Insights.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Utilizar para eventos relevantes del dominio como conversiones realizadas, exportaciones generadas o monedas creadas.</description></item>
    ///   <item><description>Las propiedades adicionales permiten enriquecer el evento con contexto de negocio para análisis posterior.</description></item>
    ///   <item><description>Los eventos se visualizan en Application Insights bajo la categoría <b>Custom Event</b>.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="eventName">
    /// Nombre descriptivo del evento (ej: <c>ConversionRealizada</c>, <c>MonedaCreada</c>, <c>ExportGenerado</c>).
    /// Se recomienda usar PascalCase sin espacios.
    /// </param>
    /// <param name="properties">
    /// Diccionario opcional con propiedades adicionales del evento.
    /// Las claves y valores deben ser cadenas de texto.
    /// </param>
    void TrackEvent(string eventName, Dictionary<string, string>? properties = null);

    /// <summary>
    /// Registra una excepción en Application Insights.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Application Insights captura automáticamente las excepciones no controladas mediante el middleware de excepciones. Invocar este método solo cuando se requiera contexto adicional.</description></item>
    ///   <item><description>Las excepciones se visualizan en Application Insights bajo la categoría <b>Exception</b> con stack trace completo.</description></item>
    ///   <item><description>Las propiedades adicionales permiten agregar contexto relevante como el path del request o el identificador del recurso.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="exception">Excepción a registrar. Incluye el stack trace completo.</param>
    /// <param name="properties">
    /// Diccionario opcional con propiedades adicionales.
    /// Las claves y valores deben ser cadenas de texto.
    /// </param>
    void TrackException(Exception exception, Dictionary<string, string>? properties = null);

    /// <summary>
    /// Registra un mensaje de traza con nivel de severidad en Application Insights.
    /// </summary>
    /// <remarks>
    /// <para>Niveles de severidad recomendados según el contexto:</para>
    /// <list type="bullet">
    ///   <item><description><see cref="InsightLevel.Information"/> — flujo normal exitoso. Solo en ambiente de desarrollo.</description></item>
    ///   <item><description><see cref="InsightLevel.Warning"/> — comportamiento inusual que no interrumpe el flujo (ej: servicio externo lento, exportación sin registros).</description></item>
    ///   <item><description><see cref="InsightLevel.Error"/> — fallo recuperable en un servicio externo.</description></item>
    ///   <item><description><see cref="InsightLevel.Critical"/> — fallo crítico que requiere atención inmediata.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="message">Mensaje descriptivo de la traza.</param>
    /// <param name="level">Nivel de severidad de la traza. Ver <see cref="InsightLevel"/> para los valores disponibles.</param>
    /// <param name="properties">
    /// Diccionario opcional con propiedades adicionales.
    /// Las claves y valores deben ser cadenas de texto.
    /// </param>
    void TrackTrace(string message, InsightLevel level, Dictionary<string, string>? properties = null);

    /// <summary>
    /// Registra una dependencia a un servicio externo en Application Insights.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Utilizar únicamente para servicios externos que <b>no</b> son capturados automáticamente por Application Insights.</description></item>
    ///   <item><description>SQL Server y las llamadas HTTP realizadas con Flurl son capturadas automáticamente. <b>No invocar</b> este método para esos casos.</description></item>
    ///   <item><description>Las dependencias se visualizan en Application Insights bajo la categoría <b>Dependency</b> correlacionadas con el request en curso.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="dependencyType">Tipo de dependencia (ej: <c>HTTP</c>, <c>AzureBlob</c>, <c>SQL</c>).</param>
    /// <param name="target">Nombre del servicio o host de destino (ej: <c>v6.exchangerate-api.com</c>).</param>
    /// <param name="name">Nombre descriptivo de la operación (ej: <c>GET pair/USD/CLP</c>).</param>
    /// <param name="startTime">Marca de tiempo de inicio de la operación en formato <c>DateTimeOffset</c>.</param>
    /// <param name="duration">Duración total de la operación.</param>
    /// <param name="success">Indica si la operación fue exitosa.</param>
    void TrackDependency(string dependencyType, string target, string name, DateTimeOffset startTime, TimeSpan duration, bool success);
}