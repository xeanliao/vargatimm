
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="OneDMPrint.aspx.cs"
    Inherits="GPS.Website.Print.OneDMPrint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Single Distribution Print</title>
    <style>
        body
        {
            font-family: Arial, Verdana;
            font-size: 15px;
            margin: 0;
            padding: 0;
            letter-spacing: 1px;
        }
        table
        {
            width: 100%;
        }
        table caption
        {
            text-transform: uppercase;
            font-size: 24px;
            font-weight: bold;
            padding: 4px;
            background-color: #94b43d;
        }
        hr
        {
            page-break-before: always;
            margin: 0;
            padding: 0;
            height: 0;
        }
        .pagebreaker
        {
            page-break-before: always;
            margin: 0;
            padding: 0;
            height: 0;
        }
        .rowlabel
        {
            width: 575px;
            font-weight: bold;
            vertical-align: top;
        }
        .collabel
        {
            font-weight: bold;
        }
        .maptable td
        {
            border: solid 1px #94b43d;
        }
        .spaceline
        {
            height: 10px;
        }
        .biglabel
        {
            font-weight: bold;
        }
        #firstpage
        {
            font-size: 23px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:Repeater ID="rptDM" runat="server" OnItemDataBound="rptDM_ItemDataBound">
            <ItemTemplate>
                <div class="pagebreaker">
                </div>
                <table class="maptable" cellspacing="0" cellpadding="0">
                    <caption>
                       <%--<%#  DMName + " - " + Eval("Name")%>--%>
                       <%# Eval("Name")%>
                    </caption>
                    <tbody>
                        <tr>
                            <td colspan="2" style="padding: 0px 1px 0px 1px;">
                                <img style="width: 100%; margin-bottom: -2px;" src='<%# Eval("MapImgUrl") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 10%; text-align: right; border-style: none;">
                                <asp:Image ID="imgDirectionLegend" ImageUrl="~/Images/direction-legend.png" Height="50px" runat="server" />
                            </td>
                        </tr>
                    </tbody>
                </table>
                <%--<div class='spaceline'></div>--%>
                <div>
                    <%# Eval("Nd")%>   
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    </form>
</body>
</html>
