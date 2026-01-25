namespace Catalog.Api.Dtos;

public class ApiErrorResponse
{
    public ApiErrorResponse(string title, string detail, int status)
    {
        Title = title;
        Detail = detail;
        Status = status;
    }

    public string Title { get; }
    public string Detail { get; }
    public int Status { get; }
}
