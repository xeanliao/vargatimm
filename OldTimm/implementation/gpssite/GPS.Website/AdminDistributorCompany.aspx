<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminDistributorCompany.aspx.cs" Inherits="GPS.Website.AdminDistributorCompany" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript">
        function showAddDistributorWindow(btn) 
        {
            document.getElementById("addDistributorTable").style.display = "";
            btn.style.display = "none";
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <!-- Search distribution company -->
        <div style="padding-bottom:4px">
            <asp:TextBox ID="txtSearchCompany" runat="server"></asp:TextBox> 
            <asp:Button ID="btnSearch" runat="server" Text="Search" OnClick="btnSearch_Click" />
        </div>

        <!-- Search Result List -->
        <asp:ListBox runat="server" ID="listSearchResult" DataTextField="Name" DataValueField="Id"
            Width="300px" Height="300px" style="border:1px solid green" 
            AutoPostBack="true" OnSelectedIndexChanged="listSearchResult_SelectedIndexChanged">
        </asp:ListBox>

        <!-- Distribution Company Form -->
        <table id="addDistributorTable" cellpadding="0" cellspacing="4" style="display:">
            <tr>
                <td>Distributor</td>
                <td>
                    <asp:TextBox ID="txtDistributionName" runat="server"></asp:TextBox> 
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDistributionName" ErrorMessage="*" Display="Dynamic" ValidationGroup="addDistributor"/>
                    <input type="hidden" id="txtDistributorID" runat="server" /> 
                </td>
            </tr>
            <tr>
                <td>Address Line 1 </td>
                <td><asp:TextBox ID="txtAddress1" runat="server"></asp:TextBox> 
                <asp:RequiredFieldValidator runat="server" ControlToValidate="txtAddress1" ErrorMessage="*" Display="Dynamic"  ValidationGroup="addDistributor" />
                </td>
            </tr>
            <tr>
                <td>Address Line 2 </td>
                <td> <asp:TextBox ID="txtAddress2" runat="server"></asp:TextBox> </td>
            </tr>
            <tr>
                <td>City</td>
                <td>
                    <asp:TextBox ID="txtCity" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator runat="server" Display="Dynamic" ErrorMessage="*" 
                    ControlToValidate="txtCity" ValidationGroup="addDistributor"></asp:RequiredFieldValidator>
                    <asp:DropDownList runat="server" ID="companyStateDropDown">
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td>Zip</td>
                <td><asp:TextBox ID="txtZip" runat="server"></asp:TextBox> 
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" Display="Dynamic" ErrorMessage="*" 
                    ControlToValidate="txtCity" ValidationGroup="addDistributor"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td></td>
                <td>
                    <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" ValidationGroup="none" /> 
                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" ValidationGroup="addDistributor" />
                </td>
            </tr>
        </table>

        <!-- Distribution List  -->
        <div style="width:600px; text-align:right; visibility:hidden"> 
            <input type="button" value="Add New Distributor" onclick="showAddDistributorWindow(this)" /> 
        </div>
        <asp:GridView ID="DistributorGrid" runat="server" AutoGenerateColumns="false" Visible="false">
            <Columns>
                <asp:BoundField DataField="Name" HeaderText="Distributor" />
                <asp:BoundField DataField="Address1" HeaderText="Address 1" />
                <asp:BoundField DataField="City" HeaderText="City" />
                <asp:BoundField DataField="State" HeaderText="State" />
                <asp:BoundField DataField="ZipCode" HeaderText="Zip" />
            </Columns>
        </asp:GridView>
    </div>
    <asp:Label ID="lblMessage" runat="server" EnableViewState="false" ForeColor="Red"></asp:Label>
    </form>
</body>
</html>
