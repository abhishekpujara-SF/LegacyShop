<%@ Page Title="Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="LegacyShop.Web.DefaultPage" %>
<asp:Content ID="Title1" ContentPlaceHolderID="TitleContent" runat="server">Dashboard</asp:Content>
<asp:Content ID="Main1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Dashboard</h2>
    <div class="row stats">
        <div class="col-md-4"><div class="stat-box"><span class="stat-value"><asp:Label ID="lblProducts" runat="server" /></span><br />Products</div></div>
        <div class="col-md-4"><div class="stat-box"><span class="stat-value"><asp:Label ID="lblCustomers" runat="server" /></span><br />Customers</div></div>
        <div class="col-md-4"><div class="stat-box"><span class="stat-value"><asp:Label ID="lblOpenOrders" runat="server" /></span><br />Open orders</div></div>
    </div>
    <p>Visit started: <asp:Label ID="lblVisit" runat="server" /></p>
    <p><a href="ExportProducts.ashx" class="btn btn-default">Export products (CSV)</a>
       <a href="api/productsapi" class="btn btn-default">Products API (JSON)</a></p>
</asp:Content>
