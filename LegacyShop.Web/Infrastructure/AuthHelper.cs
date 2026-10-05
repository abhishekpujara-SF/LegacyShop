using System;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Security;
using LegacyShop.Web.Data;

namespace LegacyShop.Web.Infrastructure
{
    /// <summary>Forms authentication against dbo.Users using unsalted SHA1 (intentionally legacy).</summary>
    public static class AuthHelper
    {
        public static bool ValidateUser(string userName, string password)
        {
            using (var conn = Db.Open())
            using (var cmd = new SqlCommand("SELECT PasswordHash FROM dbo.Users WHERE UserName = @u", conn))
            {
                cmd.Parameters.AddWithValue("@u", userName);
                var stored = cmd.ExecuteScalar() as string;
                return stored != null && string.Equals(stored.Trim(), Sha1Hex(password), StringComparison.OrdinalIgnoreCase);
            }
        }

        public static void SignIn(string userName, bool persistent)
        {
            FormsAuthentication.SetAuthCookie(userName, persistent);
            HttpContext.Current.Session["UserName"] = userName;
        }

        public static void SignOut()
        {
            FormsAuthentication.SignOut();
            HttpContext.Current.Session.Abandon();
        }

        public static string CurrentUser
        {
            get
            {
                var ctx = HttpContext.Current;
                return ctx != null && ctx.User.Identity.IsAuthenticated ? ctx.User.Identity.Name : null;
            }
        }

        private static string Sha1Hex(string input)
        {
            using (var sha = SHA1.Create())
            {
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input ?? string.Empty));
                var sb = new StringBuilder();
                foreach (var b in bytes) sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }
    }
}
