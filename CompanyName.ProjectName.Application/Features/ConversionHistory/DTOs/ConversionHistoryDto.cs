namespace CompanyName.ProjectName.Application.Features.ConversionHistory.DTOs;

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