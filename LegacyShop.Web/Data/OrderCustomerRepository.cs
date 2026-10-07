using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using LegacyShop.Web.Models;

namespace LegacyShop.Web.Data
{
    /// <summary>
    /// The customer/order relationship. Each order has one owning customer (Orders.CustomerId, role "Owner")
    /// plus any number of additional participants in dbo.OrderCustomers. Both directions
    /// ("customers on an order", "orders of a customer") read the same union.
    /// </summary>
    public class OrderCustomerRepository
    {
        private const string LinkSql =
            "SELECT o.OrderId, c.CustomerId, c.FullName AS CustomerName, c.Email, o.OrderDate, o.Status AS OrderStatus, " +
            "ISNULL(t.Total, 0) AS Total, 'Owner' AS Role, CAST(1 AS BIT) AS IsOwner, o.OrderDate AS AddedOn " +
            "FROM dbo.Orders o JOIN dbo.Customers c ON c.CustomerId = o.CustomerId " +
            "LEFT JOIN (SELECT OrderId, SUM(Quantity * UnitPrice) AS Total FROM dbo.OrderItems GROUP BY OrderId) t ON t.OrderId = o.OrderId " +
            "UNION ALL " +
            "SELECT o.OrderId, c.CustomerId, c.FullName, c.Email, o.OrderDate, o.Status, " +
            "ISNULL(t.Total, 0), oc.Role, CAST(0 AS BIT), oc.AddedOn " +
            "FROM dbo.OrderCustomers oc JOIN dbo.Orders o ON o.OrderId = oc.OrderId JOIN dbo.Customers c ON c.CustomerId = oc.CustomerId " +
            "LEFT JOIN (SELECT OrderId, SUM(Quantity * UnitPrice) AS Total FROM dbo.OrderItems GROUP BY OrderId) t ON t.OrderId = o.OrderId";

        public List<OrderLink> GetLinksForOrder(int orderId)
        {
            return Query("WHERE x.OrderId = @order", "ORDER BY x.IsOwner DESC, x.CustomerName", orderId, null);
        }

        public List<OrderLink> GetLinksForCustomer(int customerId)
        {
            return Query("WHERE x.CustomerId = @customer", "ORDER BY x.OrderDate DESC, x.OrderId DESC", null, customerId);
        }

        /// <summary>All relationships, optionally narrowed to one customer, one role (or "Owner") and/or hiding owner rows.</summary>
        public List<OrderLink> GetAllLinks(int? customerId, string role, bool includeOwners)
        {
            var where = "WHERE 1 = 1";
            if (customerId.HasValue) where += " AND x.CustomerId = @customer";
            if (!string.IsNullOrEmpty(role)) where += " AND x.Role = @role";
            if (!includeOwners) where += " AND x.IsOwner = 0";
            return Query(where, "ORDER BY x.OrderId DESC, x.IsOwner DESC, x.CustomerName", null, customerId, role);
        }

