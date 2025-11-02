using BookSearchSystem.Blazor.Models;

namespace BookSearchSystem.Blazor.DTOs;

public class ChatResponse
{
    public bool Success { get; set; }
    public string? ConversationId { get; set; }
    public ResponseData? Response { get; set; }
}

public class ResponseData
{
    public string Content { get; set; } = string.Empty;
    public List<Book>? Books { get; set; }
    public DateTime Timestamp { get; set; }
}
