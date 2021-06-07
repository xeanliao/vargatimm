<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminGtuBagToAuditor.aspx.cs" Inherits="GPS.Website.AdminGtuBagToAuditor" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <script type="text/javascript">
        function changeRow(index, txtID, btnID) 
        {
            document.getElementById(txtID).value = index;
            document.getElementById(btnID).click();
        }
    </script>
    <div style="display:none">
        <asp:HiddenField ID="txtLeftGridRow" runat="server" EnableViewState="false" />
        <asp:Button ID="btnChangeLeftGridRow" runat="server" Text="Select Gtu" Width="0" OnClick="btnChangeLeftGridRow_Click" />
        <asp:HiddenField ID="txtRightGridRow" runat="server" EnableViewState="false" />
        <asp:Button ID="btnChangeRightGridRow" runat="server" Text="Select Gtu" Width="0" OnClick="btnChangeRightGridRow_Click" />
    </div>

    <div><asp:Label ID="lblInfo" runat="server" EnableViewState="false" ForeColor="Red"></asp:Label></div>
    <div style="width:440px">
        <asp:DropDownList ID="dropDownAuditor" runat="server" AutoPostBack="true" OnSelectedIndexChanged="dropDownAuditor_SelectedIndexChanged" >
        </asp:DropDownList>
    </div>

    <table cellpadding="0" cellspacing="0" border="0" width="440px">
        <tr>
            <!-- unallocated gtuBag -->
            <td align="right" valign="top" style="width:200px;">
                <asp:Label ID="Literal1" runat="server" Font-Bold="true">GTU Bag available</asp:Label>
                <div style="width:100%; height:400px; overflow:auto; border:solid 1px green">
                    <asp:GridView id="gridLeft" runat="server" AutoGenerateColumns="false" Width="100%"
                    ShowHeader="false" OnRowDataBound="gridLeft_RowDataBound" BorderWidth="0" GridLines="None">
                        <SelectedRowStyle BackColor="#E0F0E0" />
                        <Columns>
                            <asp:BoundField HeaderText="GTU Bag available" DataField="ID" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblGtuCount"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>

            <!-- arrow buttons to move left or Righ -->
            <td align="center" valign="middle" style="width:40px;">
                <asp:ImageButton ImageUrl="~/Images/icons/arrow_right.png" ID="btnArrowRight" 
                    runat="server" onclick="btnArrowRight_Click" /><br />
                <asp:ImageButton ImageUrl="~/Images/icons/arrow_left.png" ID="btnArrowLeft" 
                    runat="server" onclick="btnArrowLeft_Click" />
            </td>

            <!-- GtuBags allocated already  -->
            <td align="left" valign="top">
                <div style="font-weight:bold">GTU Bag Assigned</div>
                <div style="width:200px; height:400px; border:solid 1px green"> 
                    <asp:GridView id="gridRight" runat="server" AutoGenerateColumns="false" ShowHeader="false"
                    OnRowDataBound="gridRight_RowDataBound" BorderWidth="0" Width="100%" GridLines="None">
                        <SelectedRowStyle BackColor="#F0E0E0" />
                        <Columns>
                            <asp:BoundField HeaderText="GTU Bag Assigned" DataField="ID" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblGtuCount"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </td>
        </tr>
    </table>
    <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
    <asp:Button ID="btnSave" runat="server" Text="Save" onclick="btnSave_Click" />
    </form>
</body>
</html>
