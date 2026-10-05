<%@ Page Title="Orders" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Orders.aspx.cs" Inherits="LegacyShop.Web.OrdersPage" %>
<asp:Content ID="Main1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Orders</h2>
    <asp:Repeater ID="rptOrders" runat="server" OnItemCommand="rptOrders_ItemCommand">
        <HeaderTemplate><table class="table table-striped"><tr><th>#</th><th>Customer</th><th>Date</th><th>Status</th><th>Total</th><th></th></tr></HeaderTemplate>
        <ItemTemplate>
            <tr>
                <td><%# Eval("OrderId") %></td>
                <td><%# Eval("CustomerName") %></td>
                <td><%# Eval("OrderDate", "{0:d}") %></td>
                <td><%# Eval("Status") %></td>
                <td><%# Eval("Total", "{0:C}") %></td>
                <td>
                    <asp:LinkButton runat="server" CommandName="Items" CommandArgument='<%# Eval("OrderId") %>'>Items</asp:LinkButton>
                    <asp:LinkButton runat="server" CommandName="Ship" CommandArgument='<%# Eval("OrderId") %>' Visible='<%# (string)Eval("Status") != "Shipped" %>'>Mark shipped</asp:LinkButton>
                </td>
            </tr>
        </ItemTemplate>
        <FooterTemplate></table></FooterTemplate>
    </asp:Repeater>
    <asp:Panel ID="pnlItems" runat="server" Visible="false">
        <h3>Items for order #<asp:Label ID="lblOrderId" runat="server" /></h3>
        <asp:GridView ID="gvItems" runat="server" AutoGenerateColumns="false" CssClass="table">
            <Columns>
                <asp:BoundField DataField="ProductName" HeaderText="Product" />
                <asp:BoundField DataField="Quantity" HeaderText="Qty" />
                <asp:BoundField DataField="UnitPrice" HeaderText="Unit price" DataFormatString="{0:C}" />
                <asp:BoundField DataField="LineTotal" HeaderText="Line total" DataFormatString="{0:C}" />
            </Columns>
        </asp:GridView>
    </asp:Panel>
</asp:Content>
