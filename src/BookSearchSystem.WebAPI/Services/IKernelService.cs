using BookSearchSystem.WebAPI.DTOs;

namespace BookSearchSystem.WebAPI.Services;

public interface IKernelService
{
    Task<(string Content, List<BookDto>? Books)> ChatAsync(string message);
}
