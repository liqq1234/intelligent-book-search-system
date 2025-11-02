# GitHub 上传指南

## 📋 准备工作

### 1. 在GitHub上创建仓库

1. 访问 https://github.com/new
2. 填写仓库信息：
   - **Repository name**: `intelligent-book-search-system`
   - **Description**: `基于AI的智能图书检索系统 - 使用Blazor WebAssembly、ASP.NET Core、Semantic Kernel和Ollama实现自然语言查询和智能SQL生成`
   - **Public** 或 **Private**: 根据需要选择
   - ⚠️ **不要勾选** "Add a README file"（本地已有）
   - ⚠️ **不要勾选** "Add .gitignore"（本地已有）
3. 点击 **Create repository**

---

## 🚀 上传步骤

### 方法1: 使用命令行（推荐）

打开PowerShell或命令提示符，进入项目目录：

```bash
cd c:\Users\LQQ\Desktop\图书智能检索系统
```

#### 步骤1: 初始化Git仓库
```bash
git init
```

#### 步骤2: 添加所有文件
```bash
git add .
```

#### 步骤3: 创建第一次提交
```bash
git commit -m "Initial commit: 智能图书检索系统完整项目"
```

#### 步骤4: 添加远程仓库
**替换 `YOUR_USERNAME` 为你的GitHub用户名**
```bash
git remote add origin https://github.com/YOUR_USERNAME/intelligent-book-search-system.git
```

#### 步骤5: 推送到GitHub
```bash
git branch -M main
git push -u origin main
```

---

### 方法2: 使用GitHub Desktop（图形界面）

1. 下载并安装 [GitHub Desktop](https://desktop.github.com/)
2. 打开GitHub Desktop
3. 点击 **File** → **Add Local Repository**
4. 选择项目文件夹：`c:\Users\LQQ\Desktop\图书智能检索系统`
5. 如果提示"This directory does not appear to be a Git repository"，点击 **create a repository**
6. 填写提交信息，点击 **Commit to main**
7. 点击 **Publish repository**
8. 选择仓库名称和可见性，点击 **Publish Repository**

---

## ⚠️ 上传前检查

### 确保敏感信息已排除

检查 `.gitignore` 文件是否包含以下内容：

```gitignore
# .NET
bin/
obj/
*.user
*.suo
*.cache
*.log

# 配置文件（包含敏感数据）
appsettings.Development.json
appsettings.Production.json

# 数据库文件
*.mdf
*.ldf

# IDE
.vs/
.vscode/
.idea/
```

### 检查配置文件

确保 `appsettings.json` 中**没有**真实的：
- ❌ 数据库密码
- ❌ API密钥
- ❌ 敏感信息

应该使用占位符：
```json
{
  "ConnectionStrings": {
    "BookDatabase": "Server=localhost,1433;Database=BookLibrary;User Id=sa;Password=YOUR_PASSWORD_HERE;TrustServerCertificate=True;Encrypt=False;"
  },
  "Ollama": {
    "Endpoint": "http://localhost:11434",
    "ModelName": "qwen2.5:7b"
  }
}
```

---

## 📝 提交信息建议

### 第一次提交
```
Initial commit: 智能图书检索系统完整项目

- 实现Blazor WebAssembly前端
- 实现ASP.NET Core Web API后端
- 集成Semantic Kernel和Ollama AI
- 实现AI自动生成SQL功能
- 完整的项目文档
```

### 后续提交示例
```
feat: 添加用户认证功能
fix: 修复数据库连接问题
docs: 更新README文档
refactor: 重构AI服务层
```

---

## 🔧 常见问题

### Q1: 提示"fatal: not a git repository"
**解决**: 先运行 `git init` 初始化仓库

### Q2: 提示"remote origin already exists"
**解决**: 运行 `git remote remove origin` 然后重新添加

### Q3: 推送失败，提示认证错误
**解决**: 
1. 使用GitHub Personal Access Token代替密码
2. 访问 https://github.com/settings/tokens
3. 生成新token，勾选 `repo` 权限
4. 使用token作为密码

### Q4: 文件太大无法上传
**解决**: 
1. 检查是否包含了 `bin/` 或 `obj/` 文件夹
2. 确保 `.gitignore` 正确配置
3. 运行 `git rm -r --cached bin obj` 移除已追踪的文件

### Q5: 想要撤销某次提交
**解决**: 
```bash
git reset --soft HEAD~1  # 撤销最后一次提交，保留更改
```

---

## 📊 上传后的工作

### 1. 添加仓库描述和标签
在GitHub仓库页面：
- 点击 ⚙️ Settings
- 添加 Topics: `ai`, `blazor`, `dotnet`, `semantic-kernel`, `ollama`, `book-search`

### 2. 创建Release（可选）
1. 点击 **Releases** → **Create a new release**
2. Tag version: `v1.0.0`
3. Release title: `智能图书检索系统 v1.0`
4. 描述项目功能和特点

### 3. 启用GitHub Pages（可选）
如果有静态文档，可以启用GitHub Pages展示

### 4. 添加徽章到README（可选）
```markdown
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Blazor](https://img.shields.io/badge/Blazor-WebAssembly-purple)
![License](https://img.shields.io/badge/license-MIT-green)
```

---

## 🎯 完整命令速查

```bash
# 1. 进入项目目录
cd c:\Users\LQQ\Desktop\图书智能检索系统

# 2. 初始化Git
git init

# 3. 添加所有文件
git add .

# 4. 创建提交
git commit -m "Initial commit: 智能图书检索系统完整项目"

# 5. 添加远程仓库（替换YOUR_USERNAME）
git remote add origin https://github.com/YOUR_USERNAME/intelligent-book-search-system.git

# 6. 推送到GitHub
git branch -M main
git push -u origin main
```

---

## 📚 Git基础命令参考

```bash
# 查看状态
git status

# 查看提交历史
git log

# 查看远程仓库
git remote -v

# 拉取最新代码
git pull

# 推送更改
git push

# 创建新分支
git checkout -b feature-name

# 切换分支
git checkout main

# 合并分支
git merge feature-name
```

---

## ✅ 上传检查清单

上传前请确认：
- [ ] `.gitignore` 文件配置正确
- [ ] 没有包含敏感信息（密码、API密钥）
- [ ] README.md 内容完整
- [ ] 项目文档齐全
- [ ] 代码可以正常编译运行
- [ ] 已在GitHub创建空仓库
- [ ] Git已正确安装

上传后请确认：
- [ ] 所有文件都已上传
- [ ] README在仓库首页正确显示
- [ ] 仓库描述和标签已添加
- [ ] 项目结构清晰可见

---

**准备好后，按照上面的命令执行即可！** 🚀
