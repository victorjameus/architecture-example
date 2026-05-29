using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Conversion.DTOs;
using CompanyName.ProjectName.Domain.Entities;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.Conversion.Queries;

public record GetConversionByIdQuery(int Id) : IRequest<ApiResponse<ConversionHistoryDto>>;

public sealed class GetConversionByIdQueryHandler(IUnitOfWork uow) : IRequestHandler<GetConversionByIdQuery, ApiResponse<ConversionHistoryDto>>
{
    public async Task<ApiResponse<ConversionHistoryDto>> Handle(GetConversionByIdQuery request, CancellationToken ct)
    {
        var conversion = await uow.Repository<ConversionHistory>().GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Conversión con id {request.Id} no encontrada.");

        var currencies = await uow.Repository<Currency>().GetAllAsync();
        var currencyMap = currencies.ToDictionary(c => c.Id, c => c.Code);

        var dto = new ConversionHistoryDto
        (
            conversion.Id,
            currencyMap.GetValueOrDefault(conversion.FromCurrencyId, string.Empty),
            currencyMap.GetValueOrDefault(conversion.ToCurrencyId, string.Empty),
            conversion.Amount,
            conversion.ConvertedAmount,
            conversion.ExchangeRate,
            conversion.ConvertedAt
        );

        return ApiResponse<ConversionHistoryDto>.Ok(dto);
    }
}