using Microsoft.Data.SqlClient;
using BookSearchSystem.WebAPI.DTOs;

namespace BookSearchSystem.WebAPI.Services;

public class AdvancedDatabaseService : IAdvancedDatabaseService
{
    private readonly string _connectionString;
    private readonly ILogger<AdvancedDatabaseService> _logger;

    public AdvancedDatabaseService(IConfiguration configuration, ILogger<AdvancedDatabaseService> logger)
    {
        _connectionString = configuration.GetConnectionString("BookDatabase") 
            ?? throw new InvalidOperationException("数据库连接字符串未配置");
        _logger = logger;
    }

    /// <summary>
    /// 获取数据库表结构信息，供AI参考
    /// </summary>
    public string GetDatabaseSchema()
    {
        return @"
数据库名称: BookLibrary

表名: Books
字段说明:
- BookID (INT, 主键): 图书ID
- Title (NVARCHAR(200)): 书名
- Author (NVARCHAR(100)): 作者
- Publisher (NVARCHAR(100)): 出版社
- PublishDate (DATE): 出版日期
- ISBN (VARCHAR(20)): ISBN编号
- Category (NVARCHAR(50)): 分类（如：Python, Java, 机器学习, 数据库等）
- Price (DECIMAL(10,2)): 价格（单位：元）
- Stock (INT): 库存数量
- Description (NVARCHAR(MAX)): 图书描述
- CreatedAt (DATETIME): 创建时间

查询示例:
1. 查找最便宜的书: SELECT TOP 10 * FROM Books WHERE Price IS NOT NULL ORDER BY Price ASC
2. 查找某分类的书: SELECT * FROM Books WHERE Category LIKE '%Python%'
3. 查找某作者的书: SELECT * FROM Books WHERE Author LIKE '%作者名%'
4. 查找价格范围: SELECT * FROM Books WHERE Price BETWEEN 50 AND 100
5. 查找库存充足的书: SELECT * FROM Books WHERE Stock > 10
";
    }

    /// <summary>
    /// 执行自定义SQL查询（仅支持SELECT语句）
    /// </summary>
    public async Task<List<BookDto>> ExecuteCustomQueryAsync(string sqlQuery)
    {
        var books = new List<BookDto>();

        try
        {
            // 安全检查：只允许SELECT语句
            var trimmedQuery = sqlQuery.Trim().ToUpper();
            if (!trimmedQuery.StartsWith("SELECT"))
            {
                _logger.LogWarning("拒绝执行非SELECT语句: {Query}", sqlQuery);
                return books;
            }

            // 防止危险操作
            if (trimmedQuery.Contains("DROP") || trimmedQuery.Contains("DELETE") || 
                trimmedQuery.Contains("UPDATE") || trimmedQuery.Contains("INSERT") ||
                trimmedQuery.Contains("ALTER") || trimmedQuery.Contains("TRUNCATE"))
            {
                _logger.LogWarning("拒绝执行包含危险关键字的SQL: {Query}", sqlQuery);
                return books;
            }

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            _logger.LogInformation("执行自定义SQL查询: {Query}", sqlQuery);

            using var command = new SqlCommand(sqlQuery, connection);
            command.CommandTimeout = 30;

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                books.Add(MapToBookDto(reader));
            }

            _logger.LogInformation("查询返回 {Count} 条结果", books.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "执行自定义SQL查询失败: {Query}", sqlQuery);
        }

        return books;
    }

    private BookDto MapToBookDto(SqlDataReader reader)
    {
        return new BookDto
        {
            BookId = GetSafeInt(reader, "BookID"),
            Title = GetSafeString(reader, "Title"),
            Author = GetSafeString(reader, "Author"),
            Publisher = GetSafeString(reader, "Publisher"),
            PublishDate = GetSafeDateTime(reader, "PublishDate"),
            ISBN = GetSafeString(reader, "ISBN"),
            Category = GetSafeString(reader, "Category"),
            Price = GetSafeDecimal(reader, "Price"),
            Stock = GetSafeInt(reader, "Stock"),
            Description = GetSafeString(reader, "Description")
        };
    }

    private int GetSafeInt(SqlDataReader reader, string columnName)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetInt32(ordinal);
        }
        catch
        {
            return 0;
        }
    }

    private string GetSafeString(SqlDataReader reader, string columnName)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
        }
        catch
        {
            return string.Empty;
        }
    }

    private DateTime? GetSafeDateTime(SqlDataReader reader, string columnName)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? null : reader.GetDateTime(ordinal);
        }
        catch
        {
            return null;
        }
    }

    private decimal GetSafeDecimal(SqlDataReader reader, string columnName)
    {
        try
        {
            var ordinal = reader.GetOrdinal(columnName);
            return reader.IsDBNull(ordinal) ? 0 : reader.GetDecimal(ordinal);
        }
        catch
        {
            return 0;
        }
    }
}
