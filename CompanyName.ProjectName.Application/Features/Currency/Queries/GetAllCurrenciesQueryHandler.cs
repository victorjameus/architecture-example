using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Currency.DTOs;

namespace CompanyName.ProjectName.Application.Features.Currency.Queries;

public record GetAllCurrenciesQuery : IRequest<ApiResponse<IEnumerable<CurrencyDto>>>;

public sealed class GetAllCurrenciesQueryHandler(IUnitOfWork uow, ICacheService cache) : IRequestHandler<GetAllCurrenciesQuery, ApiResponse<IEnumerable<CurrencyDto>>>
{
    private const string CacheKey = "currencies:all";

    public async Task<ApiResponse<IEnumerable<CurrencyDto>>> Handle(GetAllCurrenciesQuery request, CancellationToken ct)
    {
        var cached = cache.Get<IEnumerable<CurrencyDto>>(CacheKey);

        if (cached is not null)
        {
            return ApiResponse<IEnumerable<CurrencyDto>>.Ok(cached);
        }

        var currencies = await uow.Repository<Domain.Entities.Currency>().GetAllAsync();

        var dto = currencies.Select
        (
            c => new CurrencyDto
            (
                c.Id,
                c.Code,
                c.Name,
                c.Symbol,
                c.IsActive,
                c.CreatedAt,
                c.UpdatedAt
            )
        );

        cache.Set(CacheKey, dto, TimeSpan.FromMinutes(5));

        return ApiResponse<IEnumerable<CurrencyDto>>.Ok(dto);
    }
}