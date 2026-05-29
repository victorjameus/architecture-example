using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.ConversionExport.DTOs;

namespace CompanyName.ProjectName.Application.Features.ConversionExport.Queries;

public record GetAllExportsQuery : IRequest<ApiResponse<IEnumerable<ConversionExportDto>>>;

public sealed class GetAllExportsQueryHandler(IUnitOfWork uow) : IRequestHandler<GetAllExportsQuery, ApiResponse<IEnumerable<ConversionExportDto>>>
{
    public async Task<ApiResponse<IEnumerable<ConversionExportDto>>> Handle(GetAllExportsQuery request, CancellationToken ct)
    {
        var exports = await uow.Repository<Domain.Entities.ConversionExport>().GetAllAsync();
        var dto = exports.Select(e => new ConversionExportDto(e.Id, e.FileName, e.BlobUrl, e.DateFrom, e.DateTo, e.TotalRecords, e.CreatedAt));

        return ApiResponse<IEnumerable<ConversionExportDto>>.Ok(dto);
    }
}