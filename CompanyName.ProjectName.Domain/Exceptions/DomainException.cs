namespace CompanyName.ProjectName.Domain.Exceptions;

/// <summary>
/// Excepción base del dominio. Representa un error originado por una regla de negocio.
/// </summary>
/// <remarks>
/// Todas las excepciones del dominio heredan de esta clase.
/// El middleware de excepciones captura las derivadas y retorna el código HTTP correspondiente.
/// </remarks>
/// <seealso cref="NotFoundException"/>
/// <seealso cref="ValidationException"/>
/// <seealso cref="ConflictException"/>
/// <seealso cref="ExternalServiceException"/>
public class DomainException(string message) : Exception(message);