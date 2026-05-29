using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.Currency.Commands;

public record DeleteCurrencyCommand(int Id) : IRequest<ApiResponse<bool>>;

public sealed class DeleteCurrencyCommandHandler(IUnitOfWork uow, ICacheService cache) : IRequestHandler<DeleteCurrencyCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteCurrencyCommand request, CancellationToken ct)
    {
        var currency = await uow.Repository<Domain.Entities.Currency>().GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Moneda con id {request.Id} no encontrada.");

        currency.IsActive = false;
        await uow.Repository<Domain.Entities.Currency>().UpdateAsync(currency);
        await uow.SaveChangesAsync();

        cache.Remove("currencies:all");

        return ApiResponse<bool>.Ok(true, "Moneda desactivada exitosamente.");
    }
}