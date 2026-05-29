namespace CompanyName.ProjectName.Domain.Exceptions;

/// <summary>
/// Se lanza cuando una solicitud no cumple las reglas de validación definidas.
/// </summary>
/// <remarks>
/// El middleware de excepciones mapea esta excepción a un código HTTP <b>400 Bad Request</b>.
/// </remarks>
/// <seealso cref="DomainException"/>
public class ValidationException(string message) : DomainException(message);