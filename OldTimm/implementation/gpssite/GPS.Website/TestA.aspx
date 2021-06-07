<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TestA.aspx.cs" Inherits="GPS.Website.TestA" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:TextBox ID="txtSQL" runat="server" Height="99px" TextMode="MultiLine" 
            Width="720px"></asp:TextBox>
        <asp:Button ID="btnRunSQL" runat="server" onclick="btnRunSQL_Click" 
            Text="Run Sql" />
        <br />
        <asp:GridView ID="gridView1" runat="server">
        </asp:GridView>

                    <div style="text-align:left; width:100%; display:none">Task ID: <asp:TextBox ID="txtTaskID" runat="server"></asp:TextBox> </div>
                    <div style="vertical-align:top; height:100%">
                        <asp:FileUpload ID="FileUpload1" runat="server" Width="100%" />
                    </div>
                    
                    <div style="vertical-align:bottom; border-top:solid 1px gray; text-align:right">
                        <asp:Button ID="btnOK" runat="server" Text="Upload" style="display:none" />
                        <asp:Button ID="btnSubmit" runat="server" Text="Submit" OnClick="upload_click" />
                    </div>
    </form>
</body>
</html>
