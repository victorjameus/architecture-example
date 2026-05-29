namespace CompanyName.ProjectName.Application.Common.Interfaces;

/// <summary>
/// Define el contrato para las operaciones de caché en memoria.
/// </summary>
/// <remarks>
/// <para>
/// Abstrae el almacenamiento en caché mediante <c>IMemoryCache</c> de .NET,
/// proporcionando un contrato simplificado para operaciones de lectura, escritura y eliminación.
/// Las implementaciones utilizan caché en memoria del proceso, por lo que los datos
/// no son compartidos entre instancias de la aplicación.
/// </para>
/// <list type="bullet">
///   <item><description>Este servicio debe registrarse con ciclo de vida <b>Singleton</b> para compartir el caché entre todos los requests.</description></item>
///   <item><description>El caché es volátil — los datos se pierden al reiniciar la aplicación.</description></item>
///   <item><description>Para entornos con múltiples instancias, considere reemplazar la implementación por un caché distribuido como Redis.</description></item>
/// </list>
/// </remarks>
/// <seealso cref="IGenericRepository{T}"/>
public interface ICacheService
{
    /// <summary>
    /// Obtiene un valor almacenado en caché por su clave.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Si la clave no existe o ha expirado, retorna el valor por defecto del tipo <typeparamref name="T"/>.</description></item>
    ///   <item><description>No lanza excepción si la clave no existe.</description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">Tipo del valor almacenado en caché.</typeparam>
    /// <param name="key">
    /// Clave única que identifica el valor en caché.
    /// Se recomienda usar el formato <c>entidad:identificador</c> (ej: <c>currencies:all</c>, <c>rates:USD</c>).
    /// </param>
    /// <returns>
    /// <list type="bullet">
    ///   <item><description>El valor almacenado de tipo <typeparamref name="T"/> si la clave existe y no ha expirado.</description></item>
    ///   <item><description>El valor por defecto de <typeparamref name="T"/> si la clave no existe o ha expirado.</description></item>
    /// </list>
    /// </returns>
    T? Get<T>(string key);

    /// <summary>
    /// Almacena un valor en caché con un tiempo de expiración absoluto.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Si ya existe un valor con la misma clave, será <b>sobreescrito</b>.</description></item>
    ///   <item><description>La expiración es absoluta — el valor expira transcurrido el tiempo indicado independientemente de si fue accedido.</description></item>
    ///   <item><description>Tras la expiración, el valor es eliminado automáticamente de la memoria.</description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">Tipo del valor a almacenar en caché.</typeparam>
    /// <param name="key">
    /// Clave única que identifica el valor en caché.
    /// Se recomienda usar el formato <c>entidad:identificador</c> (ej: <c>currencies:all</c>, <c>rates:USD</c>).
    /// </param>
    /// <param name="value">Valor a almacenar en caché.</param>
    /// <param name="expiration">Tiempo de expiración absoluto a partir del momento de almacenamiento.</param>
    void Set<T>(string key, T value, TimeSpan expiration);

    /// <summary>
    /// Elimina un valor almacenado en caché por su clave.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Si la clave no existe, la operación no tiene efecto y no lanza excepción.</description></item>
    ///   <item><description>Invocar este método al crear, actualizar o eliminar entidades para garantizar la consistencia del caché.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="key">Clave única del valor a eliminar del caché.</param>
    void Remove(string key);
}