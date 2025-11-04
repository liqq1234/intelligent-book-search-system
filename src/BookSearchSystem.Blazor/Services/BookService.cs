using System.Net.Http.Json;
using BookSearchSystem.Blazor.Models;
using BookSearchSystem.Blazor.DTOs;

namespace BookSearchSystem.Blazor.Services;

public class BookService : IBookService
{
    private readonly HttpClient _httpClient;

    public BookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BookSearchResponse> SearchBooksAsync(SearchRequest request)
    {
        try
        {
            var queryParams = new List<string>();
            if (!string.IsNullOrEmpty(request.Title))
                queryParams.Add($"title={Uri.EscapeDataString(request.Title)}");
            if (!string.IsNullOrEmpty(request.Author))
                queryParams.Add($"author={Uri.EscapeDataString(request.Author)}");
            if (!string.IsNullOrEmpty(request.Category))
                queryParams.Add($"category={Uri.EscapeDataString(request.Category)}");
            queryParams.Add($"page={request.Page}");
            queryParams.Add($"pageSize={request.PageSize}");

            var queryString = string.Join("&", queryParams);
            var response = await _httpClient.GetFromJsonAsync<BookSearchResponse>($"/api/books/search?{queryString}");

            return response ?? new BookSearchResponse { Success = false };
        }
        catch
        {
            return new BookSearchResponse { Success = false };
        }
    }

    public async Task<Book?> GetBookByIdAsync(int bookId)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Book>($"/api/books/{bookId}");
            return response;
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<Book>> GetRecommendedBooksAsync(int count = 10)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Book>>($"/api/books/recommend?count={count}");
            return response ?? new List<Book>();
        }
        catch
        {
            return new List<Book>();
        }
    }
}
