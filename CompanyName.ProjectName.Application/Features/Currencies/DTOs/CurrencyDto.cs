namespace CompanyName.ProjectName.Application.Features.Currencies.DTOs;

public record CurrencyDto
(
    int Id,
    string Code,
    string Name,
    string Symbol,
    bool IsActive,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);