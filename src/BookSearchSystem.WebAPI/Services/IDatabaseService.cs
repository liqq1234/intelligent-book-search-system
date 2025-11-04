using BookSearchSystem.WebAPI.DTOs;

namespace BookSearchSystem.WebAPI.Services;

public interface IDatabaseService
{
    Task<List<BookDto>> SearchBooksAsync(string keyword);
    Task<BookDto?> GetBookByIdAsync(int bookId);
    Task<List<BookDto>> GetRecommendedBooksAsync(int count = 10);
}
