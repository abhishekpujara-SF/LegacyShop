<%@ Page Title="Products" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Products.aspx.cs" Inherits="LegacyShop.Web.ProductsPage" %>
<asp:Content ID="Title1" ContentPlaceHolderID="TitleContent" runat="server">Products</asp:Content>
<asp:Content ID="Main1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Products</h2>
    <p>
        Category:
        <asp:DropDownList ID="ddlCategory" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlCategory_SelectedIndexChanged" />
        <a href="ProductEdit.aspx" class="btn btn-primary">Add product</a>
    </p>
    <asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="false" DataKeyNames="ProductId"
        CssClass="table table-striped" OnRowCommand="gvProducts_RowCommand" EmptyDataText="No products.">
        <Columns>
            <asp:BoundField DataField="Sku" HeaderText="SKU" />
            <asp:BoundField DataField="Name" HeaderText="Name" />
            <asp:BoundField DataField="CategoryName" HeaderText="Category" />
            <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
            <asp:BoundField DataField="Stock" HeaderText="Stock" />
            <asp:CheckBoxField DataField="IsActive" HeaderText="Active" />
            <asp:HyperLinkField Text="Edit" DataNavigateUrlFields="ProductId" DataNavigateUrlFormatString="ProductEdit.aspx?id={0}" />
            <asp:ButtonField CommandName="DeleteProduct" Text="Delete" ButtonType="Link" />
        </Columns>
    </asp:GridView>
    <asp:Label ID="lblMessage" runat="server" CssClass="text-success" />
</asp:Content>
