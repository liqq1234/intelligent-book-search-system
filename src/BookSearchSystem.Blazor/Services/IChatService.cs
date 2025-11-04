using BookSearchSystem.Blazor.DTOs;

namespace BookSearchSystem.Blazor.Services;

public interface IChatService
{
    Task<ResponseData> SendMessageAsync(string message, string? conversationId = null);
}
