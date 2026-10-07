using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using LegacyShop.Web.Data;
using LegacyShop.Web.Models;

namespace LegacyShop.Web
{
    public partial class OrderCustomersPage : Page
    {
        private readonly OrderCustomerRepository _links = new OrderCustomerRepository();

        private int OrderId
        {
            get { int id; return int.TryParse(Request.QueryString["orderId"], out id) ? id : 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var order = new OrderRepository().GetOrder(OrderId);
            if (order == null) { Response.Redirect("~/Orders.aspx"); return; }
            lblOrderId.Text = order.OrderId.ToString();
            lblDate.Text = order.OrderDate.ToString("d");
            lblStatus.Text = order.Status;
            lblTotal.Text = order.Total.ToString("C");
            lnkEdit.NavigateUrl = "~/OrderEdit.aspx?id=" + order.OrderId;

            if (IsPostBack) return;
            foreach (var r in OrderRoles.Assignable) ddlNewRole.Items.Add(r);
            BindAll();
        }

        private void BindAll()
        {
            gvLinks.DataSource = _links.GetLinksForOrder(OrderId);
            gvLinks.DataBind();

            var available = _links.GetAvailableCustomers(OrderId);
            ddlCustomer.DataSource = available;
            ddlCustomer.DataTextField = "FullName";
            ddlCustomer.DataValueField = "CustomerId";
            ddlCustomer.DataBind();
            pnlAdd.Visible = available.Count > 0;
            lblNone.Visible = available.Count == 0;
        }

        private void ShowMessage(string text, bool success)
        {
            lblMessage.Text = Server.HtmlEncode(text);
            lblMessage.CssClass = success ? "text-success" : "text-danger";
        }

        protected void gvLinks_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            var link = (OrderLink)e.Row.DataItem;
            var ddl = (DropDownList)e.Row.FindControl("ddlRole");
            foreach (var r in OrderRoles.Assignable) ddl.Items.Add(r);
            if (!link.IsOwner) ddl.SelectedValue = link.Role;
        }

        protected void gvLinks_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "SaveRole" && e.CommandName != "RemoveLink") return;
            int customerId = int.Parse((string)e.CommandArgument);
            try
            {
                if (e.CommandName == "SaveRole")
                {
                    var row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    var role = ((DropDownList)row.FindControl("ddlRole")).SelectedValue;
                    _links.UpdateRole(OrderId, customerId, role);
                    ShowMessage("Role updated.", true);
                }
                else
                {
                    _links.Remove(OrderId, customerId);
                    ShowMessage("Customer removed from the order.", true);
                }
            }
            catch (InvalidOperationException ex)
            {
                ShowMessage(ex.Message, false);
            }
            BindAll();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ddlCustomer.SelectedValue)) return;
            try
            {
                _links.Add(OrderId, int.Parse(ddlCustomer.SelectedValue), ddlNewRole.SelectedValue);
                ShowMessage("Customer added to the order.", true);
            }
            catch (InvalidOperationException ex)
            {
                ShowMessage(ex.Message, false);
            }
            BindAll();
        }
    }
}
