using System;
using System.Web.UI;
using LegacyShop.Web.Data;
using LegacyShop.Web.Models;

namespace LegacyShop.Web
{
    public partial class ProductEditPage : Page
    {
        private readonly ProductRepository _repo = new ProductRepository();

        private int ProductId
        {
            get { int id; return int.TryParse(Request.QueryString["id"], out id) ? id : 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            ddlCategory.DataSource = _repo.GetCategories();
            ddlCategory.DataTextField = "Name";
            ddlCategory.DataValueField = "CategoryId";
            ddlCategory.DataBind();

            if (ProductId == 0) return;
            var p = _repo.GetById(ProductId);
            if (p == null) { Response.Redirect("~/Products.aspx"); return; }
            litHeading.Text = "Edit product";
            txtSku.Text = p.Sku;
            txtName.Text = p.Name;
            ddlCategory.SelectedValue = p.CategoryId.ToString();
            txtPrice.Text = p.Price.ToString("0.00");
            txtStock.Text = p.Stock.ToString();
            chkActive.Checked = p.IsActive;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;
            try
            {
                _repo.Save(new Product
                {
                    ProductId = ProductId,
                    CategoryId = int.Parse(ddlCategory.SelectedValue),
                    Sku = txtSku.Text.Trim(),
                    Name = txtName.Text.Trim(),
                    Price = decimal.Parse(txtPrice.Text),
                    Stock = int.Parse(txtStock.Text),
                    IsActive = chkActive.Checked
                });
                Response.Redirect("~/Products.aspx");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                lblMessage.Text = "Could not save (duplicate SKU?): " + Server.HtmlEncode(ex.Message);
            }
        }
    }
}
