Pagination, Filtering, and Sorting API (ASP.NET Core)

Overview
This project delivers a predictable, enterprise-style product catalog API. The main goal is to avoid unbounded list endpoints by enforcing pagination, filters, sorting, and explicit limits through a clear query contract.

What this project demonstrates
- Query contract via DTOs instead of ad-hoc query strings
- Safe defaults and explicit limits to protect the API
- Incremental query building (filters -> sort -> count -> paging)
- Paged responses with metadata for client-side navigation
- Consistent error responses for invalid requests

Features
- Pagination with defaults and limits
- Filters: name (contains), minPrice, maxPrice
- Sorting by name, price, or createdAt with asc/desc
- CRUD endpoints for product management
- Swagger UI for visual exploration
- Seed data on first run for quick testing

Tech Stack
- .NET 10 (ASP.NET Core Web API)
- EF Core 10
- SQLite
- Swagger (Swashbuckle)

API Endpoints
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

Response: Paged Result
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

Expected HTTP Status Codes
- 200 OK (successful GET)
- 201 Created (successful POST)
- 204 No Content (successful DELETE)
- 400 Bad Request (validation errors)
- 404 Not Found (resource does not exist)

Common Errors
- Invalid sortBy value
  {
    "title": "Invalid sortBy value.",
    "detail": "Allowed values: name, price, createdAt.",
    "status": 400
  }
- Invalid sortDir value
  {
    "title": "Invalid sortDir value.",
    "detail": "Allowed values: asc, desc.",
    "status": 400
  }
- Invalid price range
  {
    "title": "Invalid price range.",
    "detail": "minPrice cannot be greater than maxPrice.",
    "status": 400
  }
- Product not found
  {
    "title": "Product not found.",
    "detail": "The requested product does not exist.",
    "status": 404
  }

Validation Rules
- page >= 1
- pageSize in range 1..100
- sortBy must be one of: name, price, createdAt
- sortDir must be asc or desc
- minPrice and maxPrice cannot be negative
- minPrice cannot be greater than maxPrice
- name filter max length: 200
- product name required, max length: 200
- product description max length: 1000
- product price must be greater than zero

How to Run
1) dotnet run --project src/Catalog.Api
2) Swagger UI: http://localhost:5107/swagger

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

Data & Persistence
- SQLite database stored in the app working directory
- Database is created automatically at startup
- Seed data is inserted on first run

Design Decisions
- Max pageSize = 100 to prevent abuse
- Default sort = createdAt desc for stable ordering
- Query params encapsulated in ProductQueryParams for a predictable contract
