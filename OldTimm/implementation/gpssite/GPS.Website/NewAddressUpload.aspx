<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewAddressUpload.aspx.cs" Inherits="GPS.Website.NewAddressUpload" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script type="text/javascript" src="Javascript/jquery-1.3.2.js"></script>
    <script type="text/javascript" src="Javascript/jquery.ui.all.js"></script>
    <script type="text/javascript" src="Javascript/jquery.layout.js"></script>
    <script type="text/javascript" src="Javascript/gps.js"></script>
    <script type="text/javascript" src="Javascript/gps.toolbar.js"></script>
    <script type="text/javascript" src="Javascript/gps.loading.js"></script>
    <script type="text/javascript" src="Javascript/print/gps.print.js"></script>
    <script type="text/javascript" src="Javascript/print/gps.print.extend.js"></script>
    <script type="text/javascript" src="Javascript/print/gps.print.map.js"></script>
    <script type="text/javascript" src="Javascript/print/gps.print.color.js"></script>
    <script type="text/javascript" src="Javascript/ajaxfileupload.js"></script>
    <script type="text/javascript">
        function uploadIMG() {
            var args = GetUrlParms();
            var type_img = args["type"];
            GPS.Loading.show();
            $.ajaxFileUpload({
                url: 'Handler/uploadfile.ashx?type=' + type_img,
                secureuri: false,
                fileElementId: 'dialoguploadfile',
                dataType: 'json',
                success: function(data) {
                    window.opener.OnUploadImageEndForNewAddress(data);
                    window.close();
                }
            });
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div id="dialoguploadfile">
            <br /><br />
            <div>
                <input id="FileUploader" name="FileUploader" size="35" style="width: 350px;" type="file" />
            </div>
            <div id="new-img-button" style="margin-top: 10px;">
                <input id="submit-new-img" class="button100" type="button" value="Upload" onclick="javascript:uploadIMG();" />
                <input id="cancel-new-img" class="button100" type="button" value="Cancel" onclick="window.close();" />
                <input id="typeParam" name="typeParam" type="hidden" />
            </div>
        </div>
        
    </form>
</body>
</html>
