using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LegacyShop.Web.Models;

namespace LegacyShop.Web.Data
{
    /// <summary>Customers and orders via raw ADO.NET (SqlCommand / SqlDataReader), no ORM.</summary>
    public class OrderRepository
    {
        public int CountCustomers()
        {
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Customers", conn))
                return (int)cmd.ExecuteScalar();
        }

        public int CountOpenOrders()
        {
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.Orders WHERE Status <> 'Shipped'", conn))
                return (int)cmd.ExecuteScalar();
        }

        public List<Order> GetOrders()
        {
            var list = new List<Order>();
            const string sql =
                "SELECT o.OrderId, o.CustomerId, c.FullName, o.OrderDate, o.Status, " +
                "ISNULL(SUM(i.Quantity * i.UnitPrice), 0) AS Total " +
                "FROM dbo.Orders o JOIN dbo.Customers c ON c.CustomerId = o.CustomerId " +
                "LEFT JOIN dbo.OrderItems i ON i.OrderId = o.OrderId " +
                "GROUP BY o.OrderId, o.CustomerId, c.FullName, o.OrderDate, o.Status ORDER BY o.OrderDate DESC";
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand(sql, conn))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read())
                {
                    list.Add(new Order
                    {
                        OrderId = r.GetInt32(0),
                        CustomerId = r.GetInt32(1),
                        CustomerName = r.GetString(2),
                        OrderDate = r.GetDateTime(3),
                        Status = r.GetString(4),
                        Total = r.GetDecimal(5)
                    });
                }
            }
            return list;
        }

        public List<OrderItem> GetItems(int orderId)
        {
            var list = new List<OrderItem>();
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand(
                "SELECT i.OrderItemId, i.OrderId, p.Name, i.Quantity, i.UnitPrice " +
                "FROM dbo.OrderItems i JOIN dbo.Products p ON p.ProductId = i.ProductId WHERE i.OrderId = @id", conn))
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
    }
}
