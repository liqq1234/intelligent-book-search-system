using BookSearchSystem.Blazor.Models;
using BookSearchSystem.Blazor.DTOs;

namespace BookSearchSystem.Blazor.Services;

public interface IBookService
{
    Task<BookSearchResponse> SearchBooksAsync(SearchRequest request);
    Task<Book?> GetBookByIdAsync(int bookId);
    Task<List<Book>> GetRecommendedBooksAsync(int count = 10);
}
