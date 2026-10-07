<%@ Page Title="Edit order" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderEdit.aspx.cs" Inherits="LegacyShop.Web.OrderEditPage" %>
<asp:Content ID="Main1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Literal ID="litHeading" runat="server" Text="New order" /></h2>
    <asp:ValidationSummary ID="vsSummary" runat="server" CssClass="text-danger" />
    <div class="form-horizontal">
        <div class="form-group"><label class="col-sm-2 control-label">Owning customer</label>
            <div class="col-sm-4"><asp:DropDownList ID="ddlCustomer" runat="server" CssClass="form-control" /></div></div>
        <div class="form-group"><label class="col-sm-2 control-label">Order date</label>
            <div class="col-sm-4"><asp:TextBox ID="txtDate" runat="server" TextMode="Date" CssClass="form-control" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDate" ErrorMessage="Order date is required." Display="None" /></div></div>
        <div class="form-group"><label class="col-sm-2 control-label">Status</label>
            <div class="col-sm-4"><asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control" /></div></div>
        <div class="form-group"><div class="col-sm-offset-2 col-sm-4">
            <asp:Button ID="btnSave" runat="server" Text="Save" CssClass="btn btn-primary" OnClick="btnSave_Click" />
            <a href="Orders.aspx" class="btn btn-default">Back to orders</a></div></div>
    </div>
    <asp:Label ID="lblMessage" runat="server" />

    <asp:Panel ID="pnlItems" runat="server" Visible="false">
        <h3>Items</h3>
        <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="false" DataKeyNames="OrderItemId"
            CssClass="table" OnRowCommand="gvItems_RowCommand" EmptyDataText="No items yet.">
            <Columns>
                <asp:BoundField DataField="ProductName" HeaderText="Product" />
                <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Unit price" DataFormatString="{0:C}" />
                <asp:BoundField DataField="LineTotal" HeaderText="Line total" DataFormatString="{0:C}" />
                <asp:ButtonField CommandName="RemoveItem" Text="Remove" ButtonType="Link" />
            </Columns>
        </asp:GridView>
        <asp:Panel ID="pnlAddItem" runat="server" CssClass="form-inline">
            <asp:DropDownList ID="ddlProduct" runat="server" CssClass="form-control" />
            <asp:TextBox ID="txtQty" runat="server" Text="1" Width="70" CssClass="form-control" />
            <asp:Button ID="btnAddItem" runat="server" Text="Add item" CssClass="btn btn-default" ValidationGroup="item" OnClick="btnAddItem_Click" />
            <asp:RangeValidator runat="server" ControlToValidate="txtQty" Type="Integer" MinimumValue="1" MaximumValue="1000"
                ErrorMessage="Quantity must be a whole number from 1 to 1000." ValidationGroup="item" CssClass="text-danger" Display="Dynamic" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtQty" ErrorMessage="Quantity is required."
                ValidationGroup="item" CssClass="text-danger" Display="Dynamic" />
        </asp:Panel>
        <p><asp:HyperLink ID="lnkCustomers" runat="server">Manage customers on this order</asp:HyperLink></p>
    </asp:Panel>
</asp:Content>
