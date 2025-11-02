using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Dapper;
using Serilog;

namespace BookSearchSystem.Services;

/// <summary>
/// 数据库服务类
/// 提供SQL Server数据库连接和查询功能
/// </summary>
public class DatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("BookDatabase") 
            ?? throw new InvalidOperationException("数据库连接字符串未配置");
    }

    /// <summary>
    /// 获取数据库连接
    /// </summary>
    public SqlConnection GetConnection()
    {
        return new SqlConnection(_connectionString);
    }

    /// <summary>
    /// 执行查询并返回结果
    /// </summary>
    public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
    {
        try
        {
            using var connection = GetConnection();
            var results = await connection.QueryAsync<T>(sql, param);
            Log.Information("查询成功，返回 {Count} 条记录", results.Count());
            return results;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "查询执行失败: {Sql}", sql);
            throw;
        }
    }

    /// <summary>
    /// 执行查询并返回动态对象
    /// </summary>
    public async Task<IEnumerable<dynamic>> QueryAsync(string sql, object? param = null)
    {
        try
        {
            using var connection = GetConnection();
            var results = await connection.QueryAsync(sql, param);
            Log.Information("查询成功，返回 {Count} 条记录", results.Count());
            return results;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "查询执行失败: {Sql}", sql);
            throw;
        }
    }

    /// <summary>
    /// 执行非查询SQL语句
    /// </summary>
    public async Task<int> ExecuteAsync(string sql, object? param = null)
    {
        try
        {
            using var connection = GetConnection();
            var rowsAffected = await connection.ExecuteAsync(sql, param);
            Log.Information("执行成功，影响 {Rows} 行", rowsAffected);
            return rowsAffected;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "执行失败: {Sql}", sql);
            throw;
        }
    }

    /// <summary>
    /// 获取数据库表结构描述
    /// 用于Text-to-SQL的Prompt
    /// </summary>
    public async Task<string> GetTableSchemaAsync()
    {
        var sql = @"
            SELECT 
                t.TABLE_NAME,
                c.COLUMN_NAME,
                c.DATA_TYPE,
                c.IS_NULLABLE
            FROM 
                INFORMATION_SCHEMA.TABLES t
                INNER JOIN INFORMATION_SCHEMA.COLUMNS c 
                    ON t.TABLE_NAME = c.TABLE_NAME
            WHERE 
                t.TABLE_TYPE = 'BASE TABLE'
                AND t.TABLE_SCHEMA = 'dbo'
            ORDER BY 
                t.TABLE_NAME, c.ORDINAL_POSITION";

        try
        {
            var results = await QueryAsync(sql);
            
            // 按表名分组
            var tables = new Dictionary<string, List<string>>();
            foreach (var row in results)
            {
                string tableName = row.TABLE_NAME;
                if (!tables.ContainsKey(tableName))
                {
                    tables[tableName] = new List<string>();
                }

                string columnInfo = $"{row.COLUMN_NAME} ({row.DATA_TYPE})";
                if (row.IS_NULLABLE == "NO")
                {
                    columnInfo += " NOT NULL";
                }
                tables[tableName].Add(columnInfo);
            }

            // 构建Schema描述
            var schemaBuilder = new System.Text.StringBuilder();
            foreach (var table in tables)
            {
                schemaBuilder.AppendLine($"{table.Key}:");
                foreach (var column in table.Value)
                {
                    schemaBuilder.AppendLine($"  - {column}");
                }
                schemaBuilder.AppendLine();
            }

            return schemaBuilder.ToString();
        }
        catch (Exception ex)
        {
            Log.Error(ex, "获取表结构失败");
            // 返回默认Schema
            return GetDefaultSchema();
        }
    }

    /// <summary>
    /// 获取默认Schema描述
    /// </summary>
    private string GetDefaultSchema()
    {
        return @"
Books:
  - BookID (int) NOT NULL
  - Title (nvarchar) NOT NULL
  - Author (nvarchar) NOT NULL
  - Publisher (nvarchar)
  - PublishDate (date)
  - ISBN (varchar)
  - Category (nvarchar)
  - Price (decimal)
  - Stock (int)
  - Description (nvarchar)

Authors:
  - AuthorID (int) NOT NULL
  - AuthorName (nvarchar) NOT NULL
  - Biography (nvarchar)
  - Country (nvarchar)

Categories:
  - CategoryID (int) NOT NULL
  - CategoryName (nvarchar) NOT NULL
  - ParentCategoryID (int)

BorrowRecords:
  - RecordID (int) NOT NULL
  - BookID (int) NOT NULL
  - UserID (int) NOT NULL
  - BorrowDate (date) NOT NULL
  - ReturnDate (date)
  - Status (nvarchar)
";
    }

    /// <summary>
    /// 测试数据库连接
    /// </summary>
    public async Task<bool> TestConnectionAsync()
    {
        try
        {
            using var connection = GetConnection();
            await connection.OpenAsync();
            await connection.ExecuteAsync("SELECT 1");
            Log.Information("数据库连接测试成功");
            return true;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "数据库连接测试失败");
            return false;
        }
    }
}
