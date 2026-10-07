USE LegacyShop;
GO
-- Many-to-many link between orders and customers (order "participants").
-- Orders.CustomerId stays the owning customer; this table holds the additional customers on an order.
-- Safe to re-run: creates the table and seed rows only when missing.
IF OBJECT_ID('dbo.OrderCustomers') IS NULL
BEGIN
    CREATE TABLE dbo.OrderCustomers (
        OrderId    INT NOT NULL CONSTRAINT FK_OrderCustomers_Orders    REFERENCES dbo.Orders(OrderId),
        CustomerId INT NOT NULL CONSTRAINT FK_OrderCustomers_Customers REFERENCES dbo.Customers(CustomerId),
        Role       NVARCHAR(20) NOT NULL CONSTRAINT DF_OrderCustomers_Role DEFAULT 'Contact'
                   CONSTRAINT CK_OrderCustomers_Role CHECK (Role IN ('Billing','Shipping','Approver','Contact')),
        AddedOn    DATETIME NOT NULL CONSTRAINT DF_OrderCustomers_AddedOn DEFAULT GETDATE(),
        CONSTRAINT PK_OrderCustomers PRIMARY KEY (OrderId, CustomerId)
    );
    CREATE INDEX IX_OrderCustomers_CustomerId ON dbo.OrderCustomers(CustomerId);

    -- Seed looked up by e-mail so a database with edited customers still loads; the owner is never linked twice.
    INSERT dbo.OrderCustomers(OrderId, CustomerId, Role)
    SELECT o.OrderId, c.CustomerId, v.Role
    FROM (VALUES
            (1, 'ben@example.com',   'Shipping'),   -- order 1 (owner Asha): Ben ships
            (2, 'asha@example.com',  'Approver'),   -- order 2 (owner Ben):  Asha approves
            (2, 'chloe@example.com', 'Billing'),    -- order 2 (owner Ben):  Chloe is billed
            (3, 'chloe@example.com', 'Contact')     -- order 3 (owner Asha): Chloe is a contact
         ) AS v(OrderId, Email, Role)
    JOIN dbo.Orders o    ON o.OrderId = v.OrderId
    JOIN dbo.Customers c ON c.Email = v.Email AND c.CustomerId <> o.CustomerId;
END
GO
