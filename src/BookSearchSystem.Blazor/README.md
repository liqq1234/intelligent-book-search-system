# 📚 图书智能检索系统 - Blazor前端

基于 Blazor WebAssembly 和 MudBlazor 构建的现代化图书检索系统前端界面。

## ✨ 功能特性

- 🎨 **现代化UI设计** - 使用 MudBlazor 组件库，界面美观大方
- 💬 **智能对话** - 与AI助手对话查询图书
- 🔍 **高级搜索** - 支持多条件筛选和分页
- 📖 **图书详情** - 详细的图书信息展示
- ⭐ **收藏功能** - 收藏喜欢的图书
- 📊 **统计分析** - 数据可视化展示
- 📱 **响应式设计** - 完美支持手机、平板、电脑

## 🛠️ 技术栈

- **框架**: Blazor WebAssembly (.NET 8.0)
- **UI库**: MudBlazor 8.13.0
- **语言**: C# 12.0
- **HTTP客户端**: HttpClient
- **状态管理**: Blazor内置

## 📂 项目结构

```
BookSearchSystem.Blazor/
├── Pages/              # 页面组件
│   ├── Home.razor      # 首页
│   ├── Chat.razor      # 智能对话
│   ├── Search.razor    # 图书搜索
│   ├── BookDetails.razor # 图书详情
│   ├── Favorites.razor # 我的收藏
│   ├── Statistics.razor # 统计分析
│   └── Settings.razor  # 设置
├── Shared/             # 共享组件
│   ├── BookCard.razor  # 图书卡片
│   └── ChatMessage.razor # 聊天消息
├── Layout/             # 布局组件
│   ├── MainLayout.razor # 主布局
│   └── NavMenu.razor   # 导航菜单
├── Services/           # 服务层
│   ├── ChatService.cs  # 聊天服务
│   └── BookService.cs  # 图书服务
├── Models/             # 数据模型
│   ├── Book.cs
│   └── ChatMessageModel.cs
└── DTOs/               # 数据传输对象
    ├── ChatRequest.cs
    ├── ChatResponse.cs
    └── SearchRequest.cs
```

## 🚀 快速开始

### 前置要求

- .NET 8.0 SDK 或更高版本
- Visual Studio 2022 或 VS Code

### 安装步骤

1. **克隆项目**
```bash
cd BookSearchSystem.Blazor
```

2. **还原依赖**
```bash
dotnet restore
```

3. **运行项目**
```bash
dotnet run
```

或使用热重载模式：
```bash
dotnet watch run
```

4. **访问应用**
打开浏览器访问: `http://localhost:5000`

## ⚙️ 配置

### API地址配置

在 `Program.cs` 中修改API基地址：

```csharp
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5001") 
});
```

## 📱 页面说明

### 首页 (/)
- 展示系统概览
- 快捷操作入口
- 推荐图书展示

### 智能对话 (/chat)
- AI对话界面
- 自然语言查询
- 实时图书推荐

### 图书搜索 (/search)
- 多条件搜索
- 分页展示
- 图书卡片视图

### 图书详情 (/book/{id})
- 详细信息展示
- 借阅操作
- 收藏功能

### 我的收藏 (/favorites)
- 收藏图书列表
- 快速访问

### 统计分析 (/statistics)
- 数据统计
- 图表展示

### 设置 (/settings)
- 主题切换
- 个人偏好设置

## 🎨 UI组件

项目使用 MudBlazor 组件库，主要组件包括：

- `MudAppBar` - 顶部应用栏
- `MudDrawer` - 侧边抽屉
- `MudCard` - 卡片组件
- `MudButton` - 按钮
- `MudTextField` - 文本输入框
- `MudGrid` - 网格布局
- `MudPagination` - 分页
- `MudSnackbar` - 消息提示

## 🔧 开发指南

### 添加新页面

1. 在 `Pages/` 目录创建 `.razor` 文件
2. 添加 `@page` 指令定义路由
3. 在 `NavMenu.razor` 添加导航链接

### 创建新组件

1. 在 `Shared/` 或 `Components/` 创建组件
2. 定义 `[Parameter]` 属性接收参数
3. 使用 `EventCallback` 处理事件

### 调用API

```csharp
@inject IBookService BookService

private async Task LoadBooks()
{
    var books = await BookService.GetRecommendedBooksAsync(10);
}
```

## 📝 注意事项

1. **API连接**: 确保后端API服务已启动
2. **CORS配置**: 后端需要配置CORS允许前端访问
3. **端口冲突**: 默认端口5000，如有冲突请修改
4. **浏览器兼容**: 推荐使用Chrome、Edge、Firefox最新版本

## 🐛 常见问题

### 无法连接API
- 检查API服务是否启动
- 确认API地址配置正确
- 检查CORS设置

### 页面加载慢
- 首次加载需要下载WebAssembly
- 后续访问会有缓存加速

### 样式显示异常
- 清除浏览器缓存
- 确认MudBlazor资源加载成功

## 📄 许可证

本项目仅供学习使用。

## 👥 贡献

欢迎提交Issue和Pull Request！

## 📞 联系方式

如有问题，请通过Issue联系。
