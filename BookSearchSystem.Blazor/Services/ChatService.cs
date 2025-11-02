using System.Net.Http.Json;
using BookSearchSystem.Blazor.DTOs;

namespace BookSearchSystem.Blazor.Services;

public class ChatService : IChatService
{
    private readonly HttpClient _httpClient;

    public ChatService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ResponseData> SendMessageAsync(string message, string? conversationId = null)
    {
        var request = new ChatRequest
        {
            Message = message,
            ConversationId = conversationId ?? Guid.NewGuid().ToString(),
            UserId = "user-001"
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/chat/message", request);
            response.EnsureSuccessStatusCode();

            var chatResponse = await response.Content.ReadFromJsonAsync<ChatResponse>();
            
            if (chatResponse?.Success == true && chatResponse.Response != null)
            {
                return chatResponse.Response;
            }

            return new ResponseData
            {
                Content = "抱歉,无法获取回复。",
                Timestamp = DateTime.Now
            };
        }
        catch (Exception ex)
        {
            return new ResponseData
            {
                Content = $"发送消息失败: {ex.Message}",
                Timestamp = DateTime.Now
            };
        }
    }
}
