USE LegacyShop;
GO
INSERT dbo.Categories(Name) VALUES (N'Keyboards'),(N'Monitors'),(N'Accessories');
INSERT dbo.Products(CategoryId,Sku,Name,Price,Stock,IsActive) VALUES
 (1,'KB-100',N'Mechanical Keyboard',79.99,25,1),
 (1,'KB-200',N'Wireless Keyboard',49.50,3,1),
 (2,'MN-240',N'24" Monitor',159.00,12,1),
 (2,'MN-270',N'27" 4K Monitor',329.00,2,1),
 (3,'AC-001',N'USB-C Cable',9.99,200,1),
 (3,'AC-002',N'Laptop Stand',34.00,0,0);
INSERT dbo.Customers(FullName,Email,City) VALUES
 (N'Asha Rao','asha@example.com',N'Pune'),
 (N'Ben Carter','ben@example.com',N'Austin'),
 (N'Chloe Dubois','chloe@example.com',N'Lyon');
INSERT dbo.Orders(CustomerId,OrderDate,Status) VALUES (1,'2026-01-10','Shipped'),(2,'2026-01-12','New'),(1,'2026-02-01','New');
INSERT dbo.OrderItems(OrderId,ProductId,Quantity,UnitPrice) VALUES
 (1,1,1,79.99),(1,5,2,9.99),(2,3,2,159.00),(3,4,1,329.00);
-- admin / Passw0rd!  (SHA1 hex)
INSERT dbo.Users(UserName,PasswordHash,Role) VALUES ('admin', LOWER(CONVERT(CHAR(40), HASHBYTES('SHA1','Passw0rd!'), 2)), 'Admin');
GO
