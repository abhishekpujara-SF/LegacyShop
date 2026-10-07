<%@ Page Title="Order customers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="OrderCustomers.aspx.cs" Inherits="LegacyShop.Web.OrderCustomersPage" %>
<asp:Content ID="Main1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Customers on order #<asp:Label ID="lblOrderId" runat="server" /></h2>
    <p>
        Date <asp:Label ID="lblDate" runat="server" /> &middot;
        Status <asp:Label ID="lblStatus" runat="server" /> &middot;
        Total <asp:Label ID="lblTotal" runat="server" />
    </p>
    <asp:GridView ID="gvLinks" runat="server" AutoGenerateColumns="false" CssClass="table table-striped"
        OnRowDataBound="gvLinks_RowDataBound" OnRowCommand="gvLinks_RowCommand" EmptyDataText="No customers.">
        <Columns>
            <asp:HyperLinkField HeaderText="Customer" DataTextField="CustomerName"
                DataNavigateUrlFields="CustomerId" DataNavigateUrlFormatString="CustomerOrders.aspx?customerId={0}" />
            <asp:BoundField DataField="Email" HeaderText="Email" />
            <asp:TemplateField HeaderText="Role">
                <ItemTemplate>
                    <asp:Label ID="lblRole" runat="server" Text='<%# Eval("Role") %>' Visible='<%# (bool)Eval("IsOwner") %>' />
                    <asp:DropDownList ID="ddlRole" runat="server" CssClass="form-control input-sm" Width="140" Visible='<%# !(bool)Eval("IsOwner") %>' />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:BoundField DataField="AddedOn" HeaderText="Added" DataFormatString="{0:d}" />
            <asp:TemplateField>
                <ItemTemplate>
                    <asp:LinkButton runat="server" CommandName="SaveRole" CommandArgument='<%# Eval("CustomerId") %>' Visible='<%# !(bool)Eval("IsOwner") %>'>Save role</asp:LinkButton>
                    <asp:LinkButton runat="server" CommandName="RemoveLink" CommandArgument='<%# Eval("CustomerId") %>' Visible='<%# !(bool)Eval("IsOwner") %>'>Remove</asp:LinkButton>
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    <p class="text-muted">The owning customer is changed on the <asp:HyperLink ID="lnkEdit" runat="server">order page</asp:HyperLink>.</p>

    <h3>Add an existing customer</h3>
    <asp:Panel ID="pnlAdd" runat="server" CssClass="form-inline">
        <asp:DropDownList ID="ddlCustomer" runat="server" CssClass="form-control" />
        <asp:DropDownList ID="ddlNewRole" runat="server" CssClass="form-control" />
        <asp:Button ID="btnAdd" runat="server" Text="Add to order" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
    </asp:Panel>
    <asp:Label ID="lblNone" runat="server" Text="Every customer is already on this order." Visible="false" CssClass="text-muted" />
    <p><asp:Label ID="lblMessage" runat="server" /></p>
    <p><a href="Orders.aspx" class="btn btn-default">Back to orders</a> <a href="Relationships.aspx" class="btn btn-default">All relationships</a></p>
</asp:Content>
