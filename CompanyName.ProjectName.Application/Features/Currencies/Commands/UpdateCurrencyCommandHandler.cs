using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Currencies.DTOs;
using CompanyName.ProjectName.Domain.Entities;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.Currencies.Commands;

public record UpdateCurrencyCommand(int Id, string Code, string Name, string Symbol) : IRequest<ApiResponse<CurrencyDto>>;

public sealed class UpdateCurrencyCommandHandler(IUnitOfWork uow, ICacheService cache) : IRequestHandler<UpdateCurrencyCommand, ApiResponse<CurrencyDto>>
{
    public async Task<ApiResponse<CurrencyDto>> Handle(UpdateCurrencyCommand request, CancellationToken ct)
    {
        var currency = await uow.Repository<Currency>().GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Moneda con id {request.Id} no encontrada.");

        var currencies = await uow.Repository<Currency>().GetAllAsync();
        var exists = currencies.Any(c => c.Code == request.Code.ToUpper() && c.Id != request.Id);

        if (exists)
        {
            throw new ConflictException($"Ya existe una moneda con el código {request.Code.ToUpper()}.");
        }

        currency.Code = request.Code.ToUpper();
        currency.Name = request.Name;
        currency.Symbol = request.Symbol;

        await uow.Repository<Currency>().UpdateAsync(currency);
        await uow.SaveChangesAsync();

        var updated = await uow.Repository<Currency>().GetByIdAsync(request.Id);

        var dto = new CurrencyDto
        (
            updated!.Id,
            updated.Code,
            updated.Name,
            updated.Symbol,
            updated.IsActive,
            updated.CreatedAt,
            updated.UpdatedAt
        );

        cache.Remove("currencies:all");

        return ApiResponse<CurrencyDto>.Ok(dto, "Moneda actualizada exitosamente.");
    }
}