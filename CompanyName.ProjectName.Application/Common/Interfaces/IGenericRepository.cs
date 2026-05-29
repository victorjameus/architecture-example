namespace CompanyName.ProjectName.Application.Common.Interfaces;

/// <summary>
/// Define el contrato base para las operaciones de acceso a datos de una entidad.
/// </summary>
/// <remarks>
/// <para>
/// Abstrae las operaciones CRUD sobre la base de datos mediante Dapper, proporcionando
/// un contrato genérico reutilizable para cualquier entidad del dominio.
/// Las implementaciones utilizan reflexión para construir dinámicamente las sentencias SQL.
/// </para>
/// <list type="bullet">
///   <item><description>Las propiedades de auditoría son gestionadas automáticamente por la base de datos mediante valores <b>DEFAULT</b>.</description></item>
///   <item><description>Las propiedades de navegación de tipo complejo son excluidas automáticamente de las sentencias <c>INSERT</c> y <c>UPDATE</c>.</description></item>
///   <item><description>Las instancias son creadas y administradas por <see cref="IUnitOfWork"/>, no deben ser instanciadas directamente.</description></item>
/// </list>
/// </remarks>
/// <typeparam name="T">Tipo de entidad sobre la cual se ejecutan las operaciones. Debe ser una clase.</typeparam>
/// <seealso cref="IUnitOfWork"/>
public interface IGenericRepository<T> where T : class
{
    /// <summary>
    /// Obtiene todos los registros de la entidad desde la base de datos.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Ejecuta un <c>SELECT *</c> sobre la tabla correspondiente al tipo <typeparamref name="T"/>.</description></item>
    ///   <item><description>El nombre de la tabla se infiere automáticamente a partir del nombre del tipo de la entidad.</description></item>
    ///   <item><description>Para conjuntos de datos grandes, considere implementar paginación en la capa de aplicación.</description></item>
    /// </list>
    /// </remarks>
    /// <returns>
    /// Colección de entidades de tipo <typeparamref name="T"/>.
    /// Retorna una colección vacía si no existen registros.
    /// </returns>
    Task<IEnumerable<T>> GetAllAsync();

    /// <summary>
    /// Obtiene un registro de la entidad por su identificador único.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item><description>Ejecuta un <c>SELECT * WHERE Id = @Id</c> sobre la tabla correspondiente al tipo <typeparamref name="T"/>.</description></item>
    ///   <item><description>El nombre de la tabla se infiere automáticamente a partir del nombre del tipo de la entidad.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="id">Identificador único del registro a recuperar. Debe ser mayor a 0.</param>
    /// <returns>
    /// <list type="bullet">
    ///   <item><description>La entidad de tipo <typeparamref name="T"/> si existe.</description></item>
    ///   <item><description><c>null</c> si no se encontró el registro.</description></item>
    /// </list>
    /// </returns>
    Task<T?> GetByIdAsync(int id);

    /// <summary>
    /// Inserta un nuevo registro de la entidad en la base de datos.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Construye dinámicamente la sentencia <c>INSERT</c> mediante reflexión,
    /// excluyendo las propiedades que no deben ser enviadas a la base de datos.
    /// </para>
    /// <para>Las siguientes propiedades son excluidas automáticamente del <c>INSERT</c>:</para>
    /// <list type="bullet">
    ///   <item><description><c>Id</c> — generado automáticamente por la base de datos mediante <b>IDENTITY</b>.</description></item>
    ///   <item><description><c>CreatedAt</c> — asignado automáticamente por la base de datos mediante <b>DEFAULT</b>.</description></item>
    ///   <item><description><c>UpdatedAt</c> — asignado automáticamente por la base de datos mediante <b>DEFAULT</b>.</description></item>
    ///   <item><description><c>ConvertedAt</c> — asignado automáticamente por la base de datos mediante <b>DEFAULT</b>.</description></item>
    ///   <item><description>Propiedades de navegación de tipo complejo — excluidas para evitar conflictos con Dapper.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="entity">
    /// Entidad a insertar. Las propiedades de auditoría y navegación no deben ser asignadas
    /// antes de invocar este método.
    /// </param>
    /// <returns>
    /// Identificador único generado por la base de datos para el registro insertado.
    /// </returns>
    Task<int> AddAsync(T entity);

    /// <summary>
    /// Actualiza un registro existente de la entidad en la base de datos.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Construye dinámicamente la sentencia <c>UPDATE</c> mediante reflexión,
    /// incluyendo todas las propiedades de la entidad excepto <c>Id</c>.
    /// </para>
    /// <list type="bullet">
    ///   <item><description>La entidad debe contener el identificador único del registro a actualizar.</description></item>
    ///   <item><description>Se recomienda obtener la entidad mediante <see cref="GetByIdAsync"/> antes de modificarla para asegurar consistencia.</description></item>
    ///   <item><description>Las propiedades de navegación de tipo complejo son excluidas automáticamente del <c>UPDATE</c>.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="entity">
    /// Entidad con los datos actualizados. Debe contener el identificador único del registro.
    /// </param>
    /// <returns>
    /// <list type="bullet">
    ///   <item><description><c>true</c> — el registro fue actualizado exitosamente.</description></item>
    ///   <item><description><c>false</c> — no se encontró el registro.</description></item>
    /// </list>
    /// </returns>
    Task<bool> UpdateAsync(T entity);

    /// <summary>
    /// Elimina físicamente un registro de la entidad en la base de datos.
    /// </summary>
    /// <remarks>
    /// <para>Esta operación es <b>irreversible</b>.</para>
    /// <list type="bullet">
    ///   <item><description>Para entidades que requieren preservar la integridad referencial, considere utilizar un borrado lógico mediante la propiedad <c>IsActive</c>.</description></item>
    ///   <item><description>Borrado lógico — establece <c>IsActive</c> en <c>false</c>. Preserva el registro en la base de datos.</description></item>
    ///   <item><description>Borrado físico — elimina el registro <b>permanentemente</b> de la base de datos.</description></item>
    /// </list>
    /// </remarks>
    /// <param name="id">Identificador único del registro a eliminar. Debe ser mayor a 0.</param>
    /// <returns>
    /// <list type="bullet">
    ///   <item><description><c>true</c> — el registro fue eliminado exitosamente.</description></item>
    ///   <item><description><c>false</c> — no se encontró el registro.</description></item>
    /// </list>
    /// </returns>
    Task<bool> DeleteAsync(int id);
}