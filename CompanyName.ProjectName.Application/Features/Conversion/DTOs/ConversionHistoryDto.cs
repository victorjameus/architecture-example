namespace CompanyName.ProjectName.Application.Features.Conversion.DTOs;

public record ConversionHistoryDto
(
    int Id,
    string FromCurrency,
    string ToCurrency,
    decimal Amount,
    decimal ConvertedAmount,
    decimal ExchangeRate,
    DateTime ConvertedAt
);