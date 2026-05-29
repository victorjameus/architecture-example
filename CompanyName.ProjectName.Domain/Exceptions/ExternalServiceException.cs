namespace CompanyName.ProjectName.Domain.Exceptions;

/// <summary>
/// Se lanza cuando un servicio externo no está disponible o retorna un error inesperado.
/// </summary>
/// <remarks>
/// El middleware de excepciones mapea esta excepción a un código HTTP <b>502 Bad Gateway</b>.
/// Se utiliza para fallos en ExchangeRate-API y Azure Blob Storage tras agotar los reintentos configurados.
/// </remarks>
/// <seealso cref="DomainException"/>
public class ExternalServiceException(string message) : DomainException(message);