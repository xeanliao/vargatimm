<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="UploadImage.aspx.cs" Inherits="GPS.Website.UploadImage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script type="text/javascript">
        function onSelectedImage(btnUoloadID) {
            document.getElementById(btnUoloadID).click();
        }
    </script>
</head>
<body style="padding:0; margin:0">
    <form id="form1" runat="server">
    <div>
    <!-- a panel for file upload section -->
    <asp:Panel ID="pnlFileUpload" runat="server" style="margin:0; padding:0">
        <asp:FileUpload ID="fileTestUpload" runat="server" Width="100%" />
        <asp:Button ID="btnUpload" Text="Upload" runat="server" OnClick="btnUpload_Click" style="display:none" />
        <br />
        <asp:Image runat="server" ID="imgUploaded" Height="134px" />
    </asp:Panel>
    
    </div>
    </form>
</body>
</html>
