<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Requirement.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Untitled Page</title>
    <style type="text/css">
        p
        {
            line-height: 30px;
            font-family: Verdana, Tahoma;
        }
    </style>
</head>
<body style="margin: 20px; font-size: 20px">
    <form id="form1" runat="server">
    <p>
        <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/export_data.aspx" Target="_blank">Export 
    Data</asp:HyperLink></p>
    <p>
        <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="http://#" Target="_blank">Import 
    Data</asp:HyperLink></p>
    <p>
        <asp:HyperLink ID="HyperLink3" runat="server" NavigateUrl="~/export_data.html" Target="_blank">Demo</asp:HyperLink></p>
    <p>
        <asp:HyperLink ID="HyperLink4" runat="server" NavigateUrl="~/submap.html" Target="_blank">SubMap</asp:HyperLink></p>
    </form>
</body>
</html>
