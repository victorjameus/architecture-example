using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.ConversionHistory.Commands;

public record DeleteConversionCommand(int Id) : IRequest<ApiResponse<bool>>;

public sealed class DeleteConversionCommandHandler(IUnitOfWork uow) : IRequestHandler<DeleteConversionCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteConversionCommand request, CancellationToken ct)
    {
        var conversion = await uow.Repository<Domain.Entities.ConversionHistory>().GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Conversión con id {request.Id} no encontrada.");

        await uow.Repository<Domain.Entities.ConversionHistory>().DeleteAsync(conversion.Id);
        await uow.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Conversión eliminada exitosamente.");
    }
}