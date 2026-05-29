using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ConversionHistory.DTOs;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.ConversionHistory.Queries;

public record GetConversionByIdQuery(int Id) : IRequest<ApiResponse<ConversionHistoryDto>>;

public sealed class GetConversionByIdQueryHandler(IUnitOfWork uow) : IRequestHandler<GetConversionByIdQuery, ApiResponse<ConversionHistoryDto>>
{
    public async Task<ApiResponse<ConversionHistoryDto>> Handle(GetConversionByIdQuery request, CancellationToken ct)
    {
        var conversion = await uow.Repository<Domain.Entities.ConversionHistory>().GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Conversión con id {request.Id} no encontrada.");

        var dto = new ConversionHistoryDto(
            conversion.Id,
            conversion.FromCurrency?.Code ?? string.Empty,
            conversion.ToCurrency?.Code ?? string.Empty,
            conversion.Amount,
            conversion.ConvertedAmount,
            conversion.ExchangeRate,
            conversion.ConvertedAt
        );

        return ApiResponse<ConversionHistoryDto>.Ok(dto);
    }
}