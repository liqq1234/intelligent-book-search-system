using BookSearchSystem.WebAPI.DTOs;

namespace BookSearchSystem.WebAPI.Services;

public interface IAdvancedDatabaseService
{
    Task<List<BookDto>> ExecuteCustomQueryAsync(string sqlQuery);
    string GetDatabaseSchema();
}
