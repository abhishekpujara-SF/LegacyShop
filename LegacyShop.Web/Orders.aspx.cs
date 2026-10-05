using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using LegacyShop.Web.Data;

namespace LegacyShop.Web
{
    public partial class OrdersPage : Page
    {
        private readonly OrderRepository _repo = new OrderRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) BindOrders();
        }

        private void BindOrders()
        {
            rptOrders.DataSource = _repo.GetOrders();
            rptOrders.DataBind();
        }

        protected void rptOrders_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int orderId = int.Parse((string)e.CommandArgument);
            if (e.CommandName == "Items")
            {
                lblOrderId.Text = orderId.ToString();
                gvItems.DataSource = _repo.GetItems(orderId);
                gvItems.DataBind();
                pnlItems.Visible = true;
            }
            else if (e.CommandName == "Ship")
            {
                _repo.SetStatus(orderId, "Shipped");
                BindOrders();
            }
        }
    }
}
