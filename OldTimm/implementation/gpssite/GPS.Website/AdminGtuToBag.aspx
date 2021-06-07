<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminGtuToBag.aspx.cs" Inherits="GPS.Website.AdminGtuToBag" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <!-- change SelectedIndex -->
    <script type="text/javascript">
        function changeRowIndex(index, txtID, btnID) {
            document.getElementById(txtID).value = index;
            document.getElementById(btnID).click();
        }

    </script>
    <div style="display:none">
        <asp:HiddenField ID="txtGtuIndexLeft" runat="server" EnableViewState="false" />
        <asp:Button ID="btnChangeGtuIndexLeft" runat="server" Text="Select Gtu" OnClick="btnChangeGtuIndexLeft_Click" />
        <asp:HiddenField ID="txtGtuIndexRight" runat="server" EnableViewState="false" />
        <asp:Button ID="btnChangeGtuIndexRight" runat="server" Text="Select Gtu" OnClick="btnChangeGtuIndexRigth_Click" />
    </div>

    <asp:ScriptManager runat="server"></asp:ScriptManager>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div style="height:700px; overflow:scroll; ">
                <asp:Label runat="server" ID="lblError" ForeColor="Red" EnableViewState="false"></asp:Label>
                <asp:DropDownList ID="dropDownBagList" runat="server" AutoPostBack="true" 
                    onselectedindexchanged="dropDownBagList_SelectedIndexChanged">
                </asp:DropDownList>
                <br />
                <br />
                <asp:TextBox ID="txtSearch" runat="server"></asp:TextBox>
                <asp:Button ID="btnSearch" runat="server" Text="Search" />
                <br />
                <br />
                <table cellpadding="0" cellspacing="0" border="0" width="450px">
                    <tr>
                        <td style="width:200; border:1px solid green; vertical-align:top">
                            <asp:Label ID="Label1" runat="server" Text="Unallocated GTUs" Font-Bold="True"></asp:Label>
                            <br />
                            <asp:GridView ID="gridLeft" runat="server" AutoGenerateColumns="false" Width="100%"
                            DataKeyNames="Id"
                            ShowHeader="False" OnRowDataBound="gridLeft_RowDataBound">
                                <SelectedRowStyle BackColor="#E0F0E0" />
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <%# Eval("ShortUniqueID") %>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ImageUrl="~/Images/icons/transmit_go.png" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                        <td style="width:50px; text-align:center; vertical-align:middle">
                            <asp:ImageButton ID="btnLeftArrow" runat="server" 
                                ImageUrl="~/Images/icons/arrow_left.png" OnClick="btnLeftArrow_Click" />
                            <br />
                            <asp:ImageButton ID="btnRightArrow" runat="server" 
                                ImageUrl="~/Images/icons/arrow_right.png" OnClick="btnRightArrow_Click" />
                        </td>
                        <td style="border:1px solid green; vertical-align:top">
                            <asp:Label ID="Label2" runat="server" Text="Allocated To" Font-Bold="True"></asp:Label>
                            <br />
                            <asp:GridView ID="gridRight" runat="server" ShowHeader="False" Width="100%" 
                            DataKeyNames="Id"
                            AutoGenerateColumns="false" OnRowDataBound="gridRight_RowDataBound">
                                <SelectedRowStyle BackColor="#F0E0E0" />
                                <Columns>
                                    <asp:BoundField DataField="Id" Visible="false" />
                                    <asp:BoundField HeaderText="UniqueID" DataField="ShortUniqueID" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:ImageButton ImageUrl="~/Images/icons/transmit_go.png" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </td>
                    </tr>
            
                </table>
    
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
    </br>
    <div style="width:450px">
        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" OnClientClick="return confirm('Do you really want to cancel?')"/>
        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" OnClientClick="return confirm('Do you really want to save?')" />
    </div>

    </form>
</body>

</html>
