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
            else if (e.CommandName == "DeleteOrder")
            {
                if (!User.Identity.IsAuthenticated)
                {
                    Response.Redirect("~/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
                    return;
                }
                try
                {
                    _repo.DeleteOrder(orderId);
                    pnlItems.Visible = false;
                    lblMessage.Text = "Order #" + orderId + " deleted.";
                    lblMessage.CssClass = "text-success";
                }
                catch (InvalidOperationException ex)
                {
                    lblMessage.Text = Server.HtmlEncode(ex.Message);
                    lblMessage.CssClass = "text-danger";
                }
                BindOrders();
            }
        }
    }
}
