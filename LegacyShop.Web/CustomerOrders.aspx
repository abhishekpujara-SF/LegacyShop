<%@ Page Title="Customer orders" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CustomerOrders.aspx.cs" Inherits="LegacyShop.Web.CustomerOrdersPage" %>
<asp:Content ID="Main1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Orders for <asp:Label ID="lblName" runat="server" /></h2>
    <p><asp:Label ID="lblEmail" runat="server" /></p>
    <asp:GridView ID="gvLinks" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
        OnRowDataBound="gvLinks_RowDataBound" OnRowCommand="gvLinks_RowCommand" EmptyDataText="This customer has no orders.">
        <Columns>
            <asp:HyperLinkField HeaderText="Order" DataTextField="OrderId" DataTextFormatString="#{0}"
                DataNavigateUrlFields="OrderId" DataNavigateUrlFormatString="OrderCustomers.aspx?orderId={0}" />
            <asp:BoundField DataField="OrderDate" HeaderText="Date" DataFormatString="{0:d}" />
            <asp:BoundField DataField="OrderStatus" HeaderText="Status" />
            <asp:BoundField DataField="OrderTotal" HeaderText="Total" DataFormatString="{0:C}" />
            <asp:TemplateField HeaderText="Role">
                <ItemTemplate>
                    <asp:Label ID="lblRole" runat="server" Text='<%# Eval("Role") %>' Visible='<%# (bool)Eval("IsOwner") %>' />
                    <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control input-sm" Width="140" Visible='<%# !(bool)Eval("IsOwner") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton runat="server" CommandName="SaveRole" CommandArgument='<%# Eval("OrderId") %>' Visible='<%# !(bool)Eval("IsOwner") %>'>Save role</asp:LinkButton>
                    <asp:LinkButton runat="server" CommandName="RemoveLink" CommandArgument='<%# Eval("OrderId") %>' Visible='<%# !(bool)Eval("IsOwner") %>'>Remove</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <p class="text-muted">Owned orders (role Owner) are edited on the order page; use Remove to take the customer off an order they are only linked to.</p>

    <h3>Add to an existing order</h3>
    <asp:Panel ID="pnlAdd" runat="server" CssClass="form-inline">
        <asp:DropDownList ID="ddlOrder" runat="server" CssClass="form-control" />
        <asp:DropDownList ID="ddlNewRole" runat="server" CssClass="form-control" />
        <asp:Button ID="btnAdd" runat="server" Text="Add to order" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
    </asp:Panel>
    <asp:Label ID="lblNone" runat="server" Text="No other orders to add this customer to." Visible="false" CssClass="text-muted" />
    <p><asp:Label ID="lblMessage" runat="server" /></p>
    <p><a href="Customers.aspx" class="btn btn-default">Back to customers</a> <a href="Relationships.aspx" class="btn btn-default">All relationships</a></p>
</asp:Content>
