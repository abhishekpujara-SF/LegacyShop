using System;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using LegacyShop.Web.Data;
using LegacyShop.Web.Models;

namespace LegacyShop.Web
{
    public partial class OrderEditPage : Page
    {
        private readonly OrderRepository _orders = new OrderRepository();

        private int OrderId
        {
            get { int id; return int.TryParse(Request.QueryString["id"], out id) ? id : 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            ddlCustomer.DataSource = new CustomerRepository().GetAll();
            ddlCustomer.DataTextField = "FullName";
            ddlCustomer.DataValueField = "CustomerId";
            ddlCustomer.DataBind();
            foreach (var s in OrderStatuses.All) ddlStatus.Items.Add(s);
            txtDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

            if (OrderId == 0) return;
            var order = _orders.GetOrder(OrderId);
            if (order == null) { Response.Redirect("~/Orders.aspx"); return; }

            litHeading.Text = "Edit order #" + order.OrderId;
            ddlCustomer.SelectedValue = order.CustomerId.ToString();
            txtDate.Text = order.OrderDate.ToString("yyyy-MM-dd");
            ddlStatus.SelectedValue = order.Status;
            lnkCustomers.NavigateUrl = "~/OrderCustomers.aspx?orderId=" + order.OrderId;

            foreach (var p in new ProductRepository().GetAll(null))
                if (p.IsActive) ddlProduct.Items.Add(new ListItem(p.Name + " (" + p.Sku + ") - " + p.Price.ToString("C"), p.ProductId.ToString()));

            if (Request.QueryString["saved"] == "1") ShowMessage("Order created. You can now add items and customers.", true);
            BindItems();
        }

        private void BindItems()
        {
            pnlItems.Visible = true;
            var shipped = ddlStatus.SelectedValue == OrderStatuses.Shipped;
            pnlAddItem.Visible = !shipped;
            gvItems.Columns[4].Visible = !shipped;
            gvItems.DataSource = _orders.GetItems(OrderId);
            gvItems.DataBind();
        }

        private void ShowMessage(string text, bool success)
        {
            lblMessage.Text = Server.HtmlEncode(text);
            lblMessage.CssClass = success ? "text-success" : "text-danger";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            DateTime date;
            if (!DateTime.TryParseExact(txtDate.Text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
            {
                ShowMessage("Enter the order date as yyyy-MM-dd.", false);
                return;
            }
            try
            {
                int id = _orders.SaveOrder(new Order
                {
                    OrderId = OrderId,
                    CustomerId = int.Parse(ddlCustomer.SelectedValue),
                    OrderDate = date,
                    Status = ddlStatus.SelectedValue
                });
                if (OrderId == 0) { Response.Redirect("~/OrderEdit.aspx?id=" + id + "&saved=1"); return; }
                ShowMessage("Order saved.", true);
                BindItems();
            }
            catch (InvalidOperationException ex)
            {
                ShowMessage(ex.Message, false);
            }
        }

        protected void btnAddItem_Click(object sender, EventArgs e)
        {
            Page.Validate("item");
            if (!Page.IsValid) return;
            try
            {
                _orders.AddItem(OrderId, int.Parse(ddlProduct.SelectedValue), int.Parse(txtQty.Text));
                txtQty.Text = "1";
            }
            catch (InvalidOperationException ex)
            {
                ShowMessage(ex.Message, false);
            }
            BindItems();
        }

        protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "RemoveItem") return;
            int itemId = (int)gvItems.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
            try
            {
                _orders.RemoveItem(OrderId, itemId);
            }
            catch (InvalidOperationException ex)
            {
                ShowMessage(ex.Message, false);
            }
            BindItems();
        }
    }
}
