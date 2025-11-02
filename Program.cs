using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Serilog;
using BookSearchSystem.Services;
using BookSearchSystem.Plugins;
using Microsoft.SemanticKernel.Connectors.Ollama;

namespace BookSearchSystem;

class Program
{
    static async Task Main(string[] args)
    {
        // 配置Serilog日志
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .WriteTo.File("logs/book-agent-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            // 加载配置
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddEnvironmentVariables()
                .Build();

            Console.WriteLine("=" + new string('=', 59));
            Console.WriteLine("图书智能检索聊天系统");
            Console.WriteLine("=" + new string('=', 59));
            Console.WriteLine("欢迎使用图书智能检索系统！");
            Console.WriteLine("我可以帮您查询图书、推荐图书、查看库存等。");
            Console.WriteLine("输入 'exit' 或 'quit' 退出系统");
            Console.WriteLine("输入 'reset' 重置对话历史");
            Console.WriteLine("=" + new string('=', 59));
            Console.WriteLine();

            // 初始化服务
            var databaseService = new DatabaseService(configuration);
            
            // 测试数据库连接
            if (!await databaseService.TestConnectionAsync())
            {
                Console.WriteLine("❌ 数据库连接失败！请检查配置。");
                return;
            }

            Console.WriteLine("✅ 数据库连接成功！");
            Console.WriteLine();

            // 创建Kernel
            var kernelBuilder = Kernel.CreateBuilder();

            // 配置Ollama本地模型
            var ollamaEndpoint = configuration["Ollama:Endpoint"] ?? "http://localhost:11434";
            var modelName = configuration["Ollama:ModelName"] ?? "qwen2.5:7b";
            
            Console.WriteLine($"🤖 使用Ollama本地模型: {modelName}");
            Console.WriteLine($"   连接地址: {ollamaEndpoint}");
            Console.WriteLine();
            
            kernelBuilder.AddOllamaChatCompletion(
                modelId: modelName,
                endpoint: new Uri(ollamaEndpoint)
            );

            // 添加插件
            var bookSearchPlugin = new BookSearchPlugin(databaseService);
            kernelBuilder.Plugins.AddFromObject(bookSearchPlugin, "BookSearchPlugin");

            var kernel = kernelBuilder.Build();

            // 获取Agent配置
            var agentInstructions = configuration["AgentSettings:Instructions"] ?? "你是一个图书馆智能助手。";

            // 创建聊天历史
            var chatHistory = new Microsoft.SemanticKernel.ChatCompletion.ChatHistory(agentInstructions);

            // 获取聊天服务
            var chatService = kernel.GetRequiredService<Microsoft.SemanticKernel.ChatCompletion.IChatCompletionService>();

            // 对话循环
            while (true)
            {
                try
                {
                    Console.Write("您: ");
                    var userInput = Console.ReadLine()?.Trim();

                    if (string.IsNullOrEmpty(userInput))
                    {
                        continue;
                    }

                    if (userInput.ToLower() is "exit" or "quit" or "退出")
                    {
                        Console.WriteLine("\n感谢使用图书智能检索系统，再见！");
                        break;
                    }

                    if (userInput.ToLower() == "reset")
                    {
                        chatHistory.Clear();
                        chatHistory.AddSystemMessage(agentInstructions);
                        Console.WriteLine("\n✅ 对话历史已重置\n");
                        continue;
                    }

                    // 添加用户消息
                    chatHistory.AddUserMessage(userInput);

                    // 获取AI响应
                    var executionSettings = new PromptExecutionSettings
                    {
                        ExtensionData = new Dictionary<string, object>
                        {
                            ["temperature"] = 0.7,
                            ["max_tokens"] = 1500
                        }
                    };

                    var response = await chatService.GetChatMessageContentsAsync(
                        chatHistory,
                        executionSettings,
                        kernel
                    );
                    
                    var lastMessage = response[^1];

                    // 添加助手响应到历史
                    chatHistory.AddAssistantMessage(lastMessage.Content ?? "");

                    Console.WriteLine($"\n助手: {lastMessage.Content}\n");
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "处理消息时发生错误");
                    Console.WriteLine($"\n❌ 发生错误: {ex.Message}\n");
                }
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "应用程序启动失败");
            Console.WriteLine($"❌ 应用程序启动失败: {ex.Message}");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
