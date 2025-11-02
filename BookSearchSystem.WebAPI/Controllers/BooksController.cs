using Microsoft.AspNetCore.Mvc;
using BookSearchSystem.WebAPI.DTOs;
using BookSearchSystem.WebAPI.Services;

namespace BookSearchSystem.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly ILogger<BooksController> _logger;
    private readonly IDatabaseService _databaseService;

    public BooksController(ILogger<BooksController> logger, IDatabaseService databaseService)
    {
        _logger = logger;
        _databaseService = databaseService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<BookSearchResponse>> Search(
        [FromQuery] string? title,
        [FromQuery] string? author,
        [FromQuery] string? category,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        try
        {
            var keyword = title ?? author ?? category ?? "";
            var books = await _databaseService.SearchBooksAsync(keyword);

            return Ok(new BookSearchResponse
            {
                Success = true,
                Total = books.Count,
                Page = page,
                PageSize = pageSize,
                Books = books.Skip((page - 1) * pageSize).Take(pageSize).ToList()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索图书失败");
            return StatusCode(500, new BookSearchResponse { Success = false });
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(int id)
    {
        try
        {
            var book = await _databaseService.GetBookByIdAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取图书详情失败: {BookId}", id);
            return StatusCode(500);
        }
    }

    [HttpGet("recommend")]
    public async Task<ActionResult<List<BookDto>>> GetRecommended([FromQuery] int count = 10)
    {
        try
        {
            var books = await _databaseService.GetRecommendedBooksAsync(count);
            return Ok(books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取推荐图书失败");
            return StatusCode(500, new List<BookDto>());
        }
    }
}

public class BookSearchResponse
{
    public bool Success { get; set; }
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public List<BookDto> Books { get; set; } = new();
}
