using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using LegacyShop.Web.Data;

namespace LegacyShop.Web
{
    /// <summary>Generic handler: GET /ExportProducts.ashx returns products as CSV.</summary>
    public class ExportProducts : IHttpHandler
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            int max = int.Parse(ConfigurationManager.AppSettings["ExportMaxRows"]);
            var rows = new ProductRepository().GetAll(null).Take(max);

            var sb = new StringBuilder("Sku,Name,Category,Price,Stock\r\n");
            foreach (var p in rows)
                sb.AppendFormat("{0},\"{1}\",{2},{3:0.00},{4}\r\n", p.Sku, p.Name.Replace("\"", "\"\""), p.CategoryName, p.Price, p.Stock);

            context.Response.ContentType = "text/csv";
            context.Response.AddHeader("Content-Disposition", "attachment; filename=products.csv");
            context.Response.Write(sb.ToString());
        }
    }
}
