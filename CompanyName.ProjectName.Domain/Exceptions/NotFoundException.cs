namespace CompanyName.ProjectName.Domain.Exceptions;

public class NotFoundException(string message) : DomainException(message);