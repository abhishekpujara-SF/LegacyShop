using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using LegacyShop.Web.Data;
using LegacyShop.Web.Models;

namespace LegacyShop.Web
{
    public partial class CustomerOrdersPage : Page
    {
        private readonly OrderCustomerRepository _links = new OrderCustomerRepository();

        private int CustomerId
        {
            get { int id; return int.TryParse(Request.QueryString["customerId"], out id) ? id : 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var customer = new CustomerRepository().GetById(CustomerId);
            if (customer == null) { Response.Redirect("~/Customers.aspx"); return; }
            lblName.Text = Server.HtmlEncode(customer.FullName);
            lblEmail.Text = Server.HtmlEncode(customer.Email);

            if (IsPostBack) return;
            foreach (var r in OrderRoles.Assignable) ddlNewRole.Items.Add(r);
            BindAll();
        }

        private void BindAll()
        {
            gvLinks.DataSource = _links.GetLinksForCustomer(CustomerId);
            gvLinks.DataBind();

            ddlOrder.Items.Clear();
            var available = _links.GetAvailableOrders(CustomerId);
            foreach (var o in available)
                ddlOrder.Items.Add(new ListItem(
                    "#" + o.OrderId + " - " + o.OrderDate.ToString("d") + " - " + o.Status + " (owner " + o.CustomerName + ")",
                    o.OrderId.ToString()));
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
            int orderId = int.Parse((string)e.CommandArgument);
            try
            {
                if (e.CommandName == "SaveRole")
                {
                    var row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                    var role = ((DropDownList)row.FindControl("ddlRole")).SelectedValue;
                    _links.UpdateRole(orderId, CustomerId, role);
                    ShowMessage("Role updated.", true);
                }
                else
                {
                    _links.Remove(orderId, CustomerId);
                    ShowMessage("Customer removed from order #" + orderId + ".", true);
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
            if (string.IsNullOrEmpty(ddlOrder.SelectedValue)) return;
            try
            {
                _links.Add(int.Parse(ddlOrder.SelectedValue), CustomerId, ddlNewRole.SelectedValue);
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
