using BookSearchSystem.Blazor.Models;

namespace BookSearchSystem.Blazor.DTOs;

public class BookSearchResponse
{
    public bool Success { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<Book> Books { get; set; } = new();
}