        /// <summary>Customers that can still be added to the order (not the owner, not already linked).</summary>
        public List<Customer> GetAvailableCustomers(int orderId)
        {
            var list = new List<Customer>();
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand(
                "SELECT c.CustomerId, c.FullName, c.Email, c.City, c.CreatedOn FROM dbo.Customers c " +
                "WHERE c.CustomerId NOT IN (SELECT CustomerId FROM dbo.Orders WHERE OrderId = @o) " +
                "AND c.CustomerId NOT IN (SELECT CustomerId FROM dbo.OrderCustomers WHERE OrderId = @o) ORDER BY c.FullName", conn))
            {
                cmd.Parameters.Add("@o", SqlDbType.Int).Value = orderId;
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new Customer
                        {
                            CustomerId = r.GetInt32(0),
                            FullName = r.GetString(1),
                            Email = r.GetString(2),
                            City = r.IsDBNull(3) ? null : r.GetString(3),
                            CreatedOn = r.GetDateTime(4)
                        });
                    }
                }
            }
            return list;
        }

        /// <summary>Orders the customer can still be added to (not owned by them, not already linked).</summary>
        public List<Order> GetAvailableOrders(int customerId)
        {
            var list = new List<Order>();
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand(
                "SELECT o.OrderId, o.CustomerId, c.FullName, o.OrderDate, o.Status FROM dbo.Orders o " +
                "JOIN dbo.Customers c ON c.CustomerId = o.CustomerId " +
                "WHERE o.CustomerId <> @c AND o.OrderId NOT IN (SELECT OrderId FROM dbo.OrderCustomers WHERE CustomerId = @c) " +
                "ORDER BY o.OrderDate DESC, o.OrderId DESC", conn))
            {
                cmd.Parameters.Add("@c", SqlDbType.Int).Value = customerId;
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
                            Status = r.GetString(4)
                        });
                    }
                }
            }
            return list;
        }

        public void Add(int orderId, int customerId, string role)
        {
            ValidateRole(role);
            using (var conn = Db.Open())
            using (var tx = conn.BeginTransaction())
            {
                using (var cmd = new SqlCommand("SELECT CustomerId FROM dbo.Orders WHERE OrderId = @o", conn, tx))
                {
                    cmd.Parameters.Add("@o", SqlDbType.Int).Value = orderId;
                    var owner = cmd.ExecuteScalar();
                    if (owner == null) throw new InvalidOperationException("Order no longer exists.");
                    if ((int)owner == customerId) throw new InvalidOperationException("That customer already owns this order.");
                }
                using (var cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.OrderCustomers WHERE OrderId = @o AND CustomerId = @c", conn, tx))
                {
                    cmd.Parameters.Add("@o", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@c", SqlDbType.Int).Value = customerId;
                    if ((int)cmd.ExecuteScalar() > 0) throw new InvalidOperationException("That customer is already on this order.");
                }
                using (var cmd = new SqlCommand(
                    "INSERT dbo.OrderCustomers(OrderId, CustomerId, Role) VALUES(@o, @c, @r)", conn, tx))
                {
                    cmd.Parameters.Add("@o", SqlDbType.Int).Value = orderId;
                    cmd.Parameters.Add("@c", SqlDbType.Int).Value = customerId;
                    cmd.Parameters.Add("@r", SqlDbType.NVarChar, 20).Value = role;
                    try { cmd.ExecuteNonQuery(); }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 547) throw new InvalidOperationException("Customer or order no longer exists.");
                        throw;
                    }
                }
                tx.Commit();
            }
        }

        public void UpdateRole(int orderId, int customerId, string role)
        {
            ValidateRole(role);
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand(
                "UPDATE dbo.OrderCustomers SET Role = @r WHERE OrderId = @o AND CustomerId = @c", conn))
            {
                cmd.Parameters.Add("@o", SqlDbType.Int).Value = orderId;
                cmd.Parameters.Add("@c", SqlDbType.Int).Value = customerId;
                cmd.Parameters.Add("@r", SqlDbType.NVarChar, 20).Value = role;
                if (cmd.ExecuteNonQuery() == 0) throw new InvalidOperationException("That link no longer exists.");
            }
        }

        public void Remove(int orderId, int customerId)
        {
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand(
                "DELETE FROM dbo.OrderCustomers WHERE OrderId = @o AND CustomerId = @c", conn))
            {
                cmd.Parameters.Add("@o", SqlDbType.Int).Value = orderId;
                cmd.Parameters.Add("@c", SqlDbType.Int).Value = customerId;
                if (cmd.ExecuteNonQuery() == 0)
                    throw new InvalidOperationException("That link no longer exists (the owning customer cannot be removed here).");
            }
        }

        private static void ValidateRole(string role)
        {
            if (Array.IndexOf(OrderRoles.Assignable, role) < 0)
                throw new InvalidOperationException("Unknown role '" + role + "'.");
        }

        private static List<OrderLink> Query(string where, string orderBy, int? orderId, int? customerId, string role = null)
        {
            var list = new List<OrderLink>();
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand("SELECT * FROM (" + LinkSql + ") x " + where + " " + orderBy, conn))
            {
                if (orderId.HasValue) cmd.Parameters.Add("@order", SqlDbType.Int).Value = orderId.Value;
                if (customerId.HasValue) cmd.Parameters.Add("@customer", SqlDbType.Int).Value = customerId.Value;
                if (!string.IsNullOrEmpty(role)) cmd.Parameters.Add("@role", SqlDbType.NVarChar, 20).Value = role;
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read())
                    {
                        list.Add(new OrderLink
                        {
                            OrderId = r.GetInt32(0),
                            CustomerId = r.GetInt32(1),
                            CustomerName = r.GetString(2),
                            Email = r.GetString(3),
                            OrderDate = r.GetDateTime(4),
                            OrderStatus = r.GetString(5),
                            OrderTotal = r.GetDecimal(6),
                            Role = r.GetString(7),
                            IsOwner = r.GetBoolean(8),
                            AddedOn = r.GetDateTime(9)
                        });
                    }
                }
            }
            return list;
        }
    }
}
