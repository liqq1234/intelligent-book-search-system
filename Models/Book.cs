namespace BookSearchSystem.Models;

/// <summary>
/// 图书实体类
/// </summary>
public class Book
{
    public int BookID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string? Publisher { get; set; }
    public DateTime? PublishDate { get; set; }
    public string? ISBN { get; set; }
    public string? Category { get; set; }
    public decimal? Price { get; set; }
    public int Stock { get; set; }
    public string? Description { get; set; }
    public DateTime? CreatedAt { get; set; }
}

/// <summary>
/// 作者实体类
/// </summary>
public class Author
{
    public int AuthorID { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? Biography { get; set; }
    public string? Country { get; set; }
}

/// <summary>
/// 分类实体类
/// </summary>
public class Category
{
    public int CategoryID { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? ParentCategoryID { get; set; }
}

/// <summary>
/// 借阅记录实体类
/// </summary>
public class BorrowRecord
{
    public int RecordID { get; set; }
    public int BookID { get; set; }
    public int UserID { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BookTitle { get; set; }
}

/// <summary>
/// SQL查询结果
/// </summary>
public class SqlQueryResult
{
    public string Sql { get; set; } = string.Empty;
    public List<string> Params { get; set; } = new();
    public string? Error { get; set; }
}
