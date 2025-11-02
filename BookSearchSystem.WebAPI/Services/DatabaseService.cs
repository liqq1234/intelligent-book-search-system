using Microsoft.Data.SqlClient;
using BookSearchSystem.WebAPI.DTOs;

namespace BookSearchSystem.WebAPI.Services;

public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(IConfiguration configuration, ILogger<DatabaseService> logger)
    {
        _connectionString = configuration.GetConnectionString("BookDatabase") 
            ?? throw new InvalidOperationException("数据库连接字符串未配置");
        _logger = logger;
    }

    public async Task<List<BookDto>> SearchBooksAsync(string keyword)
    {
        var books = new List<BookDto>();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT TOP 10 BookId, Title, Author, Publisher, PublishDate, ISBN, 
                       Category, Price, Stock, Description
                FROM Books
                WHERE Title LIKE @Keyword 
                   OR Author LIKE @Keyword 
                   OR Category LIKE @Keyword
                ORDER BY BookId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Keyword", $"%{keyword}%");

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                books.Add(MapToBookDto(reader));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "搜索图书失败: {Keyword}", keyword);
        }

        return books;
    }

    public async Task<BookDto?> GetBookByIdAsync(int bookId)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = @"
                SELECT BookId, Title, Author, Publisher, PublishDate, ISBN, 
                       Category, Price, Stock, Description
                FROM Books
                WHERE BookId = @BookId";

            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@BookId", bookId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return MapToBookDto(reader);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取图书详情失败: {BookId}", bookId);
        }

        return null;
    }

    public async Task<List<BookDto>> GetRecommendedBooksAsync(int count = 10)
    {
        var books = new List<BookDto>();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var query = $@"
                SELECT TOP {count} BookId, Title, Author, Publisher, PublishDate, ISBN, 
                       Category, Price, Stock, Description
                FROM Books
                WHERE Stock > 0
                ORDER BY NEWID()";

            using var command = new SqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();
            
            while (await reader.ReadAsync())
            {
                books.Add(MapToBookDto(reader));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "获取推荐图书失败");
        }

        return books;
    }

    private BookDto MapToBookDto(SqlDataReader reader)
    {
        return new BookDto
        {
            BookId = reader.GetInt32(0),
            Title = reader.GetString(1),
            Author = reader.GetString(2),
            Publisher = reader.GetString(3),
            PublishDate = reader.IsDBNull(4) ? null : reader.GetDateTime(4),
            ISBN = reader.GetString(5),
            Category = reader.GetString(6),
            Price = reader.GetDecimal(7),
            Stock = reader.GetInt32(8),
            Description = reader.IsDBNull(9) ? null : reader.GetString(9)
        };
    }
}
