namespace CompanyName.ProjectName.Application.Features.ConversionExport.DTOs;

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