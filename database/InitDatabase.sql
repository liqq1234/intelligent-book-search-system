-- 图书智能检索系统数据库初始化脚本
-- 创建数据库
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'BookLibrary')
BEGIN
    CREATE DATABASE BookLibrary;
END
GO

USE BookLibrary;
GO

-- 创建图书表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Books')
BEGIN
    CREATE TABLE Books (
        BookID INT PRIMARY KEY IDENTITY(1,1),
        Title NVARCHAR(200) NOT NULL,
        Author NVARCHAR(100) NOT NULL,
        Publisher NVARCHAR(100),
        PublishDate DATE,
        ISBN VARCHAR(20),
        Category NVARCHAR(50),
        Price DECIMAL(10,2),
        Stock INT DEFAULT 0,
        Description NVARCHAR(MAX),
        CreatedAt DATETIME DEFAULT GETDATE()
    );
END
GO

-- 创建作者表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Authors')
BEGIN
    CREATE TABLE Authors (
        AuthorID INT PRIMARY KEY IDENTITY(1,1),
        AuthorName NVARCHAR(100) NOT NULL,
        Biography NVARCHAR(MAX),
        Country NVARCHAR(50)
    );
END
GO

-- 创建分类表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
BEGIN
    CREATE TABLE Categories (
        CategoryID INT PRIMARY KEY IDENTITY(1,1),
        CategoryName NVARCHAR(50) NOT NULL,
        ParentCategoryID INT NULL,
        FOREIGN KEY (ParentCategoryID) REFERENCES Categories(CategoryID)
    );
END
GO

-- 创建借阅记录表
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'BorrowRecords')
BEGIN
    CREATE TABLE BorrowRecords (
        RecordID INT PRIMARY KEY IDENTITY(1,1),
        BookID INT NOT NULL,
        UserID INT NOT NULL,
        BorrowDate DATE NOT NULL,
        ReturnDate DATE,
        Status NVARCHAR(20) DEFAULT N'借出中',
        FOREIGN KEY (BookID) REFERENCES Books(BookID)
    );
END
GO

-- 插入测试数据 - 分类
IF NOT EXISTS (SELECT * FROM Categories)
BEGIN
    INSERT INTO Categories (CategoryName, ParentCategoryID) VALUES
    (N'计算机', NULL),
    (N'人工智能', 1),
    (N'机器学习', 2),
    (N'深度学习', 2),
    (N'编程语言', 1),
    (N'Python', 5),
    (N'文学', NULL),
    (N'小说', 7),
    (N'历史', NULL),
    (N'科学', NULL);
END
GO

-- 插入测试数据 - 作者
IF NOT EXISTS (SELECT * FROM Authors)
BEGIN
    INSERT INTO Authors (AuthorName, Biography, Country) VALUES
    (N'吴恩达', N'人工智能领域专家，Coursera联合创始人', N'美国'),
    (N'李航', N'统计学习方法作者，知名机器学习专家', N'中国'),
    (N'周志华', N'南京大学教授，机器学习领域专家', N'中国'),
    (N'Ian Goodfellow', N'深度学习领域专家，GAN发明者', N'加拿大'),
    (N'Yann LeCun', N'深度学习先驱，图灵奖得主', N'法国');
END
GO

-- 插入测试数据 - 图书
IF NOT EXISTS (SELECT * FROM Books)
BEGIN
    INSERT INTO Books (Title, Author, Publisher, PublishDate, ISBN, Category, Price, Stock, Description) VALUES
    (N'机器学习', N'周志华', N'清华大学出版社', '2016-01-01', '9787302423287', N'机器学习', 88.00, 15, N'机器学习领域经典教材，系统介绍机器学习基本概念和算法'),
    (N'统计学习方法', N'李航', N'清华大学出版社', '2012-03-01', '9787302275954', N'机器学习', 68.00, 20, N'统计学习方法的经典著作，涵盖监督学习主要方法'),
    (N'深度学习', N'Ian Goodfellow', N'人民邮电出版社', '2017-08-01', '9787115461476', N'深度学习', 168.00, 10, N'深度学习领域权威教材，被誉为深度学习圣经'),
    (N'Python编程：从入门到实践', N'Eric Matthes', N'人民邮电出版社', '2016-07-01', '9787115428028', N'Python', 89.00, 25, N'Python编程入门经典，适合零基础学习者'),
    (N'流畅的Python', N'Luciano Ramalho', N'人民邮电出版社', '2017-05-01', '9787115454157', N'Python', 139.00, 12, N'Python进阶必读，深入理解Python语言特性'),
    (N'人工智能：一种现代方法', N'Stuart Russell', N'人民邮电出版社', '2013-11-01', '9787115318077', N'人工智能', 119.00, 8, N'人工智能领域经典教材，全面介绍AI基础理论'),
    (N'Python机器学习', N'Sebastian Raschka', N'机械工业出版社', '2017-03-01', '9787111558804', N'机器学习', 89.00, 18, N'结合Python实现机器学习算法，实践性强'),
    (N'动手学深度学习', N'阿斯顿·张', N'人民邮电出版社', '2019-06-01', '9787115510891', N'深度学习', 98.00, 22, N'PyTorch版深度学习实战教程'),
    (N'算法导论', N'Thomas H. Cormen', N'机械工业出版社', '2012-12-01', '9787111407010', N'计算机', 128.00, 10, N'算法领域经典教材，计算机专业必读'),
    (N'设计模式：可复用面向对象软件的基础', N'Erich Gamma', N'机械工业出版社', '2007-09-01', '9787111211990', N'编程语言', 65.00, 15, N'软件设计模式经典著作，四人帮著作');
END
GO

-- 插入测试数据 - 借阅记录
IF NOT EXISTS (SELECT * FROM BorrowRecords)
BEGIN
    INSERT INTO BorrowRecords (BookID, UserID, BorrowDate, ReturnDate, Status) VALUES
    (1, 1001, '2024-10-01', '2024-10-15', N'已归还'),
    (2, 1002, '2024-10-05', NULL, N'借出中'),
    (3, 1003, '2024-10-10', NULL, N'借出中'),
    (4, 1001, '2024-10-12', '2024-10-20', N'已归还'),
    (5, 1004, '2024-10-15', NULL, N'借出中');
END
GO

-- 创建索引以提高查询性能
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Books_Title')
    CREATE INDEX IX_Books_Title ON Books(Title);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Books_Author')
    CREATE INDEX IX_Books_Author ON Books(Author);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Books_Category')
    CREATE INDEX IX_Books_Category ON Books(Category);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_BorrowRecords_BookID')
    CREATE INDEX IX_BorrowRecords_BookID ON BorrowRecords(BookID);
GO

PRINT N'数据库初始化完成！';
PRINT N'已创建表：Books, Authors, Categories, BorrowRecords';
PRINT N'已插入测试数据：10本图书，5位作者，10个分类，5条借阅记录';
GO
