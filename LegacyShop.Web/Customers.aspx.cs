using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LegacyShop.Web
{
    public partial class CustomersPage : Page
    {
        protected void btnAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblMessage.Text = "Name and email are required.";
                return;
            }
            sdsCustomers.InsertParameters.Clear();
            sdsCustomers.InsertParameters.Add("FullName", txtName.Text.Trim());
            sdsCustomers.InsertParameters.Add("Email", txtEmail.Text.Trim());
            sdsCustomers.InsertParameters.Add("City", txtCity.Text.Trim());
            try
            {
                sdsCustomers.Insert();
                txtName.Text = txtEmail.Text = txtCity.Text = string.Empty;
                lblMessage.Text = string.Empty;
                gvCustomers.DataBind();
            }
            catch (System.Data.SqlClient.SqlException)
            {
                lblMessage.Text = "Could not add customer (duplicate email?).";
            }
        }

        protected void sdsCustomers_Deleted(object sender, SqlDataSourceStatusEventArgs e)
        {
            if (e.Exception != null)
            {
                lblMessage.Text = "Cannot delete: customer owns orders or is linked to orders. Remove those first.";
                e.ExceptionHandled = true;
            }
        }
    }
}
