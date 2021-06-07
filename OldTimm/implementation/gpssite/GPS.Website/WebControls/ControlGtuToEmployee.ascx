<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ControlGtuToEmployee.ascx.cs" Inherits="GPS.Website.WebControls.ControlGtuToEmployee" %>

<asp:GridView runat="server" ID="GtuEmployeeGrid" AutoGenerateColumns="false" DataKeyNames="GtuID" width="100%"
    OnRowDataBound="GtuEmployeeGrid_RowDataBound" OnRowCommand="GtuEmployeeGrid_RowCommand" AllowSorting="true"
     OnSorting="GtuEmployeeGrid_Sorting"
    >
    <Columns>
        <asp:TemplateField HeaderText="GTU#" HeaderStyle-HorizontalAlign="Left" SortExpression="UniqueId">
            <ItemTemplate>
                <asp:Label ID="Label1" runat="server" Text='<%# ShowShortGtuNumber( (string)Eval("UniqueId") ) %>'></asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Company">
            <ItemTemplate>
                <asp:DropDownList ID="companyDropDown" runat="server" AutoPostBack="true" 
                DataTextField="Name" DataValueField="ID"
                OnSelectedIndexChanged="CompanyDropDown_SelectedIndexChanged">
                </asp:DropDownList>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Employee">
            <ItemTemplate>
                <asp:DropDownList ID="employeeDropDown" runat="server" AutoPostBack="true"
                DataTextField="FullName" DataValueField="Id"
                OnSelectedIndexChanged="employeeRoleDropDown_SelectedIndexChanged">
                </asp:DropDownList>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Role">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblEmployeeRole">Walker</asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:ImageButton runat="server" ID="btnDisconnect" ImageUrl="~/Images/icons/link_break.png" 
                CommandName="disconnect" CommandArgument='<%# Eval("GtuID") %>' OnClientClick="return confirm('Are you sure you want to remove the assignment from GTU and Employee?')" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
