using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Currency.DTOs;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.Currency.Queries;

public record GetCurrencyByIdQuery(int Id) : IRequest<ApiResponse<CurrencyDto>>;

public sealed class GetCurrencyByIdQueryHandler(IUnitOfWork uow) : IRequestHandler<GetCurrencyByIdQuery, ApiResponse<CurrencyDto>>
{
    public async Task<ApiResponse<CurrencyDto>> Handle(GetCurrencyByIdQuery request, CancellationToken ct)
    {
        var currency = await uow.Repository<Domain.Entities.Currency>().GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Moneda con id {request.Id} no encontrada.");

        var dto = new CurrencyDto
        (
            currency.Id,
            currency.Code,
            currency.Name,
            currency.Symbol,
            currency.IsActive,
            currency.CreatedAt,
            currency.UpdatedAt
        );

        return ApiResponse<CurrencyDto>.Ok(dto);
    }
}