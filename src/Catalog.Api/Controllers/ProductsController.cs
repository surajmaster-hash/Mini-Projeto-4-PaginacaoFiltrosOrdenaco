using Catalog.Api.Dtos;
using Catalog.Api.Entities;
using Catalog.Api.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Api.Controllers;

[ApiController]
[Route("products")]
public class ProductsController : ControllerBase
{
    private static readonly string[] AllowedSortBy = { "name", "price", "createdAt" };
    private static readonly string[] AllowedSortDir = { "asc", "desc" };
    private const int MaxNameLength = 200;
    private const int MaxDescriptionLength = 1000;
    private readonly CatalogDbContext _dbContext;

    public ProductsController(CatalogDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpGet]
    public async Task<ActionResult<PagedResult<ProductResponse>>> GetProducts([FromQuery] ProductQueryParams queryParams)
    {
        var page = Math.Max(1, queryParams.Page);
        var pageSize = Math.Clamp(queryParams.PageSize, 1, ProductQueryParams.MaxPageSize);
        var sortBy = string.IsNullOrWhiteSpace(queryParams.SortBy) ? "createdAt" : queryParams.SortBy.Trim();
        var sortDir = string.IsNullOrWhiteSpace(queryParams.SortDir) ? "desc" : queryParams.SortDir.Trim();

        if (!AllowedSortBy.Contains(sortBy, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiErrorResponse(
                "Invalid sortBy value.",
                "Allowed values: name, price, createdAt.",
                StatusCodes.Status400BadRequest));
        }

        if (!AllowedSortDir.Contains(sortDir, StringComparer.OrdinalIgnoreCase))
        {
            return BadRequest(new ApiErrorResponse(
                "Invalid sortDir value.",
                "Allowed values: asc, desc.",
                StatusCodes.Status400BadRequest));
        }

        if (queryParams.MinPrice.HasValue && queryParams.MaxPrice.HasValue &&
            queryParams.MinPrice.Value > queryParams.MaxPrice.Value)
        {
            return BadRequest(new ApiErrorResponse(
                "Invalid price range.",
                "minPrice cannot be greater than maxPrice.",
                StatusCodes.Status400BadRequest));
        }

        if (queryParams.MinPrice.HasValue && queryParams.MinPrice.Value < 0)
        {
            return BadRequest(new ApiErrorResponse(
                "Invalid minPrice value.",
                "minPrice cannot be negative.",
                StatusCodes.Status400BadRequest));
        }

        if (queryParams.MaxPrice.HasValue && queryParams.MaxPrice.Value < 0)
        {
            return BadRequest(new ApiErrorResponse(
                "Invalid maxPrice value.",
                "maxPrice cannot be negative.",
                StatusCodes.Status400BadRequest));
        }

        if (!string.IsNullOrWhiteSpace(queryParams.Name) &&
            queryParams.Name.Trim().Length > MaxNameLength)
        {
            return BadRequest(new ApiErrorResponse(
                "Invalid name filter.",
                $"name cannot be longer than {MaxNameLength} characters.",
                StatusCodes.Status400BadRequest));
        }

        var productsQuery = _dbContext.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(queryParams.Name))
        {
            var pattern = $"%{queryParams.Name.Trim()}%";
            productsQuery = productsQuery.Where(product => EF.Functions.Like(product.Name, pattern));
        }

        if (queryParams.MinPrice.HasValue)
        {
            productsQuery = productsQuery.Where(product => product.Price >= queryParams.MinPrice.Value);
        }

        if (queryParams.MaxPrice.HasValue)
        {
            productsQuery = productsQuery.Where(product => product.Price <= queryParams.MaxPrice.Value);
        }

        productsQuery = ApplySorting(productsQuery, sortBy, sortDir);

        var totalItems = await productsQuery.CountAsync();
        var totalPages = totalItems == 0
            ? 0
            : (int)Math.Ceiling(totalItems / (double)pageSize);

