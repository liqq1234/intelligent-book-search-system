using Microsoft.AspNetCore.Mvc;
using BookSearchSystem.WebAPI.DTOs;
using BookSearchSystem.WebAPI.Services;

namespace BookSearchSystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly ILogger<ChatController> _logger;
    private readonly IKernelService _kernelService;

    public ChatController(ILogger<ChatController> logger, IKernelService kernelService)
    {
        _logger = logger;
        _kernelService = kernelService;
    }

    [HttpPost("message")]
    public async Task<ActionResult<ChatResponse>> SendMessage([FromBody] ChatRequest request)
    {
        try
        {
            _logger.LogInformation("收到聊天请求: {Message}", request.Message);

            var response = await _kernelService.ChatAsync(request.Message);

            return Ok(new ChatResponse
            {
                Success = true,
                ConversationId = request.ConversationId ?? Guid.NewGuid().ToString(),
                Response = new ResponseData
                {
                    Content = response.Content,
                    Books = response.Books,
                    Timestamp = DateTime.Now
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "处理聊天请求时发生错误");
            return StatusCode(500, new ChatResponse
            {
                Success = false,
                Response = new ResponseData
                {
                    Content = "抱歉，处理您的请求时发生错误。",
                    Timestamp = DateTime.Now
                }
            });
        }
    }
}
