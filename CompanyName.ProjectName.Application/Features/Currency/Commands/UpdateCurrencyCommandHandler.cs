using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Currency.DTOs;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.Currency.Commands;

public record UpdateCurrencyCommand(int Id, string Code, string Name, string Symbol) : IRequest<ApiResponse<CurrencyDto>>;

public sealed class UpdateCurrencyCommandHandler(IUnitOfWork uow) : IRequestHandler<UpdateCurrencyCommand, ApiResponse<CurrencyDto>>
{
    public async Task<ApiResponse<CurrencyDto>> Handle(UpdateCurrencyCommand request, CancellationToken ct)
    {
        var currency = await uow.Repository<Domain.Entities.Currency>().GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Moneda con id {request.Id} no encontrada.");

        var currencies = await uow.Repository<Domain.Entities.Currency>().GetAllAsync();
        var exists = currencies.Any(c => c.Code == request.Code.ToUpper() && c.Id != request.Id);

        if (exists)
        {
            throw new ConflictException($"Ya existe una moneda con el código {request.Code.ToUpper()}.");
        }

        currency.Code = request.Code.ToUpper();
        currency.Name = request.Name;
        currency.Symbol = request.Symbol;

        await uow.Repository<Domain.Entities.Currency>().UpdateAsync(currency);
        await uow.SaveChangesAsync();

        var updated = await uow.Repository<Domain.Entities.Currency>().GetByIdAsync(request.Id);
        var dto = new CurrencyDto(updated!.Id, updated.Code, updated.Name, updated.Symbol, updated.IsActive, updated.CreatedAt, updated.UpdatedAt);

        return ApiResponse<CurrencyDto>.Ok(dto, "Moneda actualizada exitosamente.");
    }
}