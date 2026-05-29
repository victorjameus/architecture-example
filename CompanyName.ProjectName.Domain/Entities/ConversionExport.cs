namespace CompanyName.ProjectName.Domain.Entities;

public class ConversionExport
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string BlobUrl { get; set; } = string.Empty;
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int TotalRecords { get; set; }
    public DateTime CreatedAt { get; set; }
}