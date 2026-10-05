using System;
using System.Web.UI;
using LegacyShop.Web.Infrastructure;

namespace LegacyShop.Web
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var user = AuthHelper.CurrentUser;
            lblUser.Text = user == null ? "" : "Hello, " + Server.HtmlEncode(user) + " ";
            lnkLogout.Visible = user != null;
            lnkLogin.Visible = user == null;
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            AuthHelper.SignOut();
            Response.Redirect("~/Default.aspx");
        }
    }
}
