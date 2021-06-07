<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NewAddressSL.aspx.cs" Inherits="GPS.Website.NewAddressSL" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <link href="Style/NewMonitorAddress.css" rel="stylesheet" type="text/css" />
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
    <script type="text/javascript">
//    var newAddressDialog = null;

//    function ShowNewAddressDialog() {
//        $('#new-addresses-msg').hide();
//        $('#new-address-line').val('');
//        $('#new-address-line-postalcode').val('');
//        $('#new-address-loading').hide();
//        $('#new-address').show();
//        if (!newAddressDialog) {
//            newAddressDialog = $('#new-address-dialog').dialog({
//                width: 400, modal: true, overlay: { opacity: 0.5 }
//            });
//        }
//        $(newAddressDialog).dialog('open');
//    }

//    function HideNewAddressDialog() {
//        $(newAddressDialog).dialog('close');
//    }

//    function OnNewAddressCallBack(data) {
//        if (data.length > 0) {
//            LoadCircleAreas(data,
//                            null,
//                            null,
//                            $('#new-address-dialog'),
//                            function() {
//                                NewAddress(data);
//                            });
//        } else {
//            GPS.Loading.hide();
//        }
//    }

        function OnNewAddressSubmit() {
            var args = GetUrlParms();
            var did = args["did"];
            var street = $('#new-address-line').val();
            var postalCode = $('#new-address-line-postalcode').val();
            var pic = $('#hidden-insert-logo').val();
            var service = new TIMM.Website.CampaignServices.CampaignWriterService();
            var iAddressId = 0;
            if (document.getElementById("new_address_id").value != "") {
                iAddressId = parseInt(document.getElementById("new_address_id").value);
                service.DeleteMonitorAddress(iAddressId);
            }
               
            service.NewMonitorAddress(did, street, postalCode, pic, function(data) {
            if (data == null) {
                    $('#new-address-line').val("");
                    $('#new-address-line-postalcode').val("");
                    alert("The zip code is not exist! Please type in the correct address and zip code.");
                    
                } else {
                    window.opener.document.location.reload();
                    window.close();
                }
            });

//        $('#new-address').hide();
//        $('#new-address-loading').show();
    }

    function OnUploadImageEndForNewAddress(data) {
        showNewAddress(data.Name)
    }

    function showNewAddress(imageName) {
        $("#address-insert").attr("src", "Files/Images/Address/" + imageName);
        $("#hidden-insert-logo").val(imageName);
        $("#address-insert").show();
    }

    function GPSUploadImage(type, callback) {
        window.open('NewAddressUpload.aspx?type='+type, '_blank', 'height=200px,width=400px,resizable=yes,status=no,toolbar=no,menubar=no,location=no');

    }


//    function GPSUploadImage(type, callback) {
//        this._fnCallBack = callback;
//        this._type = type;
//        var thisObj = this;
//        if ($("#dialoguploadfile").length == 0) {
//            $("#seperateMark").append('<div id="dialoguploadfile"><br /><br /><input id="FileUploader" name="FileUploader" size="35" style="width: 350px;" type="file" /></div>');
//            $("#dialoguploadfile").dialog({
//                autoOpen: false,
//                title: 'UpLoad File',
//                modal: true,
//                width: 450,
//                height: 150,
//                resizable: false,
//                overlay: {
//                    opacity: 0.5
//                    //                ,
//                    //                background: "black"
//                },
//                buttons: {
//                    "Cancel": function() {
//                        $(this).dialog("close");
//                    },
//                    "Upload": function() {
////                        GPS.Loading.show();
////                        $.ajaxFileUpload({
////                            url: 'Handler/uploadfile.ashx?type=' + thisObj._type,
////                            secureuri: false,
////                            fileElementId: 'dialoguploadfile',
////                            dataType: 'json',
////                            success: function(data) {
////                                if (thisObj._fnCallBack) {
////                                    if (!thisObj._fnCallBack(data)) {
////                                        GPS.Loading.hide();
////                                    }
////                                }
////                                else {
////                                    GPS.Loading.hide();
////                                }
////                            },
////                            error: function(data, status, e) {
////                                GPS.Loading.hide();
////                                GPSAlert('Upload file error.');
////                            }
////                        });
//                        $(this).dialog("close");
//                    }
//                }
//            });
//        }
////        $("#dialoguploadfile").dialog("open");
//    }

    </script>
    
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="CampaignServices/CampaignWriterService.svc" />
            </Services>
        </asp:ScriptManager>
        <input type="hidden" id="new_address_id" name="new_address_id" value="<%= mAddressId %>" />
        <div id="new-address">
        <label id="new-address-title">
            Street Address</label>
        <input id="new-address-line" name="new-address-line" size="35" class="text" type="text" value="<%= mStreet %>" />
        <label id="Span1">
            Postal Code</label>
        <input id="new-address-line-postalcode" name="new-address-line-postalcode" class="text"
            style="width: 100px;" type="text" value="<%= mZipcode %>" />
        <%--<label>
            Star Color</label>
        <select id="new-address-color">
            <option value="green" selected="selected">Green</option>
            <option value="red">Red</option>
        </select>--%>
        <div id="address-logo-container">
            <label for="btn-insert-logo">Insert View&nbsp;&nbsp;</label><input id="btn-insert-logo" value="Upload" type="button" onclick="javascript:GPSUploadImage('addressimage',OnUploadImageEndForNewAddress);" />
            <img id="address-insert" src="" alt="" style="display:none;width:200px;height:200px;"/><input id="hidden-insert-logo" type="hidden" />
        </div>
        <div id="new-addresses-button" style="margin-top: 10px;">
            <input id="submit-new-address" class="button100" type="button" value="Submit" onclick="javascript:OnNewAddressSubmit();" />
            <input id="cancel-new-address" class="button100" type="button" value="Cancel" onclick="javascript:HideNewAddressDialog();" />
            <div style="float:right"> <asp:LinkButton ID="btnDelete" runat="server" Text="Delete Address" ForeColor="Red" OnClick="btnDelete_Click"></asp:LinkButton></div>
        </div>
        <div id="seperateMark" name="seperateMark">
            <%--<span id="new-addresses-msg" style="display: none">Address Error.</span>--%>
        </div>
    </div>
    <div id="new-address-loading" style="display: none; text-align: center;">
        Loading...</div>
    <asp:Label id="lblMessage" runat="server" EnableViewState="false"></asp:Label>    
    </form>
</body>
</html>
