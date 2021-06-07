<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminGtuToEmployeeControl.ascx.cs" Inherits="GPS.Website.WebControls.AdminGtuToEmployeeControl" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajax" %>

<asp:Label runat="server" ID="lblMessage" EnableViewState="false" ForeColor="Red" ></asp:Label>
<asp:Panel runat="server" ID="pnlConnect" Visible="false">
    <table cellpadding="0" cellspacing="4">
        <tr>
            <td>GTU#</td>
            <td>
                <asp:Label runat="server" ID="lblGtuUniqueID" ></asp:Label>
                <input runat="server" id="txtGtuID" type="hidden" />
             </td>
        </tr>
        <tr>
            <td>
                Distributor
            </td>
            <td>
                <asp:DropDownList runat="server" Id="dropDistributor" AutoPostBack="true" 
                OnSelectedIndexChanged="dropDistributor_SelectedIndexChanged">
                    <asp:ListItem Text="Distributor" Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ValidationGroup="mapping" ID="validator1" ControlToValidate="dropDistributor" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>Employee</td>
            <td><asp:DropDownList runat="server" ID="dropEmployee" DataTextField="FullName" DataValueField="Id"
                AutoPostBack="true" OnSelectedIndexChanged="dropEmployee_SelectedIndexChanged"></asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td>Role</td>
            <td><asp:Label runat="server" ID="lblRole"></asp:Label> </td>
        </tr>
        <tr>
            <td>User color</td>
            <td>
                <asp:TextBox runat="server" ID="txtColor"></asp:TextBox>
                <asp:RequiredFieldValidator runat="server" ValidationGroup="mapping" ControlToValidate="txtColor" ErrorMessage="*" Display="Dynamic"></asp:RequiredFieldValidator> 
                <asp:RegularExpressionValidator runat="server" ValidationGroup="mapping" ControlToValidate="txtColor" ErrorMessage="*" Display="Dynamic" ValidationExpression="^[a-fA-F0-9]{6}$" ></asp:RegularExpressionValidator>
                <ajax:ColorPickerExtender runat="server" ID="colorPickerExtender1"
                    TargetControlID="txtColor"
                    OnClientColorSelectionChanged="colorChanged" >
                </ajax:ColorPickerExtender>
            </td>
        </tr>
        <tr>
            <td></td>
            <td>
                <asp:Button runat="server" ID="btnCancelConnect" Text="Cancel" />
                <asp:Button runat="server" ID="btnSave" Text="Save" OnClick="btnSaveMapping_Click" ValidationGroup="mapping" /> 
            </td>
        </tr>
    </table>
</asp:Panel>
<asp:GridView runat="server" ID="GtuEmployeeGrid" AutoGenerateColumns="false" DataKeyNames="GtuID" width="100%"
    OnRowDataBound="GtuEmployeeGrid_RowDataBound" OnRowCommand="GtuEmployeeGrid_RowCommand" AllowSorting="true"
    OnSorting="GtuEmployeeGrid_Sorting"
    >
    <Columns>
        <asp:BoundField DataField="UniqueID" HeaderText="GTU#" SortExpression="UniqueID" />
        <asp:TemplateField HeaderText="Color">
            <ItemTemplate>
                <asp:Panel runat="server" ID="pnlGtuColor" Width="20px" Height="20px"></asp:Panel>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Company">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblDistributorID" style="display:none"><%# Eval("CompanyId")%>  </asp:Label>
                <asp:Label runat="server" ID="lblDistributorName"> </asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Employee">
            <ItemTemplate>
                <input type="hidden" runat="server" id="rowEmployeeID" value='<%# Eval("UserID") %>' />
                <asp:Label runat="server" ID="lblEmployeeName"><%# Eval("FullName") %> </asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Role">
            <ItemTemplate>
                <asp:Label runat="server" ID="lblEmployeeRole">Walker</asp:Label>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <asp:ImageButton runat="server" ID="btnDisconnect" visible="false" ImageUrl="~/Images/icons/link_break.png" 
                CommandName="disconnect" OnClientClick="return confirm('Are you sure you want to remove the assignment from GTU and Employee?')" />
                <asp:ImageButton runat="server" ID="btnConnect" Visible="false" ImageUrl="~/Images/icons/add.png" CommandName="connect" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>
<asp:Panel runat="server" ID="pnlAssignAll" Visible="true" HorizontalAlign="Right">
    <asp:Button runat="server" ID="btnAssignAll" Text="Auto Assign" Enabled="true" 
        onclick="btnAssignAll_Click" /> 
</asp:Panel>