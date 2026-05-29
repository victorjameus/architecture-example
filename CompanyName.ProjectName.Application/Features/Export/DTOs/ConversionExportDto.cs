namespace CompanyName.ProjectName.Application.Features.Export.DTOs;

public record ConversionExportDto
(
    int Id,
    string FileName,
    string BlobUrl,
    DateTime? DateFrom,
    DateTime? DateTo,
    int TotalRecords,
    DateTime CreatedAt
);