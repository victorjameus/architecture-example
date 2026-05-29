using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ExchangeRate.DTOs;

namespace CompanyName.ProjectName.Application.Features.ExchangeRate.Queries;

public record GetAllRatesQuery(string BaseCurrency) : IRequest<ApiResponse<ExchangeRateDto>>;

public sealed class GetAllRatesQueryHandler(IExchangeRateService exchangeRateService, ICacheService cache)
    : IRequestHandler<GetAllRatesQuery, ApiResponse<ExchangeRateDto>>
{
    public async Task<ApiResponse<ExchangeRateDto>> Handle(GetAllRatesQuery request, CancellationToken ct)
    {
        var cacheKey = $"rates:{request.BaseCurrency.ToUpper()}";
        var cached = cache.Get<ExchangeRateDto>(cacheKey);

        if (cached is not null)
        {
            return ApiResponse<ExchangeRateDto>.Ok(cached);
        }

        var rates = await exchangeRateService.GetAllRatesAsync(request.BaseCurrency.ToUpper());
        var dto = new ExchangeRateDto(request.BaseCurrency.ToUpper(), rates);

        cache.Set(cacheKey, dto, TimeSpan.FromHours(1));

        return ApiResponse<ExchangeRateDto>.Ok(dto);
    }
}