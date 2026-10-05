using System;
using System.Web.UI;
using LegacyShop.Web.Data;

namespace LegacyShop.Web
{
    public partial class DefaultPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblProducts.Text = new ProductRepository().Count().ToString();
            var orders = new OrderRepository();
            lblCustomers.Text = orders.CountCustomers().ToString();
            lblOpenOrders.Text = orders.CountOpenOrders().ToString();
            lblVisit.Text = Convert.ToString(Session["VisitStart"]);
        }
    }
}
