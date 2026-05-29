namespace CompanyName.ProjectName.Application.Common.Interfaces;

/// <summary>
/// Define el contrato para la gestión de transacciones y acceso a repositorios.
/// </summary>
/// <remarks>
/// <para>
/// Coordina las operaciones de escritura sobre múltiples repositorios dentro
/// de una misma transacción, garantizando la consistencia de los datos.
/// Las implementaciones utilizan Dapper sobre una conexión compartida de SQL Server.
/// </para>
/// <para>Ciclo de vida recomendado:</para>
/// <list type="number">
///   <item><description>Obtener el repositorio mediante <see cref="Repository{T}"/>.</description></item>
///   <item><description>Ejecutar las operaciones de escritura necesarias.</description></item>
///   <item><description>Confirmar los cambios mediante <see cref="SaveChangesAsync"/>.</description></item>
/// </list>
/// </remarks>
/// <seealso cref="IGenericRepository{T}"/>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Obtiene el repositorio genérico para una entidad específica.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Los repositorios se instancian una única vez por entidad y se reutilizan durante el ciclo de vida del <see cref="IUnitOfWork"/>.</description></item>
    ///   <item><description>La conexión a la base de datos se abre automáticamente si aún no está abierta.</description></item>
    ///   <item><description>El nombre de la tabla es inferido automáticamente a partir del nombre del tipo <typeparamref name="T"/>.</description></item>
    /// </list>
    /// </remarks>
    /// <typeparam name="T">Tipo de entidad para la cual se obtiene el repositorio. Debe ser una clase.</typeparam>
    /// <returns>
    /// Instancia de <see cref="IGenericRepository{T}"/> para la entidad especificada.
    /// </returns>
    IGenericRepository<T> Repository<T>() where T : class;

    /// <summary>
    /// Confirma todos los cambios pendientes en la base de datos.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Si existe una transacción activa, realiza el <b>commit</b> de la misma.</description></item>
    ///   <item><description>En caso de error, ejecuta automáticamente el <b>rollback</b> y relanza la excepción.</description></item>
    ///   <item><description>Tras la confirmación, los repositorios en caché son liberados y la transacción es descartada.</description></item>
    /// </list>
    /// </remarks>
    /// <returns>
    /// Número de registros afectados por las operaciones confirmadas.
    /// </returns>
    /// <exception cref="Domain.Exceptions.DomainException">
    /// Se lanza cuando ocurre un error durante el commit. El rollback se ejecuta automáticamente.
    /// </exception>
    Task<int> SaveChangesAsync();
}