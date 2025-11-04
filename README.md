# 图书智能检索聊天系统

基于Microsoft Agent Framework (Semantic Kernel)构建的智能图书检索系统,通过自然语言对话方式帮助用户快速检索图书数据库信息。

## 项目特点

- ✅ **自然语言查询**: 用户通过对话方式查询图书信息
- ✅ **智能检索**: AI Agent自动调用工具查询数据库
- ✅ **多轮对话**: 支持上下文理解的多轮交互
- ✅ **图书推荐**: 基于用户需求智能推荐相关图书
- ✅ **Text-to-SQL**: 支持复杂的自然语言转SQL查询

## 技术栈

- **开发语言**: C# (.NET 8.0)
- **前端**: Blazor WebAssembly + MudBlazor
- **后端**: ASP.NET Core Web API
- **AI框架**: Microsoft Semantic Kernel v1.30.0
- **AI模型**: Ollama + Qwen2.5:7b
- **数据库**: Microsoft SQL Server (Docker)
- **ORM**: Dapper
- **架构**: Clean Architecture

## 项目结构

```
图书智能检索系统/
├── src/                                    # 源代码目录
│   ├── BookSearchSystem.WebAPI/           # 后端 API 项目
│   │   ├── Controllers/                   # API 控制器
│   │   ├── Services/                      # 业务服务层
│   │   ├── DTOs/                          # 数据传输对象
│   │   ├── Program.cs                     # API 启动入口
│   │   └── appsettings.json              # API 配置文件
│   │
│   ├── BookSearchSystem.Blazor/           # 前端 Blazor WebAssembly 项目
│   │   ├── Pages/                         # 页面组件
│   │   ├── Services/                      # 前端服务
│   │   ├── Models/                        # 前端模型
│   │   └── Program.cs                     # 前端启动入口
│   │
│   └── BookSearchSystem.Shared/           # 共享类库
│       ├── DTOs/                          # 共享数据传输对象
│       └── Models/                        # 共享数据模型
│
├── database/                              # 数据库脚本
│   └── InitDatabase.sql                  # 数据库初始化脚本
│
├── tests/                                 # 测试项目（待添加）
├── BookSearchSystem.sln                   # 解决方案文件
├── Directory.Build.props                  # 统一项目配置
├── .gitignore                            # Git 忽略配置
└── README.md                             # 项目说明文档
```

## 快速开始

### 1. 环境要求

- .NET 8.0 SDK 或更高版本
- Docker Desktop
- Ollama (本地 AI 模型服务)

### 2. 启动 SQL Server (Docker)

```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2019-latest
```

### 3. 初始化数据库

在 SQL Server 中执行初始化脚本:

```bash
sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -i database/InitDatabase.sql
```

### 4. 启动 Ollama 并下载模型

```bash
# 启动 Ollama
ollama serve

# 下载 Qwen2.5:7b 模型
ollama pull qwen2.5:7b
```

### 5. 配置应用

编辑 `src/BookSearchSystem.WebAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "BookDatabase": "Server=localhost,1433;Database=BookLibrary;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True;"
  },
  "Ollama": {
    "Endpoint": "http://localhost:11434",
    "ModelName": "qwen2.5:7b"
  }
}
```

### 6. 安装依赖

```bash
dotnet restore
```

### 7. 运行应用

#### 运行后端 API

```bash
cd src/BookSearchSystem.WebAPI
dotnet run
```

后端 API 将在 `https://localhost:5001` 启动

#### 运行前端 Blazor

```bash
cd src/BookSearchSystem.Blazor
dotnet run
```

前端应用将在 `https://localhost:5002` 启动

#### 使用 Visual Studio

直接打开 `BookSearchSystem.sln` 解决方案文件，设置多个启动项目同时运行前后端。

## 使用示例

### 基础查询

