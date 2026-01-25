namespace Catalog.Api.Dtos;

public class PagedResult<T>
{
    public PagedResult(
        IReadOnlyList<T> items,
        int page,
        int pageSize,
        int totalItems,
        int totalPages,
        string sortBy,
        string sortDir)
    {
        Items = items;
        Page = page;
        PageSize = pageSize;
        TotalItems = totalItems;
        TotalPages = totalPages;
        SortBy = sortBy;
        SortDir = sortDir;
    }

    public IReadOnlyList<T> Items { get; }
    public int Page { get; }
    public int PageSize { get; }
    public int TotalItems { get; }
    public int TotalPages { get; }
    public string SortBy { get; }
    public string SortDir { get; }
}
