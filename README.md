Pagination, Filtering, and Sorting API (ASP.NET Core)

Overview
This project demonstrates a predictable, enterprise-style list endpoint with pagination, filters, and sorting.
It focuses on a clear query contract, safe defaults, explicit limits, and consistent error responses.

Features
- Paged listing with metadata (page, pageSize, totalItems, totalPages)
- Filters: name (contains), minPrice, maxPrice
- Sorting by name, price, or createdAt with asc/desc direction
- Safe defaults and limits to avoid unbounded queries
- Consistent error contract for invalid requests
- Swagger UI for visual API exploration

Tech Stack
- .NET 10 (ASP.NET Core Web API)
- EF Core 10
- SQLite
- Swagger (Swashbuckle)

Endpoints
- GET /products
- GET /products/{id}
- POST /products
- PUT /products/{id}
- DELETE /products/{id}

Query Parameters (GET /products)
- page (default: 1)
- pageSize (default: 20, max: 100)
- sortBy (name, price, createdAt)
- sortDir (asc, desc)
- name (contains)
- minPrice
- maxPrice

Paged Response Shape
{
  "items": [ ... ],
  "page": 1,
  "pageSize": 20,
  "totalItems": 153,
  "totalPages": 8,
  "sortBy": "price",
  "sortDir": "desc"
}

Error Contract (HTTP 400 / 404)
{
  "title": "Invalid sortBy value.",
  "detail": "Allowed values: name, price, createdAt.",
  "status": 400
}

How to Run
1) dotnet run --project src/Catalog.Api
2) Open Swagger UI: http://localhost:5107/swagger

Examples
- /products?page=2&pageSize=20
- /products?minPrice=10&maxPrice=50
- /products?sortBy=price&sortDir=desc
- /products?name=milk

Create Product
POST /products
{
  "name": "Sparkling Water",
  "description": "Lime sparkling water.",
  "price": 2.75
}

Update Product
PUT /products/{id}
{
  "name": "Sparkling Water - Lime",
  "description": "Lime sparkling water, 350ml.",
  "price": 2.95
}

Design Decisions
- Max pageSize = 100 to prevent abuse
- Default sort = createdAt desc for stable ordering
- Query params encapsulated in ProductQueryParams for a predictable contract