```
您: 帮我找一下关于机器学习的书
助手: 我为您找到了以下机器学习相关的图书：
1. 《机器学习》 - 周志华，清华大学出版社，价格：88.00元，库存：15本
2. 《统计学习方法》 - 李航，清华大学出版社，价格：68.00元，库存：20本
3. 《Python机器学习》 - Sebastian Raschka，机械工业出版社，价格：89.00元，库存：18本
```

### 图书推荐

```
您: 推荐一些深度学习的书
助手: 为您推荐以下深度学习图书：
1. 《深度学习》 - Ian Goodfellow，人民邮电出版社，价格：168.00元
2. 《动手学深度学习》 - 阿斯顿·张，人民邮电出版社，价格：98.00元
```

### 库存查询

```
您: 查一下《机器学习》这本书的库存
助手: 《机器学习》（周志华著）目前库存：15本，状态：有货
```

### 复杂查询

```
您: 统计一下每个分类有多少本书
助手: 各分类图书统计如下：
- 机器学习: 3本
- 深度学习: 2本
- Python: 2本
- 人工智能: 1本
- 计算机: 1本
- 编程语言: 1本
```

## 核心功能

### 1. 图书搜索 (search_books)

根据标题、作者或分类搜索图书

**参数**:
- `title`: 图书标题关键词
- `author`: 作者名称
- `category`: 分类名称
- `maxResults`: 最大返回结果数

### 2. 获取图书详情 (get_book_details)

获取指定图书的详细信息

**参数**:
- `bookId`: 图书ID

### 3. 查询库存 (check_stock)

查询图书库存情况

**参数**:
- `bookId`: 图书ID

### 4. 图书推荐 (recommend_books)

根据分类或作者推荐图书

**参数**:
- `category`: 图书分类（可选）
- `author`: 作者名称（可选）
- `maxResults`: 最大推荐数量

### 5. 查询借阅记录 (get_borrow_records)

查询借阅记录

**参数**:
- `bookId`: 图书ID（可选）
- `userId`: 用户ID（可选）

## 数据库表结构

### Books (图书表)
- BookID: 图书ID (主键)
- Title: 标题
- Author: 作者
- Publisher: 出版社
- PublishDate: 出版日期
- ISBN: ISBN号
- Category: 分类
- Price: 价格
- Stock: 库存
- Description: 描述

### Authors (作者表)
- AuthorID: 作者ID (主键)
- AuthorName: 作者姓名
- Biography: 简介
- Country: 国家

### Categories (分类表)
- CategoryID: 分类ID (主键)
- CategoryName: 分类名称
- ParentCategoryID: 父分类ID

### BorrowRecords (借阅记录表)
- RecordID: 记录ID (主键)
- BookID: 图书ID
- UserID: 用户ID
- BorrowDate: 借阅日期
- ReturnDate: 归还日期
- Status: 状态

## 开发说明

### 添加新功能

1. 在 `Plugins/BookSearchPlugin.cs` 中添加新的 `[KernelFunction]`
2. 使用 `[Description]` 属性描述功能
3. 函数会自动被Semantic Kernel识别并可被AI调用

### 日志查看

日志文件位于 `logs/` 目录下,按天滚动。

### 调试技巧

- 设置环境变量 `SERILOG_MINIMUM_LEVEL=Debug` 查看详细日志
- 查看 `logs/` 目录下的日志文件
- 使用 Visual Studio 或 VS Code 调试器

## 常见问题

### Q: 数据库连接失败?
A: 检查 `appsettings.json` 中的连接字符串是否正确,确保SQL Server服务已启动。

### Q: Azure OpenAI API调用失败?
A: 确认API密钥和Endpoint配置正确,检查网络连接。

### Q: 查询结果为空?
A: 确认数据库已正确初始化并包含测试数据。

## 许可证

本项目仅供学习和研究使用。

## 联系方式

如有问题或建议,请联系项目负责人。

---

**指导教师**: 范丰龙  
**创建日期**: 2025年11月1日
