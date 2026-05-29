namespace CompanyName.ProjectName.Domain.Exceptions;

/// <summary>
/// Se lanza cuando un recurso solicitado no existe en el sistema.
/// </summary>
/// <remarks>
/// El middleware de excepciones mapea esta excepción a un código HTTP <b>404 Not Found</b>.
/// </remarks>
/// <seealso cref="DomainException"/>
public class NotFoundException(string message) : DomainException(message);