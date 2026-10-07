<%@ Page Title="Relationships" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Relationships.aspx.cs" Inherits="LegacyShop.Web.RelationshipsPage" %>
<asp:Content ID="Main1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Customer / order relationships</h2>
    <p class="form-inline">
        Customer: <asp:DropDownList ID="ddlCustomer" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="Filter_Changed" />
        Role: <asp:DropDownList ID="ddlRole" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="Filter_Changed" />
        <asp:CheckBox ID="chkOwners" runat="server" Text="Include owners" Checked="true" AutoPostBack="true" OnCheckedChanged="Filter_Changed" />
    </p>
    <asp:GridView ID="gvLinks" runat="server" AutoGenerateColumns="false" CssClass="table table-striped" EmptyDataText="No relationships match.">
        <Columns>
            <asp:HyperLinkField HeaderText="Order" DataTextField="OrderId" DataTextFormatString="#{0}"
                DataNavigateUrlFields="OrderId" DataNavigateUrlFormatString="OrderCustomers.aspx?orderId={0}" />
            <asp:BoundField DataField="OrderDate" HeaderText="Date" DataFormatString="{0:d}" />
            <asp:BoundField DataField="OrderStatus" HeaderText="Status" />
            <asp:HyperLinkField HeaderText="Customer" DataTextField="CustomerName"
                DataNavigateUrlFields="CustomerId" DataNavigateUrlFormatString="CustomerOrders.aspx?customerId={0}" />
            <asp:BoundField DataField="Role" HeaderText="Role" />
            <asp:BoundField DataField="OrderTotal" HeaderText="Order total" DataFormatString="{0:C}" />
        </Columns>
    </asp:GridView>
    <p><asp:Label ID="lblSummary" runat="server" CssClass="text-muted" /></p>
</asp:Content>
