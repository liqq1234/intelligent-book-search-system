namespace BookSearchSystem.Blazor.Models;

public class ChatMessageModel
{
    public string Role { get; set; } = string.Empty; // "user" or "assistant"
    public string Content { get; set; } = string.Empty;
    public List<Book>? Books { get; set; }
    public DateTime Timestamp { get; set; }
}
