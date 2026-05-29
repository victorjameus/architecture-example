namespace CompanyName.ProjectName.Application.Features.Export.DTOs;

public record CreateConversionExportDto
(
    DateTime? DateFrom,
    DateTime? DateTo
);