        var items = await productsQuery
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(product => new ProductResponse
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CreatedAt = product.CreatedAt
            })
            .ToListAsync();

        var result = new PagedResult<ProductResponse>(
            items,
            page,
            pageSize,
            totalItems,
            totalPages,
            sortBy.ToLowerInvariant(),
            sortDir.ToLowerInvariant());

        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> GetProductById(Guid id)
    {
        var product = await _dbContext.Products.AsNoTracking()
            .FirstOrDefaultAsync(item => item.Id == id);

        if (product is null)
        {
            return NotFound(new ApiErrorResponse(
                "Product not found.",
                "The requested product does not exist.",
                StatusCodes.Status404NotFound));
        }

        return Ok(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CreatedAt = product.CreatedAt
        });
    }

    [HttpPost]
    public async Task<ActionResult<ProductResponse>> CreateProduct([FromBody] CreateProductRequest request)
    {
        var createValidation = ValidateProductRequest(request.Name, request.Description, request.Price);
        if (createValidation is not null)
        {
            return BadRequest(createValidation);
        }

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim() ?? string.Empty,
            Price = request.Price,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Products.Add(product);
        await _dbContext.SaveChangesAsync();

        var response = new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CreatedAt = product.CreatedAt
        };

        return CreatedAtAction(nameof(GetProductById), new { id = response.Id }, response);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProductResponse>> UpdateProduct(Guid id, [FromBody] UpdateProductRequest request)
    {
        var updateValidation = ValidateProductRequest(request.Name, request.Description, request.Price);
        if (updateValidation is not null)
        {
            return BadRequest(updateValidation);
        }

        var product = await _dbContext.Products.FirstOrDefaultAsync(item => item.Id == id);
        if (product is null)
        {
            return NotFound(new ApiErrorResponse(
                "Product not found.",
                "The requested product does not exist.",
                StatusCodes.Status404NotFound));
        }

        product.Name = request.Name.Trim();
        product.Description = request.Description?.Trim() ?? string.Empty;
        product.Price = request.Price;

        await _dbContext.SaveChangesAsync();

        return Ok(new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            CreatedAt = product.CreatedAt
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult> DeleteProduct(Guid id)
    {
        var product = await _dbContext.Products.FirstOrDefaultAsync(item => item.Id == id);
        if (product is null)
        {
            return NotFound(new ApiErrorResponse(
                "Product not found.",
                "The requested product does not exist.",
                StatusCodes.Status404NotFound));
        }

        _dbContext.Products.Remove(product);
        await _dbContext.SaveChangesAsync();

        return NoContent();
    }

    private static ApiErrorResponse? ValidateProductRequest(string? name, string? description, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return new ApiErrorResponse(
                "Invalid product name.",
                "Name is required.",
                StatusCodes.Status400BadRequest);
        }

        if (name.Trim().Length > MaxNameLength)
        {
            return new ApiErrorResponse(
                "Invalid product name.",
                $"Name cannot be longer than {MaxNameLength} characters.",
                StatusCodes.Status400BadRequest);
        }

        if (!string.IsNullOrWhiteSpace(description) &&
            description.Trim().Length > MaxDescriptionLength)
        {
            return new ApiErrorResponse(
                "Invalid product description.",
                $"Description cannot be longer than {MaxDescriptionLength} characters.",
                StatusCodes.Status400BadRequest);
        }

        if (price <= 0)
        {
            return new ApiErrorResponse(
                "Invalid product price.",
                "Price must be greater than zero.",
                StatusCodes.Status400BadRequest);
        }

        return null;
    }

    private static IQueryable<Product> ApplySorting(
        IQueryable<Product> query,
        string sortBy,
        string sortDir)
    {
        var normalizedSortBy = sortBy.ToLowerInvariant();
        var normalizedSortDir = sortDir.ToLowerInvariant();

        return normalizedSortBy switch
        {
            "name" => normalizedSortDir == "asc"
                ? query.OrderBy(product => product.Name)
                : query.OrderByDescending(product => product.Name),
            "price" => normalizedSortDir == "asc"
                ? query.OrderBy(product => product.Price)
                : query.OrderByDescending(product => product.Price),
            _ => normalizedSortDir == "asc"
                ? query.OrderBy(product => product.CreatedAt)
                : query.OrderByDescending(product => product.CreatedAt)
        };
    }
}
