namespace CompanyName.ProjectName.Application.Common.Models;

public class PagedResponse<T>
{
    public List<T> Items { get; set; } = [];
    public int TotalRecords { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);
    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}