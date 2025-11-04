using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using BookSearchSystem.WebAPI.DTOs;
using System.ComponentModel;

namespace BookSearchSystem.WebAPI.Services;

public class KernelService : IKernelService
{
    private readonly Kernel _kernel;
    private readonly IDatabaseService _databaseService;
    private readonly IAdvancedDatabaseService _advancedDatabaseService;
    private readonly ILogger<KernelService> _logger;

    public KernelService(Kernel kernel, IDatabaseService databaseService, IAdvancedDatabaseService advancedDatabaseService, ILogger<KernelService> logger)
    {
        _kernel = kernel;
        _databaseService = databaseService;
        _advancedDatabaseService = advancedDatabaseService;
        _logger = logger;
    }

    public async Task<(string Content, List<BookDto>? Books)> ChatAsync(string message)
    {
        try
        {
            // 创建插件实例并保存引用以便获取搜索结果
            var searchPlugin = new BookSearchPlugin(_databaseService);
            var sqlPlugin = new SqlQueryPlugin(_advancedDatabaseService);
            
            _kernel.Plugins.Clear();
            _kernel.Plugins.AddFromObject(searchPlugin, "BookSearch");
            _kernel.Plugins.AddFromObject(sqlPlugin, "SqlQuery");

            // 获取聊天服务
            var chatService = _kernel.GetRequiredService<IChatCompletionService>();
            var history = new ChatHistory();
            
            history.AddSystemMessage(@"你是一个专业的图书检索助手。你可以帮助用户查找图书、推荐图书。

你有两个工具可以使用：
1. SearchBooks - 用于简单的关键词搜索（书名、作者、分类）
2. ExecuteSqlQuery - 用于复杂查询（如：最便宜的书、价格范围、库存排序等）

当用户需要复杂查询时（如最便宜、最贵、价格排序、库存筛选等），应该：
1. 先调用 GetDatabaseSchema 了解数据库结构
2. 根据用户需求生成合适的SQL SELECT语句
3. 调用 ExecuteSqlQuery 执行查询
4. 用友好的语言介绍查询结果

SQL查询示例：
- 最便宜的书: SELECT TOP 10 * FROM Books WHERE Price IS NOT NULL ORDER BY Price ASC
- 某个价格范围: SELECT * FROM Books WHERE Price BETWEEN 50 AND 100
- 库存最多的书: SELECT TOP 10 * FROM Books ORDER BY Stock DESC

注意：只生成SELECT语句，不要使用UPDATE、DELETE等危险操作。");

            history.AddUserMessage(message);

            // 启用自动函数调用
            var executionSettings = new OpenAIPromptExecutionSettings
            {
                ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
                MaxTokens = 2000,
                Temperature = 0.7
            };

            _logger.LogInformation("开始调用AI模型处理消息: {Message}", message);
            var result = await chatService.GetChatMessageContentAsync(history, executionSettings, _kernel);
            _logger.LogInformation("AI模型响应成功");
            
            // 从插件获取搜索到的图书
            var books = searchPlugin.LastSearchedBooks ?? sqlPlugin.LastQueryResults;

            return (result.Content ?? "抱歉，我无法回答这个问题。", books);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Chat处理失败: {Message}, 错误类型: {ExceptionType}", ex.Message, ex.GetType().Name);
            return ($"抱歉，处理您的请求时出现了问题：{ex.Message}", null);
        }
    }
}

// 图书搜索插件
public class BookSearchPlugin
{
    private readonly IDatabaseService _databaseService;
    public List<BookDto>? LastSearchedBooks { get; private set; }

    public BookSearchPlugin(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    [KernelFunction, Description("搜索图书数据库中的图书")]
    public async Task<string> SearchBooks(
        [Description("搜索关键词，可以是书名、作者或分类")] string keyword)
    {
        var books = await _databaseService.SearchBooksAsync(keyword);
        
        // 保存搜索结果供外部使用
        LastSearchedBooks = books;
        
        if (books == null || !books.Any())
        {
            return "未找到相关图书。";
        }

        var result = $"找到 {books.Count} 本相关图书：\n";
        foreach (var book in books.Take(5))
        {
            result += $"- 《{book.Title}》 作者：{book.Author}，价格：¥{book.Price}，库存：{book.Stock}\n";
        }

        return result;
    }
}

// SQL查询插件 - 支持AI生成SQL进行复杂查询
public class SqlQueryPlugin
{
    private readonly IAdvancedDatabaseService _advancedDatabaseService;
    public List<BookDto>? LastQueryResults { get; private set; }

    public SqlQueryPlugin(IAdvancedDatabaseService advancedDatabaseService)
    {
        _advancedDatabaseService = advancedDatabaseService;
    }

    [KernelFunction, Description("获取数据库表结构信息，用于生成SQL查询")]
    public string GetDatabaseSchema()
    {
        return _advancedDatabaseService.GetDatabaseSchema();
    }

    [KernelFunction, Description("执行自定义SQL查询（仅支持SELECT语句）。用于复杂查询，如：查找最便宜的书、价格排序、库存筛选等")]
    public async Task<string> ExecuteSqlQuery(
        [Description("SQL SELECT查询语句，例如：SELECT TOP 10 * FROM Books WHERE Price IS NOT NULL ORDER BY Price ASC")] string sqlQuery)
    {
        var books = await _advancedDatabaseService.ExecuteCustomQueryAsync(sqlQuery);
        
        // 保存查询结果供外部使用
        LastQueryResults = books;
        
        if (books == null || !books.Any())
        {
            return "查询未返回任何结果。";
        }

        var result = $"查询返回 {books.Count} 条结果：\n";
        foreach (var book in books.Take(10))
        {
            result += $"- 《{book.Title}》 作者：{book.Author}，价格：¥{book.Price}，库存：{book.Stock}\n";
        }

        return result;
    }
}
