using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ConversionExport.DTOs;

namespace CompanyName.ProjectName.Application.Features.ConversionExport.Commands;

public record CreateConversionExportCommand(DateTime? DateFrom, DateTime? DateTo) : IRequest<ApiResponse<ConversionExportDto>>;

public sealed class CreateConversionExportCommandHandler(IUnitOfWork uow, IBlobStorageService blobStorageService) : IRequestHandler<CreateConversionExportCommand, ApiResponse<ConversionExportDto>>
{
    public async Task<ApiResponse<ConversionExportDto>> Handle(CreateConversionExportCommand request, CancellationToken ct)
    {
        var conversions = await uow.Repository<Domain.Entities.ConversionHistory>().GetAllAsync();

        if (request.DateFrom.HasValue)
        {
            conversions = conversions.Where(c => c.ConvertedAt >= request.DateFrom.Value);
        }

        if (request.DateTo.HasValue)
        {
            conversions = conversions.Where(c => c.ConvertedAt <= request.DateTo.Value);
        }

        var records = conversions.ToList();
        var csv = GenerateCsv(records);
        var fileName = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

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

        entity.Id = id;
        var dto = new ConversionExportDto(entity.Id, entity.FileName, entity.BlobUrl, entity.DateFrom, entity.DateTo, entity.TotalRecords, entity.CreatedAt);

        return ApiResponse<ConversionExportDto>.Ok(dto, "Exportación generada exitosamente.");
    }

    private static string GenerateCsv(List<Domain.Entities.ConversionHistory> records)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Id,FromCurrencyId,ToCurrencyId,Amount,ConvertedAmount,ExchangeRate,ConvertedAt");

        foreach (var r in records)
        {
            sb.AppendLine($"{r.Id},{r.FromCurrencyId},{r.ToCurrencyId},{r.Amount},{r.ConvertedAmount},{r.ExchangeRate},{r.ConvertedAt:yyyy-MM-dd HH:mm:ss}");
        }

        return sb.ToString();
    }
}