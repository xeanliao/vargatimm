<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Footer.aspx.cs" Inherits="GPS.Website.Print.Footer" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        body
        {
            margin: 0;
            padding: 0;
        }
        table
        {
            font-size: 8pt;
            font-family: Verdana;
        }
        td
        {
            vertical-align: top;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <table style="width: 100%;">
        <tr>
            <td style="width: 17%;">
                <asp:Image ImageUrl="~/Images/vargainc-logo.png" ID="imgVargaLogo" runat="server" Height="54px" />
            </td>
            <td style="width: 33%;">
                MC#:
                <asp:Literal ID="ltlCampaign" runat="server"></asp:Literal>
                <br />
                Created on:
                <asp:Literal ID="ltlPreparedOn" runat="server"></asp:Literal><br />
                Created for:
                <asp:Literal ID="ltlPreparedFor" runat="server"></asp:Literal><br />
                Created by:
                <asp:Literal ID="ltlPreparedBy" runat="server"></asp:Literal>
            </td>
            <td>
                &nbsp;
            </td>
            <td style="text-align: right; width: 30%;">
                <a href="http://www.vargainc.com">www.vargainc.com</a><br />
                PH: 949-768-1500<br />
                FX: 949-768-1501<br />
                <label style="font-size: 7pt;">
                    &copy;2010 Varga Media Solutions, Inc. All rights reserved.</label>
            </td>
            <td style="text-align: right; width: 20%;">
                <span style="font-style: normal;">Created for </span><span style="font-style: italic;">
                    you</span><span style="font-style: normal;"> with help from:</span><br />
                <asp:Image ID="imgTimmLogo" runat="server" ImageUrl="~/Images/timm-logo.png" Height="36px" style="margin-top: 2px; margin-right: 2px;" />
            </td>
        </tr>
    </table>
    </form>
</body>
</html>
