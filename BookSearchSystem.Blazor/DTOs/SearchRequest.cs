namespace BookSearchSystem.Blazor.DTOs;

public class SearchRequest
{
    public string? Title { get; set; }
    public string? Author { get; set; }
    public string? Category { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
