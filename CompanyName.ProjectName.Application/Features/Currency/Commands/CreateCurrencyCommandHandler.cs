using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Currency.DTOs;
using CompanyName.ProjectName.Domain.Enums;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.Currency.Commands;

public record CreateCurrencyCommand(string Code, string Name, string Symbol) : IRequest<ApiResponse<CurrencyDto>>;

public sealed class CreateCurrencyCommandHandler(IUnitOfWork uow, IInsightService insightService) : IRequestHandler<CreateCurrencyCommand, ApiResponse<CurrencyDto>>
{
    public async Task<ApiResponse<CurrencyDto>> Handle(CreateCurrencyCommand request, CancellationToken ct)
    {
        var currencies = await uow.Repository<Domain.Entities.Currency>().GetAllAsync();
        var exists = currencies.Any(c => c.Code == request.Code.ToUpper());

        if (exists)
        {
            throw new ConflictException($"Ya existe una moneda con el código {request.Code.ToUpper()}.");
        }

        var entity = new Domain.Entities.Currency
        {
            Code = request.Code.ToUpper(),
            Name = request.Name,
            Symbol = request.Symbol,
            IsActive = true
        };

        var id = await uow.Repository<Domain.Entities.Currency>().AddAsync(entity);
        await uow.SaveChangesAsync();

        var created = await uow.Repository<Domain.Entities.Currency>().GetByIdAsync(id);
        var dto = new CurrencyDto(created!.Id, created.Code, created.Name, created.Symbol, created.IsActive, created.CreatedAt, created.UpdatedAt);

        insightService.TrackEvent("MonedaCreada", new Dictionary<string, string>
        {
            { "Code", created.Code },
            { "Name", created.Name }
        });

        insightService.TrackTrace($"Moneda {created.Code} creada con id {created.Id}.", InsightLevel.Information);

        return ApiResponse<CurrencyDto>.Ok(dto, "Moneda creada exitosamente.");
    }
}