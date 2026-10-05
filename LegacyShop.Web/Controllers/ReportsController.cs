using System.Web.Mvc;
using LegacyShop.Web.Data;
using LegacyShop.Web.Models;

namespace LegacyShop.Web.Controllers
{
    /// <summary>MVC 5 controller: low-stock report (Views/Reports/Index.cshtml).</summary>
    public class ReportsController : Controller
    {
        public ActionResult Index()
        {
            var model = new DashboardStats
            {
                LowStock = new ProductRepository().GetLowStock(Db.LowStockThreshold)
            };
            ViewBag.Threshold = Db.LowStockThreshold;
            return View(model);
        }
    }
}
