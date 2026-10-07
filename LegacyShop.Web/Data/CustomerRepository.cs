using System.Collections.Generic;
using System.Data.SqlClient;
using LegacyShop.Web.Models;

namespace LegacyShop.Web.Data
{
    /// <summary>Read access to customers for pickers and headers (editing stays on Customers.aspx via SqlDataSource).</summary>
    public class CustomerRepository
    {
        public List<Customer> GetAll()
        {
            var list = new List<Customer>();
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand("SELECT CustomerId, FullName, Email, City, CreatedOn FROM dbo.Customers ORDER BY FullName", conn))
            using (var r = cmd.ExecuteReader())
            {
                while (r.Read()) list.Add(Map(r));
            }
            return list;
        }

        public Customer GetById(int id)
        {
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand("SELECT CustomerId, FullName, Email, City, CreatedOn FROM dbo.Customers WHERE CustomerId = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", id);
                using (var r = cmd.ExecuteReader())
                    return r.Read() ? Map(r) : null;
            }
        }

        private static Customer Map(SqlDataReader r)
        {
            return new Customer
            {
                CustomerId = r.GetInt32(0),
                FullName = r.GetString(1),
                Email = r.GetString(2),
                City = r.IsDBNull(3) ? null : r.GetString(3),
                CreatedOn = r.GetDateTime(4)
            };
        }
    }
}
