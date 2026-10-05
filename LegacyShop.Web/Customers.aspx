<%@ Page Title="Customers" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Customers.aspx.cs" Inherits="LegacyShop.Web.CustomersPage" %>
<asp:Content ID="Title1" ContentPlaceHolderID="TitleContent" runat="server">Customers</asp:Content>
<asp:Content ID="Main1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>Customers</h2>
    <%-- Declarative data access: SqlDataSource bound straight to the connection string, with in-grid edit/delete. --%>
    <asp:SqlDataSource ID="sdsCustomers" runat="server"
        ConnectionString="<%$ ConnectionStrings:LegacyShopDb %>"
        SelectCommand="SELECT CustomerId, FullName, Email, City, CreatedOn FROM dbo.Customers ORDER BY FullName"
        UpdateCommand="UPDATE dbo.Customers SET FullName=@FullName, Email=@Email, City=@City WHERE CustomerId=@CustomerId"
        DeleteCommand="DELETE FROM dbo.Customers WHERE CustomerId=@CustomerId"
        InsertCommand="INSERT dbo.Customers(FullName,Email,City) VALUES(@FullName,@Email,@City)"
        OnDeleted="sdsCustomers_Deleted" />
    <asp:GridView ID="gvCustomers" runat="server" DataSourceID="sdsCustomers" DataKeyNames="CustomerId"
        AutoGenerateColumns="false" AllowSorting="true" CssClass="table table-striped">
        <Columns>
            <asp:BoundField DataField="FullName" HeaderText="Name" SortExpression="FullName" />
            <asp:BoundField DataField="Email" HeaderText="Email" SortExpression="Email" />
            <asp:BoundField DataField="City" HeaderText="City" SortExpression="City" />
            <asp:BoundField DataField="CreatedOn" HeaderText="Created" ReadOnly="true" DataFormatString="{0:d}" />
            <asp:CommandField ShowEditButton="true" ShowDeleteButton="true" />
        </Columns>
    </asp:GridView>
    <h3>Add customer</h3>
    <p>
        <asp:TextBox ID="txtName" runat="server" placeholder="Full name" />
        <asp:TextBox ID="txtEmail" runat="server" placeholder="Email" />
        <asp:TextBox ID="txtCity" runat="server" placeholder="City" />
        <asp:Button ID="btnAdd" runat="server" Text="Add" CssClass="btn btn-primary" OnClick="btnAdd_Click" />
    </p>
    <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" />
</asp:Content>
