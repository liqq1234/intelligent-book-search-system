using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using Serilog;
using BookSearchSystem.Models;
using BookSearchSystem.Services;

namespace BookSearchSystem.Plugins;

/// <summary>
/// 图书搜索插件
/// 提供图书查询、推荐等功能的Kernel Functions
/// </summary>
public class BookSearchPlugin
{
    private readonly DatabaseService _databaseService;

    public BookSearchPlugin(DatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [KernelFunction("search_books")]
    [Description("根据标题、作者或分类搜索图书")]
    public async Task<string> SearchBooksAsync(
        [Description("图书标题关键词")] string? title = null,
        [Description("作者名称")] string? author = null,
        [Description("分类名称")] string? category = null,
        [Description("最大返回结果数")] int maxResults = 10)
    {
        try
        {
            var conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(title))
            {
                conditions.Add("Title LIKE @Title");
                parameters["Title"] = $"%{title}%";
            }

            if (!string.IsNullOrEmpty(author))
            {
                conditions.Add("Author LIKE @Author");
                parameters["Author"] = $"%{author}%";
            }

            if (!string.IsNullOrEmpty(category))
            {
                conditions.Add("Category LIKE @Category");
                parameters["Category"] = $"%{category}%";
            }

            var whereClause = conditions.Any() ? "WHERE " + string.Join(" AND ", conditions) : "";

            var sql = $@"
                SELECT TOP {maxResults}
                    BookID, Title, Author, Publisher, 
                    PublishDate, Category, Price, Stock
                FROM Books
                {whereClause}
                ORDER BY Title";

            var results = await _databaseService.QueryAsync<Book>(sql, parameters);
            var bookList = results.ToList();

            if (!bookList.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    success = true,
                    message = "未找到符合条件的图书",
                    count = 0,
                    books = Array.Empty<Book>()
                }, new JsonSerializerOptions { WriteIndented = false, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            }

            Log.Information("搜索到 {Count} 本图书", bookList.Count);

            return JsonSerializer.Serialize(new
            {
                success = true,
                count = bookList.Count,
                books = bookList
            }, new JsonSerializerOptions { WriteIndented = false, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "搜索图书失败");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message });
        }
    }

    [KernelFunction("get_book_details")]
    [Description("获取指定图书的详细信息")]
    public async Task<string> GetBookDetailsAsync(
        [Description("图书ID")] int bookId)
    {
        try
        {
            var sql = @"
                SELECT 
                    BookID, Title, Author, Publisher, PublishDate,
                    ISBN, Category, Price, Stock, Description
                FROM Books
                WHERE BookID = @BookID";

            var results = await _databaseService.QueryAsync<Book>(sql, new { BookID = bookId });
            var book = results.FirstOrDefault();

            if (book == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"未找到ID为 {bookId} 的图书"
                }, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            }

            Log.Information("获取图书详情: {Title}", book.Title);

            return JsonSerializer.Serialize(new
            {
                success = true,
                book
            }, new JsonSerializerOptions { WriteIndented = false, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取图书详情失败");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message });
        }
    }

    [KernelFunction("check_stock")]
    [Description("查询图书库存情况")]
    public async Task<string> CheckStockAsync(
        [Description("图书ID")] int bookId)
    {
        try
        {
            var sql = "SELECT BookID, Title, Stock FROM Books WHERE BookID = @BookID";
            var results = await _databaseService.QueryAsync<Book>(sql, new { BookID = bookId });
            var book = results.FirstOrDefault();

            if (book == null)
            {
                return JsonSerializer.Serialize(new
                {
                    success = false,
                    message = $"未找到ID为 {bookId} 的图书"
                }, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            }

            var status = book.Stock > 0 ? "有货" : "缺货";
            Log.Information("库存查询: {Title} - {Stock}本", book.Title, book.Stock);

            return JsonSerializer.Serialize(new
            {
                success = true,
                book_id = bookId,
                title = book.Title,
                stock = book.Stock,
                status
            }, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "查询库存失败");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message });
        }
    }

    [KernelFunction("recommend_books")]
    [Description("根据分类或作者推荐图书")]
    public async Task<string> RecommendBooksAsync(
        [Description("图书分类")] string? category = null,
        [Description("作者名称")] string? author = null,
        [Description("最大推荐数量")] int maxResults = 5)
    {
        try
        {
            string sql;
            object? parameters;

            if (!string.IsNullOrEmpty(category))
            {
                sql = $@"
                    SELECT TOP {maxResults}
                        BookID, Title, Author, Category, Price, Stock
                    FROM Books
                    WHERE Category LIKE @Category
                    ORDER BY NEWID()";
                parameters = new { Category = $"%{category}%" };
            }
            else if (!string.IsNullOrEmpty(author))
            {
                sql = $@"
                    SELECT TOP {maxResults}
                        BookID, Title, Author, Category, Price, Stock
                    FROM Books
                    WHERE Author LIKE @Author
                    ORDER BY PublishDate DESC";
                parameters = new { Author = $"%{author}%" };
            }
            else
            {
                sql = $@"
                    SELECT TOP {maxResults}
                        BookID, Title, Author, Category, Price, Stock
                    FROM Books
                    WHERE Stock > 0
                    ORDER BY NEWID()";
                parameters = null;
            }

            var results = await _databaseService.QueryAsync<Book>(sql, parameters);
            var bookList = results.ToList();

            if (!bookList.Any())
            {
                return JsonSerializer.Serialize(new
                {
                    success = true,
                    message = "暂无推荐图书",
                    count = 0,
                    books = Array.Empty<Book>()
                }, new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            }

            Log.Information("推荐 {Count} 本图书", bookList.Count);

            return JsonSerializer.Serialize(new
            {
                success = true,
                count = bookList.Count,
                books = bookList,
                recommendation_type = !string.IsNullOrEmpty(category) ? "category" : 
                                     !string.IsNullOrEmpty(author) ? "author" : "random"
            }, new JsonSerializerOptions { WriteIndented = false, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "推荐图书失败");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message });
        }
    }

    [KernelFunction("get_borrow_records")]
    [Description("查询借阅记录")]
    public async Task<string> GetBorrowRecordsAsync(
        [Description("图书ID（可选）")] int? bookId = null,
        [Description("用户ID（可选）")] int? userId = null)
    {
        try
        {
            var conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (bookId.HasValue)
            {
                conditions.Add("br.BookID = @BookID");
                parameters["BookID"] = bookId.Value;
            }

            if (userId.HasValue)
            {
                conditions.Add("br.UserID = @UserID");
                parameters["UserID"] = userId.Value;
            }

            var whereClause = conditions.Any() ? "WHERE " + string.Join(" AND ", conditions) : "";

            var sql = $@"
                SELECT 
                    br.RecordID, br.BookID, b.Title as BookTitle, br.UserID,
                    br.BorrowDate, br.ReturnDate, br.Status
                FROM BorrowRecords br
                INNER JOIN Books b ON br.BookID = b.BookID
                {whereClause}
                ORDER BY br.BorrowDate DESC";

            var results = await _databaseService.QueryAsync<BorrowRecord>(sql, parameters);
            var recordList = results.ToList();

            Log.Information("查询到 {Count} 条借阅记录", recordList.Count);

            return JsonSerializer.Serialize(new
            {
                success = true,
                count = recordList.Count,
                records = recordList
            }, new JsonSerializerOptions { WriteIndented = false, Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "查询借阅记录失败");
            return JsonSerializer.Serialize(new { success = false, error = ex.Message });
        }
    }
}
