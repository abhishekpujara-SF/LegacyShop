using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using LegacyShop.Web.Data;
using LegacyShop.Web.Models;

namespace LegacyShop.Web
{
    public partial class RelationshipsPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            ddlCustomer.DataSource = new CustomerRepository().GetAll();
            ddlCustomer.DataTextField = "FullName";
            ddlCustomer.DataValueField = "CustomerId";
            ddlCustomer.DataBind();
            ddlCustomer.Items.Insert(0, new ListItem("(all)", ""));
            ddlRole.Items.Add(new ListItem("(all)", ""));
            ddlRole.Items.Add(OrderRoles.Owner);
            foreach (var r in OrderRoles.Assignable) ddlRole.Items.Add(r);
            BindGrid();
        }

        private void BindGrid()
        {
            int? customerId = null;
            if (!string.IsNullOrEmpty(ddlCustomer.SelectedValue)) customerId = int.Parse(ddlCustomer.SelectedValue);
            List<OrderLink> links = new OrderCustomerRepository().GetAllLinks(customerId, ddlRole.SelectedValue, chkOwners.Checked);
            gvLinks.DataSource = links;
            gvLinks.DataBind();
            lblSummary.Text = links.Count + " relationship(s) across " +
                links.Select(l => l.OrderId).Distinct().Count() + " order(s) and " +
                links.Select(l => l.CustomerId).Distinct().Count() + " customer(s).";
        }

        protected void Filter_Changed(object sender, EventArgs e)
        {
            BindGrid();
        }
    }
}
