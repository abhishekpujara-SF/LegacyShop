<%@ Page Title="Sign in" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LegacyShop.Web.LoginPage" %>
<asp:Content ID="Title1" ContentPlaceHolderID="TitleContent" runat="server">Sign in</asp:Content>
<asp:Content ID="Main1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Sign in</h2>
    <div class="form-horizontal" style="max-width:420px">
        <div class="form-group"><asp:TextBox ID="txtUser" runat="server" CssClass="form-control" placeholder="User name" /></div>
        <div class="form-group"><asp:TextBox ID="txtPass" runat="server" TextMode="Password" CssClass="form-control" placeholder="Password" /></div>
        <div class="form-group"><asp:CheckBox ID="chkRemember" runat="server" Text="Remember me" /></div>
        <asp:Button ID="btnLogin" runat="server" Text="Sign in" CssClass="btn btn-primary" OnClick="btnLogin_Click" />
        <asp:Label ID="lblError" runat="server" CssClass="text-danger" />
    </div>
</asp:Content>
