using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ExchangeRate.DTOs;

namespace CompanyName.ProjectName.Application.Features.ExchangeRate.Queries;

public record GetRatePairQuery(string FromCurrency, string ToCurrency) : IRequest<ApiResponse<ExchangeRatePairDto>>;

public sealed class GetRatePairQueryHandler(IExchangeRateService exchangeRateService) : IRequestHandler<GetRatePairQuery, ApiResponse<ExchangeRatePairDto>>
{
    public async Task<ApiResponse<ExchangeRatePairDto>> Handle(GetRatePairQuery request, CancellationToken ct)
    {
        var rate = await exchangeRateService.GetRateAsync(request.FromCurrency.ToUpper(), request.ToCurrency.ToUpper());
        var dto = new ExchangeRatePairDto(request.FromCurrency.ToUpper(), request.ToCurrency.ToUpper(), rate);

        return ApiResponse<ExchangeRatePairDto>.Ok(dto);
    }
}