using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Currency.DTOs;

namespace CompanyName.ProjectName.Application.Features.Currency.Queries;

public record GetAllCurrenciesQuery : IRequest<ApiResponse<IEnumerable<CurrencyDto>>>;

public sealed class GetAllCurrenciesQueryHandler(IUnitOfWork uow) : IRequestHandler<GetAllCurrenciesQuery, ApiResponse<IEnumerable<CurrencyDto>>>
{
    public async Task<ApiResponse<IEnumerable<CurrencyDto>>> Handle(GetAllCurrenciesQuery request, CancellationToken ct)
    {
        var currencies = await uow.Repository<Domain.Entities.Currency>().GetAllAsync();
        var dto = currencies.Select(c => new CurrencyDto(c.Id, c.Code, c.Name, c.Symbol, c.IsActive, c.CreatedAt, c.UpdatedAt));

        return ApiResponse<IEnumerable<CurrencyDto>>.Ok(dto);
    }
}