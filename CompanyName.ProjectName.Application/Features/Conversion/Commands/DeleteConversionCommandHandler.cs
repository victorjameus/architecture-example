using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Domain.Entities;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.Conversion.Commands;

public record DeleteConversionCommand(int Id) : IRequest<ApiResponse<bool>>;

public sealed class DeleteConversionCommandHandler(IUnitOfWork uow) : IRequestHandler<DeleteConversionCommand, ApiResponse<bool>>
{
    public async Task<ApiResponse<bool>> Handle(DeleteConversionCommand request, CancellationToken ct)
    {
        var conversion = await uow.Repository<ConversionHistory>().GetByIdAsync(request.Id)
            ?? throw new NotFoundException($"Conversión con id {request.Id} no encontrada.");

        await uow.Repository<ConversionHistory>().DeleteAsync(conversion.Id);
        await uow.SaveChangesAsync();

        return ApiResponse<bool>.Ok(true, "Conversión eliminada exitosamente.");
    }
}