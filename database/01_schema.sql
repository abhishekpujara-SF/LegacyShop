IF DB_ID('LegacyShop') IS NULL CREATE DATABASE LegacyShop;
GO
USE LegacyShop;
GO
IF OBJECT_ID('dbo.OrderItems') IS NOT NULL DROP TABLE dbo.OrderItems;
IF OBJECT_ID('dbo.Orders') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.Products') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Categories') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Customers') IS NOT NULL DROP TABLE dbo.Customers;
IF OBJECT_ID('dbo.Users') IS NOT NULL DROP TABLE dbo.Users;
GO
CREATE TABLE dbo.Categories (
    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
    Name       NVARCHAR(100) NOT NULL UNIQUE
);
CREATE TABLE dbo.Products (
    ProductId  INT IDENTITY(1,1) PRIMARY KEY,
    CategoryId INT NOT NULL CONSTRAINT FK_Products_Categories REFERENCES dbo.Categories(CategoryId),
    Sku        NVARCHAR(30)  NOT NULL UNIQUE,
    Name       NVARCHAR(150) NOT NULL,
    Price      DECIMAL(10,2) NOT NULL CHECK (Price >= 0),
    Stock      INT           NOT NULL DEFAULT 0,
    IsActive   BIT           NOT NULL DEFAULT 1
);
CREATE TABLE dbo.Customers (
    CustomerId INT IDENTITY(1,1) PRIMARY KEY,
    FullName   NVARCHAR(120) NOT NULL,
    Email      NVARCHAR(200) NOT NULL UNIQUE,
    City       NVARCHAR(80)  NULL,
    CreatedOn  DATETIME      NOT NULL DEFAULT GETDATE()
);
CREATE TABLE dbo.Orders (
    OrderId    INT IDENTITY(1,1) PRIMARY KEY,
    CustomerId INT NOT NULL CONSTRAINT FK_Orders_Customers REFERENCES dbo.Customers(CustomerId),
    OrderDate  DATETIME NOT NULL DEFAULT GETDATE(),
    Status     NVARCHAR(20) NOT NULL DEFAULT 'New'
);
CREATE TABLE dbo.OrderItems (
    OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT NOT NULL CONSTRAINT FK_OrderItems_Orders REFERENCES dbo.Orders(OrderId),
    ProductId   INT NOT NULL CONSTRAINT FK_OrderItems_Products REFERENCES dbo.Products(ProductId),
    Quantity    INT NOT NULL CHECK (Quantity > 0),
    UnitPrice   DECIMAL(10,2) NOT NULL
);
-- Legacy auth table: unsalted SHA1 hex (intentionally weak; auth-security-migration should flag it)
CREATE TABLE dbo.Users (
    UserId       INT IDENTITY(1,1) PRIMARY KEY,
    UserName     NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash CHAR(40) NOT NULL,
    Role         NVARCHAR(20) NOT NULL DEFAULT 'Admin'
);
GO
