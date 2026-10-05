<%@ Page Title="Edit product" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductEdit.aspx.cs" Inherits="LegacyShop.Web.ProductEditPage" %>
<asp:Content ID="Title1" ContentPlaceHolderID="TitleContent" runat="server">Edit product</asp:Content>
<asp:Content ID="Main1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Literal ID="litHeading" runat="server" Text="New product" /></h2>
    <asp:ValidationSummary ID="vsSummary" runat="server" CssClass="text-danger" />
    <div class="form-horizontal">
        <div class="form-group"><label class="col-sm-2 control-label">SKU</label>
            <div class="col-sm-4"><asp:TextBox ID="txtSku" runat="server" CssClass="form-control" MaxLength="30" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtSku" ErrorMessage="SKU is required." Display="None" /></div></div>
        <div class="form-group"><label class="col-sm-2 control-label">Name</label>
            <div class="col-sm-4"><asp:TextBox ID="txtName" runat="server" CssClass="form-control" MaxLength="150" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtName" ErrorMessage="Name is required." Display="None" /></div></div>
        <div class="form-group"><label class="col-sm-2 control-label">Category</label>
            <div class="col-sm-4"><asp:DropDownList ID="ddlCategory" runat="server" CssClass="form-control" /></div></div>
        <div class="form-group"><label class="col-sm-2 control-label">Price</label>
            <div class="col-sm-4"><asp:TextBox ID="txtPrice" runat="server" CssClass="form-control" />
                <asp:RangeValidator runat="server" ControlToValidate="txtPrice" Type="Currency" MinimumValue="0" MaximumValue="100000" ErrorMessage="Price must be between 0 and 100000." Display="None" /></div></div>
        <div class="form-group"><label class="col-sm-2 control-label">Stock</label>
            <div class="col-sm-4"><asp:TextBox ID="txtStock" runat="server" CssClass="form-control" />
                <asp:RangeValidator runat="server" ControlToValidate="txtStock" Type="Integer" MinimumValue="0" MaximumValue="1000000" ErrorMessage="Stock must be a whole number." Display="None" /></div></div>
        <div class="form-group"><div class="col-sm-offset-2 col-sm-4">
            <asp:CheckBox ID="chkActive" runat="server" Text="Active" Checked="true" /></div></div>
        <div class="form-group"><div class="col-sm-offset-2 col-sm-4">
            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            <a href="Products.aspx" class="btn btn-default">Cancel</a></div></div>
    </div>
    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" />
</asp:Content>
