using CompanyName.ProjectName.Application.Common.Interfaces;
using CompanyName.ProjectName.Application.Common.Models;
using CompanyName.ProjectName.Application.Features.Export.DTOs;
using CompanyName.ProjectName.Domain.Entities;

namespace CompanyName.ProjectName.Application.Features.Export.Queries;

public record GetAllExportsQuery : IRequest<ApiResponse<IEnumerable<ConversionExportDto>>>;

public sealed class GetAllExportsQueryHandler(IUnitOfWork uow) : IRequestHandler<GetAllExportsQuery, ApiResponse<IEnumerable<ConversionExportDto>>>
{
    public async Task<ApiResponse<IEnumerable<ConversionExportDto>>> Handle(GetAllExportsQuery request, CancellationToken ct)
    {
        var exports = await uow.Repository<ConversionExport>().GetAllAsync();

        var dto = exports.Select
        (
            e => new ConversionExportDto
            (
                e.Id,
                e.FileName,
                e.BlobUrl,
                e.DateFrom,
                e.DateTo,
                e.TotalRecords,
                e.CreatedAt
            )
        );

        return ApiResponse<IEnumerable<ConversionExportDto>>.Ok(dto);
    }
}