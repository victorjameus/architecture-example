using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ConversionHistory.DTOs;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.ConversionHistory.Commands;

public record CreateConversionCommand(int FromCurrencyId, int ToCurrencyId, decimal Amount) : IRequest<ApiResponse<ConversionHistoryDto>>;

public sealed class CreateConversionCommandHandler(IUnitOfWork uow, IExchangeRateService exchangeRateService) : IRequestHandler<CreateConversionCommand, ApiResponse<ConversionHistoryDto>>
{
    public async Task<ApiResponse<ConversionHistoryDto>> Handle(CreateConversionCommand request, CancellationToken ct)
    {
        var fromCurrency = await uow.Repository<Domain.Entities.Currency>().GetByIdAsync(request.FromCurrencyId)
            ?? throw new NotFoundException($"Moneda origen con id {request.FromCurrencyId} no encontrada.");

        var toCurrency = await uow.Repository<Domain.Entities.Currency>().GetByIdAsync(request.ToCurrencyId)
            ?? throw new NotFoundException($"Moneda destino con id {request.ToCurrencyId} no encontrada.");

        var rate = await exchangeRateService.GetRateAsync(fromCurrency.Code, toCurrency.Code);
        var convertedAmount = Math.Round(request.Amount * rate, 4);

        var entity = new Domain.Entities.ConversionHistory
        {
            FromCurrencyId = request.FromCurrencyId,
            ToCurrencyId = request.ToCurrencyId,
            Amount = request.Amount,
            ConvertedAmount = convertedAmount,
            ExchangeRate = rate
        };

        var id = await uow.Repository<Domain.Entities.ConversionHistory>().AddAsync(entity);
        await uow.SaveChangesAsync();

        var created = await uow.Repository<Domain.Entities.ConversionHistory>().GetByIdAsync(id);
        var dto = new ConversionHistoryDto(
            created!.Id,
            fromCurrency.Code,
            toCurrency.Code,
            created.Amount,
            created.ConvertedAmount,
            created.ExchangeRate,
            created.ConvertedAt
        );

        return ApiResponse<ConversionHistoryDto>.Ok(dto, "Conversión realizada exitosamente.");
    }
}