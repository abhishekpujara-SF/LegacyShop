using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using LegacyShop.Web.Infrastructure;

namespace LegacyShop.Web
{
    public partial class LoginPage : Page
    {
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            if (!AuthHelper.ValidateUser(txtUser.Text.Trim(), txtPass.Text))
            {
                lblError.Text = "Invalid user name or password.";
                return;
            }
            AuthHelper.SignIn(txtUser.Text.Trim(), chkRemember.Checked);
            var returnUrl = Request.QueryString["ReturnUrl"];
            Response.Redirect(string.IsNullOrEmpty(returnUrl) ? FormsAuthentication.DefaultUrl : returnUrl);
        }
    }
}
