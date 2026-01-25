using System.ComponentModel.DataAnnotations;

namespace Catalog.Api.Dtos;

public class ProductQueryParams
{
    public const int MaxPageSize = 100;

    [Range(1, int.MaxValue)]
    public int Page { get; set; } = 1;

    [Range(1, MaxPageSize)]
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string? SortDir { get; set; }

    [MaxLength(200)]
    public string? Name { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
}
