<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PreLogin.aspx.cs" Inherits="GPS.Website.PreLogin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>TIMM</title>
</head>
<body>
    <form id="form1" runat="server">
        <table>
            <tr>
                <td>
                    Select Version
                </td>
                <td>
                    <%--<asp:TextBox ID="txtDbname"  value="Server=192.168.4.215; Port=3306; Database=Timm;User ID=root;Password=admin" runat="server" Width="300"></asp:TextBox>--%>
                    <asp:DropDownList ID="dplVersion" AutoPostBack="true" runat="server"></asp:DropDownList>
                </td>
             </tr>
             <tr>
                <td>
                    <asp:Button ID="btnGoLogin" runat="server" onclick="btnGoLogin_Click"  Text="GoLogin" Width="80" Height="30"/>
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
<script type="text/javascript">

</script>
