using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LegacyShop.Web.Models;

namespace LegacyShop.Web.Data
{
    /// <summary>Orders and order items via raw ADO.NET (SqlCommand / SqlDataReader), no ORM.</summary>
    public class OrderRepository
    {
        private const string OrderSelect =
            "SELECT o.OrderId, o.CustomerId, c.FullName, o.OrderDate, o.Status, " +
            "ISNULL(SUM(i.Quantity * i.UnitPrice), 0) AS Total, " +
            "(SELECT COUNT(*) FROM dbo.OrderCustomers oc WHERE oc.OrderId = o.OrderId) AS LinkedCount " +
            "FROM dbo.Orders o JOIN dbo.Customers c ON c.CustomerId = o.CustomerId " +
            "LEFT JOIN dbo.OrderItems i ON i.OrderId = o.OrderId ";

        private const string OrderGroupBy =
            "GROUP BY o.OrderId, o.CustomerId, c.FullName, o.OrderDate, o.Status ";

        public int CountCustomers()
        {
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Customers", conn))
                return (int)cmd.ExecuteScalar();
        }

        public int CountOpenOrders()
        {
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Orders WHERE Status NOT IN ('Shipped', 'Cancelled')", conn))
                return (int)cmd.ExecuteScalar();
        }

        public List<Order> GetOrders()
        {
            var list = new List<Order>();
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand(OrderSelect + OrderGroupBy + "ORDER BY o.OrderDate DESC, o.OrderId DESC", conn))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read()) list.Add(MapOrder(r));
            }
            return list;
        }

        public Order GetOrder(int orderId)
        {
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand(OrderSelect + "WHERE o.OrderId = @id " + OrderGroupBy, conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = orderId;
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? MapOrder(r) : null;
            }
        }

        /// <summary>Inserts (OrderId == 0) or updates an order and returns its id. A new owner is dropped from the order's participants.</summary>
        public int SaveOrder(Order order)
        {
            using (var conn = Db.Open())
            using (var tx = conn.BeginTransaction())
            {
                int id = order.OrderId;
                if (id == 0)
                {
                    using (var cmd = new SqlCommand(
                        "INSERT dbo.Orders(CustomerId, OrderDate, Status) VALUES(@c, @d, @s); SELECT CAST(SCOPE_IDENTITY() AS INT)", conn, tx))
                    {
                        AddOrderParameters(cmd, order);
                        id = (int)cmd.ExecuteScalar();
                    }
                }
                else
                {
                    using (var cmd = new SqlCommand(
                        "UPDATE dbo.Orders SET CustomerId = @c, OrderDate = @d, Status = @s WHERE OrderId = @id", conn, tx))
                    {
                        AddOrderParameters(cmd, order);
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        if (cmd.ExecuteNonQuery() == 0) throw new InvalidOperationException("Order no longer exists.");
                    }
                    using (var cmd = new SqlCommand(
                        "DELETE FROM dbo.OrderCustomers WHERE OrderId = @id AND CustomerId = @c", conn, tx))
                    {
                        cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                        cmd.Parameters.Add("@c", SqlDbType.Int).Value = order.CustomerId;
                        cmd.ExecuteNonQuery();
                    }
                }
                tx.Commit();
                return id;
            }
        }

        /// <summary>Deletes an order with its items and customer links. Shipped orders are kept.</summary>
        public void DeleteOrder(int orderId)
        {
            using (var conn = Db.Open())
            using (var tx = conn.BeginTransaction())
            {
                EnsureNotShipped(conn, tx, orderId, "Shipped orders cannot be deleted.");
                Execute(conn, tx, "DELETE FROM dbo.OrderCustomers WHERE OrderId = @id", orderId);
                Execute(conn, tx, "DELETE FROM dbo.OrderItems WHERE OrderId = @id", orderId);
                Execute(conn, tx, "DELETE FROM dbo.Orders WHERE OrderId = @id", orderId);
                tx.Commit();
            }
        }

        public List<OrderItem> GetItems(int orderId)
        {
            var list = new List<OrderItem>();
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand(
                "SELECT i.OrderItemId, i.OrderId, p.Name, i.Quantity, i.UnitPrice " +
                "FROM dbo.OrderItems i JOIN dbo.Products p ON p.ProductId = i.ProductId WHERE i.OrderId = @id ORDER BY i.OrderItemId", conn))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = orderId;
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new OrderItem
                        {
                            OrderItemId = r.GetInt32(0),
                            OrderId = r.GetInt32(1),
                            ProductName = r.GetString(2),
                            Quantity = r.GetInt32(3),
                            UnitPrice = r.GetDecimal(4)
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>Adds a line at the product's current price; merges into an existing line for the same product.</summary>
        public void AddItem(int orderId, int productId, int quantity)
        {
            using (var conn = Db.Open())
            using (var tx = conn.BeginTransaction())
            {
                EnsureNotShipped(conn, tx, orderId, "Items cannot be changed on a shipped order.");
                using (var cmd = new SqlCommand(
                    "UPDATE dbo.OrderItems SET Quantity = Quantity + @q WHERE OrderId = @o AND ProductId = @p", conn, tx))
                {
                    cmd.Parameters.Add("@q", SqlDbType.Int).Value = quantity;
                    cmd.Parameters.Add("@o", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@p", SqlDbType.Int).Value = productId;
                    if (cmd.ExecuteNonQuery() == 0)
                    {
                        cmd.CommandText =
                            "INSERT dbo.OrderItems(OrderId, ProductId, Quantity, UnitPrice) " +
                            "SELECT @o, ProductId, @q, Price FROM dbo.Products WHERE ProductId = @p";
                        if (cmd.ExecuteNonQuery() == 0) throw new InvalidOperationException("Product not found.");
                    }
                }
                tx.Commit();
            }
        }

        public void RemoveItem(int orderId, int orderItemId)
        {
            using (var conn = Db.Open())
            using (var tx = conn.BeginTransaction())
            {
                EnsureNotShipped(conn, tx, orderId, "Items cannot be changed on a shipped order.");
                using (var cmd = new SqlCommand("DELETE FROM dbo.OrderItems WHERE OrderItemId = @i AND OrderId = @o", conn, tx))
                {
                    cmd.Parameters.Add("@i", SqlDbType.Int).Value = orderItemId;
                    cmd.Parameters.Add("@o", SqlDbType.Int).Value = orderId;
                    cmd.ExecuteNonQuery();
                }
                tx.Commit();
            }
        }

        public void SetStatus(int orderId, string status)
        {
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand("UPDATE dbo.Orders SET Status = @s WHERE OrderId = @id", conn))
            {
                cmd.Parameters.AddWithValue("@s", status);
                cmd.Parameters.AddWithValue("@id", orderId);
                cmd.ExecuteNonQuery();
            }
        }

        private static Order MapOrder(SqlDataReader r)
        {
            return new Order
            {
                OrderId = r.GetInt32(0),
                CustomerId = r.GetInt32(1),
                CustomerName = r.GetString(2),
                OrderDate = r.GetDateTime(3),
                Status = r.GetString(4),
                Total = r.GetDecimal(5),
                LinkedCount = r.GetInt32(6)
            };
        }

        private static void AddOrderParameters(SqlCommand cmd, Order order)
        {
            cmd.Parameters.Add("@c", SqlDbType.Int).Value = order.CustomerId;
            cmd.Parameters.Add("@d", SqlDbType.DateTime).Value = order.OrderDate;
            cmd.Parameters.Add("@s", SqlDbType.NVarChar, 20).Value = order.Status;
        }

        private static void Execute(SqlConnection conn, SqlTransaction tx, string sql, int id)
        {
            using (var cmd = new SqlCommand(sql, conn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = id;
                cmd.ExecuteNonQuery();
            }
        }

        private static void EnsureNotShipped(SqlConnection conn, SqlTransaction tx, int orderId, string message)
        {
            using (var cmd = new SqlCommand("SELECT Status FROM dbo.Orders WHERE OrderId = @id", conn, tx))
            {
                cmd.Parameters.Add("@id", SqlDbType.Int).Value = orderId;
                var status = cmd.ExecuteScalar() as string;
                if (status == null) throw new InvalidOperationException("Order no longer exists.");
                if (status == OrderStatuses.Shipped) throw new InvalidOperationException(message);
            }
        }
    }
}
