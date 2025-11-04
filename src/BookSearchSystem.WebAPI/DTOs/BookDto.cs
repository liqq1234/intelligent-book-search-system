namespace BookSearchSystem.WebAPI.DTOs;

public class BookDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Publisher { get; set; } = string.Empty;
    public DateTime? PublishDate { get; set; }
    public string ISBN { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public string? Description { get; set; }
    public string? CoverUrl { get; set; }
}
