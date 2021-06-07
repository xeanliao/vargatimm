<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="NewAddress.ascx.cs"
    Inherits="GPS.Website.WebControls.NewAddress" %>

<script type="text/javascript">
    var newAddressDialog = null;

    function ShowNewAddressDialog() {
        $('#new-addresses-msg').hide();
        $('#new-address-line').val('');
        $('#new-address-line-postalcode').val('');
        $('#new-address-loading').hide();
        $('#new-address').show();
        if (!newAddressDialog) {
            newAddressDialog = $('#new-address-dialog').dialog({
                width: 400, modal: true, overlay: { opacity: 0.5 }
            });
        }
        $(newAddressDialog).dialog('open');
    }

    function HideNewAddressDialog() {
        $(newAddressDialog).dialog('close');
    }

    function OnNewAddressCallBack(data) {
        if (data.length > 0) {
            LoadCircleAreas(data,
                            null,
                            null,
                            $('#new-address-dialog'),
                            function() {
                                NewAddress(data);
                            });
        } else {
            GPS.Loading.hide();
        }
    }

    function OnNewAddressSubmit() {
        HideNewAddressDialog();
        GPS.Loading.show();
        var params = [];
        params.push('action=newaddress');
        params.push('address=' + $('#new-address-line').val());
        params.push('postalcode=' + $('#new-address-line-postalcode').val());
        params.push('color=' + $('#new-address-color').val());
        $.ajax({
            type: "get",
            url: "Handler/loadaddresses.ashx",
            data: params.join('&'),
            dataType: "json",
            success: function(data, textStatus) {
                OnNewAddressCallBack(data);
            }
        });
        $('#new-address').hide();
        $('#new-address-loading').show();
    }

//    function OnUploadImageEndForNewAddress(data) {
//        $("#address-insert").attr("src", "Files/Images/Address/" + data.Name);
//        $("#hidden-insert-logo").val(data.Name);
//        $("#address-insert").show();
//    }

</script>

<div id="new-address">
    <label id="new-address-title">
        Street Address</label>
    <input id="new-address-line" name="new-address-line" size="35" class="text" type="text" />
    <label id="Span1">
        Postal Code</label>
    <input id="new-address-line-postalcode" name="new-address-line-postalcode" class="text"
        style="width: 100px;" type="text" />
    <label>
        Star Color</label>
    <select id="new-address-color">
        <option value="green" selected="selected">Green</option>
        <option value="red">Red</option>
    </select>
    <%--<div id="address-logo-container">
        <label for="btn-insert-logo">Insert View&nbsp;&nbsp;</label><input id="btn-insert-logo" value="Upload" type="button" onclick="javascript:GPSUploadFile('addressimage',OnUploadImageEndForNewAddress);" />
        <img id="address-insert" src="" alt="" style="display:none;width:200px;height:200px;"/><input id="hidden-insert-logo" type="hidden" />
    </div>--%>
    <div id="new-addresses-button" style="margin-top: 10px;">
        <input id="submit-new-address" class="button100" type="button" value="Submit" onclick="javascript:OnNewAddressSubmit();" />
        <input id="cancel-new-address" class="button100" type="button" value="Cancel" onclick="javascript:HideNewAddressDialog();" /></div>
    <div>
        <span id="new-addresses-msg" style="display: none">Address Error.</span>
    </div>
</div>
<div id="new-address-loading" style="display: none; text-align: center;">
    Loading...</div>
