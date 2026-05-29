using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Conversion.DTOs;
using CompanyName.ProjectName.Domain.Entities;
using CompanyName.ProjectName.Domain.Enums;
using CompanyName.ProjectName.Domain.Exceptions;

namespace CompanyName.ProjectName.Application.Features.Conversion.Commands;

public record CreateConversionCommand(int FromCurrencyId, int ToCurrencyId, decimal Amount) : IRequest<ApiResponse<ConversionHistoryDto>>;

public sealed class CreateConversionCommandHandler(IUnitOfWork uow, IExchangeRateService exchangeRateService, IInsightService insightService)
    : IRequestHandler<CreateConversionCommand, ApiResponse<ConversionHistoryDto>>
{
    public async Task<ApiResponse<ConversionHistoryDto>> Handle(CreateConversionCommand request, CancellationToken ct)
    {
        var fromCurrency = await uow.Repository<Currency>().GetByIdAsync(request.FromCurrencyId)
            ?? throw new NotFoundException($"Moneda origen con id {request.FromCurrencyId} no encontrada.");

        var toCurrency = await uow.Repository<Currency>().GetByIdAsync(request.ToCurrencyId)
            ?? throw new NotFoundException($"Moneda destino con id {request.ToCurrencyId} no encontrada.");

        var rate = await exchangeRateService.GetRateAsync(fromCurrency.Code, toCurrency.Code);
        var convertedAmount = Math.Round(request.Amount * rate, 4);

        var entity = new ConversionHistory
        {
            FromCurrencyId = request.FromCurrencyId,
            ToCurrencyId = request.ToCurrencyId,
            Amount = request.Amount,
            ConvertedAmount = convertedAmount,
            ExchangeRate = rate
        };

        var id = await uow.Repository<ConversionHistory>().AddAsync(entity);
        await uow.SaveChangesAsync();

        var created = await uow.Repository<ConversionHistory>().GetByIdAsync(id);
        var dto = new ConversionHistoryDto
        (
            created!.Id,
            fromCurrency.Code,
            toCurrency.Code,
            created.Amount,
            created.ConvertedAmount,
            created.ExchangeRate,
            created.ConvertedAt
        );

        insightService.TrackEvent("ConversionRealizada", new Dictionary<string, string>
        {
            { "FromCurrency", fromCurrency.Code },
            { "ToCurrency", toCurrency.Code },
            { "Amount", request.Amount.ToString() },
            { "ExchangeRate", rate.ToString() },
            { "ConvertedAmount", convertedAmount.ToString() }
        });

        insightService.TrackTrace
        (
            $"Conversión realizada: {request.Amount} {fromCurrency.Code} = {convertedAmount} {toCurrency.Code}.",
            InsightLevel.Information
        );

        return ApiResponse<ConversionHistoryDto>.Ok(dto, "Conversión realizada exitosamente.");
    }
}