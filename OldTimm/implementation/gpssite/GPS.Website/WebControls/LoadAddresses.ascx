<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoadAddresses.ascx.cs" Inherits="GPS.Website.WebControls.LoadAddresses" %>

<script type="text/javascript">
    //<![CDATA[

    $(document).ready(function() {
//        $("#load-addresses-load").click(function() {
//            
//        });
        $("#load-addresses-cancel").click(function() {
            HideDialog(loadAddressesDialog);
        });
    });

    function on_upload_addresses() {
        $("#load-addresses-load").attr("disabled", "disabled");
        HideDialog(loadAddressesDialog);
        FileUpload("Handler/loadaddresses.ashx?color={0}".replace('{0}', $('#load_addresses_address_color').val()),
                "load-addresses-file-upload",
                "load_addresses_section",
                "load_addresses_progressbar_section",
                "load-addresses-msg",
                window.LoadAddressesCallback);
        $("#load-addresses-load").removeAttr('disabled'); 
    }

    
    //]]>
</script>

<div id="load_addresses_section">
    <input id="load_addresses_address_color" type="hidden" value="green" />
    <div>
        <h3>
            <span id="lbl-load-addresses-title">Load Addresses</span></h3>
    </div>
    <div>
        <input id="load-addresses-file-upload" name="load-addresses-file-upload" size="35" style="width: 350px;" type="file" />
    </div>
    <div id="load-addresses-button">
        <input id="load-addresses-load" class="button100" type="button" value="Load" onclick="javascript:on_upload_addresses();" />
        <input id="load-addresses-cancel" class="button100" type="button" value="Cancel" /></div>
    <div>
        <span id="load-addresses-msg" style="display: none"></span>
    </div>
</div>
<div id="load_addresses_progressbar_section" style="display: none">
    Loading...</div>