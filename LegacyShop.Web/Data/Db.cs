using System.Configuration;
using System.Data.SqlClient;

namespace LegacyShop.Web.Data
{
    /// <summary>Static connection helper over System.Data.SqlClient and ConfigurationManager.</summary>
    public static class Db
    {
        public static string ConnectionString
        {
            get { return ConfigurationManager.ConnectionStrings["LegacyShopDb"].ConnectionString; }
        }

        public static SqlConnection Open()
        {
            var conn = new SqlConnection(ConnectionString);
            conn.Open();
            return conn;
        }

        public static int LowStockThreshold
        {
            get { return int.Parse(ConfigurationManager.AppSettings["LowStockThreshold"]); }
        }
    }
}
