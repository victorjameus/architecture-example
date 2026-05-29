using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ConversionHistory.DTOs;

namespace CompanyName.ProjectName.Application.Features.ConversionHistory.Queries;

public record GetAllConversionsQuery(int Page, int PageSize) : IRequest<ApiResponse<PagedResponse<ConversionHistoryDto>>>;

public sealed class GetAllConversionsQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetAllConversionsQuery, ApiResponse<PagedResponse<ConversionHistoryDto>>>
{
    public async Task<ApiResponse<PagedResponse<ConversionHistoryDto>>> Handle(GetAllConversionsQuery request, CancellationToken ct)
    {
        var conversions = await uow.Repository<Domain.Entities.ConversionHistory>().GetAllAsync();
        var currencies = await uow.Repository<Domain.Entities.Currency>().GetAllAsync();
        var currencyMap = currencies.ToDictionary(c => c.Id, c => c.Code);

        var dtos = conversions.Select(c => new ConversionHistoryDto
        (
            c.Id,
            currencyMap.GetValueOrDefault(c.FromCurrencyId, string.Empty),
            currencyMap.GetValueOrDefault(c.ToCurrencyId, string.Empty),
            c.Amount,
            c.ConvertedAmount,
            c.ExchangeRate,
            c.ConvertedAt
        ));

        var total = dtos.Count();
        var items = dtos.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

        var paged = new PagedResponse<ConversionHistoryDto>
        {
            Items = items,
            TotalRecords = total,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ApiResponse<PagedResponse<ConversionHistoryDto>>.Ok(paged);
    }
}