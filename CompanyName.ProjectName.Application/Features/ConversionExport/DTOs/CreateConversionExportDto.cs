namespace CompanyName.ProjectName.Application.Features.ConversionExport.DTOs;

public record CreateConversionExportDto
(
    DateTime? DateFrom,
    DateTime? DateTo
);