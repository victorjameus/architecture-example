namespace CompanyName.ProjectName.Application.Common.Interfaces;

/// <summary>
/// Define el contrato para las operaciones de almacenamiento en Azure Blob Storage.
/// </summary>
/// <remarks>
/// <para>
/// Abstrae la comunicación con Azure Blob Storage, proporcionando métodos para
/// subir y eliminar archivos en un contenedor configurado. Las implementaciones
/// deben garantizar resiliencia ante fallos del servicio externo.
/// </para>
/// <list type="bullet">
///   <item><description>Los archivos subidos son accesibles públicamente mediante la URL retornada.</description></item>
///   <item><description>El contenedor de destino es configurado mediante <c>AzureBlobStorage:ContainerName</c> en el archivo de configuración.</description></item>
///   <item><description>Las implementaciones deben incorporar reintentos automáticos con backoff exponencial y circuit breaker.</description></item>
/// </list>
/// </remarks>
/// <seealso cref="IExchangeRateService"/>
/// <seealso cref="IInsightService"/>
public interface IBlobStorageService
{
    /// <summary>
    /// Sube un archivo al contenedor de Azure Blob Storage configurado.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Si el contenedor no existe, es creado automáticamente con acceso público.</description></item>
    ///   <item><description>Si ya existe un archivo con el mismo nombre, será <b>sobreescrito</b>.</description></item>
    ///   <item><description>El stream es consumido completamente durante la carga. No debe ser reutilizado tras invocar este método.</description></item>
    ///   <item><description>Si el servicio externo falla, se ejecutan hasta <b>3 reintentos</b> con backoff exponencial antes de lanzar la excepción.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="content">
    /// Stream con el contenido del archivo a subir.
    /// Debe estar posicionado al inicio antes de invocar este método.
    /// </param>
    /// <param name="fileName">
    /// Nombre del archivo en el contenedor, incluyendo extensión (ej: <c>export_20260529_120000.csv</c>).
    /// Se recomienda incluir un timestamp para garantizar unicidad.
    /// Los archivos CSV de exportación incluyen los códigos ISO 4217 de las monedas en vez de sus identificadores.
    /// </param>
    /// <param name="contentType">
    /// Tipo MIME del archivo (ej: <c>text/csv</c>, <c>application/pdf</c>).
    /// </param>
    /// <returns>
    /// URL pública del archivo subido en Azure Blob Storage.
    /// </returns>
    /// <exception cref="Domain.Exceptions.ExternalServiceException">
    /// Se lanza cuando Azure Blob Storage no está disponible tras agotar los reintentos configurados,
    /// o cuando el circuit breaker se encuentra en estado abierto.
    /// </exception>
    Task<string> UploadAsync(Stream content, string fileName, string contentType);

    /// <summary>
    /// Elimina un archivo del contenedor de Azure Blob Storage configurado.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Si el archivo no existe, la operación retorna <c>false</c> sin lanzar una excepción.</description></item>
    ///   <item><description>Esta operación es <b>irreversible</b>. El archivo eliminado no puede ser recuperado.</description></item>
    ///   <item><description>Si el servicio externo falla, se ejecutan hasta <b>3 reintentos</b> con backoff exponencial antes de lanzar la excepción.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="fileName">
    /// Nombre del archivo a eliminar en el contenedor, incluyendo extensión (ej: <c>export_20260529_120000.csv</c>).
    /// </param>
    /// <returns>
    /// <list type="bullet">
    ///   <item><description><c>true</c> — el archivo fue eliminado exitosamente.</description></item>
    ///   <item><description><c>false</c> — el archivo no existe en el contenedor.</description></item>
    /// </list>
    /// </returns>
    /// <exception cref="Domain.Exceptions.ExternalServiceException">
    /// Se lanza cuando Azure Blob Storage no está disponible tras agotar los reintentos configurados,
    /// o cuando el circuit breaker se encuentra en estado abierto.
    /// </exception>
    Task<bool> DeleteAsync(string fileName);
}