using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using LegacyShop.Web.Data;

namespace LegacyShop.Web
{
    public partial class ProductsPage : Page
    {
        private readonly ProductRepository _repo = new ProductRepository();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            ddlCategory.DataSource = _repo.GetCategories();
            ddlCategory.DataTextField = "Name";
            ddlCategory.DataValueField = "CategoryId";
            ddlCategory.DataBind();
            ddlCategory.Items.Insert(0, new ListItem("(all)", ""));
            BindGrid();
        }

        private void BindGrid()
        {
            int? categoryId = null;
            if (!string.IsNullOrEmpty(ddlCategory.SelectedValue)) categoryId = int.Parse(ddlCategory.SelectedValue);
            gvProducts.DataSource = _repo.GetAll(categoryId);
            gvProducts.DataBind();
        }

        protected void ddlCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void gvProducts_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "DeleteProduct") return;
            if (!User.Identity.IsAuthenticated)
            {
                Response.Redirect("~/Login.aspx?ReturnUrl=" + Server.UrlEncode(Request.RawUrl));
                return;
            }
            int id = (int)gvProducts.DataKeys[Convert.ToInt32(e.CommandArgument)].Value;
            try
            {
                _repo.Delete(id);
                lblMessage.Text = "Product deleted.";
            }
            catch (System.Data.SqlClient.SqlException)
            {
                lblMessage.Text = "Cannot delete: product is referenced by existing orders.";
            }
            BindGrid();
        }
    }
}
