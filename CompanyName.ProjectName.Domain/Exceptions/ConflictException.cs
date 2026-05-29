namespace CompanyName.ProjectName.Domain.Exceptions;

/// <summary>
/// Se lanza cuando una operación entra en conflicto con el estado actual de un recurso.
/// </summary>
/// <remarks>
/// El middleware de excepciones mapea esta excepción a un código HTTP <b>409 Conflict</b>.
/// Se utiliza principalmente para violaciones de restricciones únicas como códigos de moneda duplicados.
/// </remarks>
/// <seealso cref="DomainException"/>
public class ConflictException(string message) : DomainException(message);