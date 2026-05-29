using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ConversionExport.DTOs;
using CompanyName.ProjectName.Domain.Enums;

namespace CompanyName.ProjectName.Application.Features.ConversionExport.Commands;

public record CreateConversionExportCommand(DateTime? DateFrom, DateTime? DateTo) : IRequest<ApiResponse<ConversionExportDto>>;

public sealed class CreateConversionExportCommandHandler(IUnitOfWork uow, IBlobStorageService blobStorageService, IInsightService insightService)
    : IRequestHandler<CreateConversionExportCommand, ApiResponse<ConversionExportDto>>
{
    public async Task<ApiResponse<ConversionExportDto>> Handle(CreateConversionExportCommand request, CancellationToken ct)
    {
        var conversions = await uow.Repository<Domain.Entities.ConversionHistory>().GetAllAsync();
        var currencies = await uow.Repository<Domain.Entities.Currency>().GetAllAsync();
        var currencyMap = currencies.ToDictionary(c => c.Id, c => c.Code);

        if (request.DateFrom.HasValue)
        {
            conversions = conversions.Where(c => c.ConvertedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            conversions = conversions.Where(c => c.ConvertedAt <= request.DateTo.Value);
        }

        var records = conversions.ToList();

        if (records.Count == 0)
        {
            insightService.TrackTrace("Export solicitado sin registros para el rango de fechas indicado.", InsightLevel.Warning);
        }

        var csv = GenerateCsv(records, currencyMap);
        var fileName = $"export_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(csv));
        var blobUrl = await blobStorageService.UploadAsync(stream, fileName, "text/csv");

        var entity = new Domain.Entities.ConversionExport
        {
            FileName = fileName,
            BlobUrl = blobUrl,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            TotalRecords = records.Count
        };

        var id = await uow.Repository<Domain.Entities.ConversionExport>().AddAsync(entity);
        await uow.SaveChangesAsync();

        var created = await uow.Repository<Domain.Entities.ConversionExport>().GetByIdAsync(id);
        var dto = new ConversionExportDto(created!.Id, created.FileName, created.BlobUrl, created.DateFrom, created.DateTo, created.TotalRecords, created.CreatedAt);

        insightService.TrackEvent("ExportGenerado", new Dictionary<string, string>
        {
            { "FileName", fileName },
            { "TotalRecords", records.Count.ToString() },
            { "DateFrom", request.DateFrom?.ToString() ?? "null" },
            { "DateTo", request.DateTo?.ToString() ?? "null" }
        });

        insightService.TrackTrace
        (
            $"Export generado: {fileName} con {records.Count} registros.",
            InsightLevel.Information
        );

        return ApiResponse<ConversionExportDto>.Ok(dto, "Exportación generada exitosamente.");
    }

    private static string GenerateCsv(List<Domain.Entities.ConversionHistory> records, Dictionary<int, string> currencyMap)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,FromCurrency,ToCurrency,Amount,ConvertedAmount,ExchangeRate,ConvertedAt");

        foreach (var r in records)
        {
            sb.AppendLine
            (
                $"{r.Id}," +
                $"{currencyMap.GetValueOrDefault(r.FromCurrencyId, string.Empty)}," +
                $"{currencyMap.GetValueOrDefault(r.ToCurrencyId, string.Empty)}," +
                $"{r.Amount}," +
                $"{r.ConvertedAmount}," +
                $"{r.ExchangeRate}," +
                $"{r.ConvertedAt:yyyy-MM-ddTHH:mm:ss}"
            );
        }

        return sb.ToString();
    }
}