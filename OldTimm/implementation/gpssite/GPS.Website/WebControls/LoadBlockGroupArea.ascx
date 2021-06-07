<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoadBlockGroupArea.ascx.cs" 
Inherits="GPS.Website.WebControls.LoadBlockGroupArea" %>

<script type="text/javascript">
    var loadBlockGroupAreaDialog = null;
    function ShowLoadBlockGroupAreaDialog() {
        $('#load-blockgroup-area-msg').hide();
        $('#load-blockgroup-code-line').val('');
        $('#load-blockgroup-area-loading').hide();
        $('#load-blockgroup-area').show();
        if (!loadBlockGroupAreaDialog) {
            loadBlockGroupAreaDialog = $('#load-blockgroup-area-dialog').dialog({
                width: 350, modal: true, overlay: { opacity: 0.5 }
            });
        }
        $(loadBlockGroupAreaDialog).dialog('open');
    }

    function HideLoadBlockGroupAreaDialog() {
        $(loadBlockGroupAreaDialog).dialog('close');
    }

    function OnLoadBlockGroupAreaCallBack(data) {
        if (data.length > 0) {
            //            if (map) { map.SignBlockGroupAreas(data); }
            LoadBlockGroupArea(data);
            HideLoadBlockGroupAreaDialog();
        }
        else {
            $('#load-blockgroup-area-loading').hide();
            $('#load-blockgroup-area').show();
            $('#load-blockgroup-area-msg').show();
        }
    }

    function OnLoadBlockGroupAreaSubmit() {
        var zipcode = $('#load-blockgroup-code-line').val();
        $('#load-blockgroup-area').hide();
        $('#load-blockgroup-area-loading').show();
        var service = new TIMM.Website.AreaServices.AreaReaderService();
        service.GetBlockGroupAreas(zipcode, OnLoadBlockGroupAreaCallBack, null);
    }
    
    

</script>

<div id="load-blockgroup-area">
    <div style="padding-top:20px;">
        <span id="load-blockgroup-area-title">Enter a Block Group code:</span>
    </div>
    <div>
        <input id="load-blockgroup-code-line" name="load-blockgroup-code-line" style="margin-top:6px;width:96%;" type="text" />
    </div>
    <div id="load-blockgroup-area-button" style="margin-top: 10px;">
        <input id="submit-load-blockgroup-area" class="button100" type="button" value="Find"
            onclick="javascript:OnLoadBlockGroupAreaSubmit();" />
        <input id="cancel-load-blockgroup-area" class="button100" type="button" value="Cancel"
            onclick="javascript:HideLoadBlockGroupAreaDialog();" /></div>
    <div style="padding-top:10px;">
        <span id="load-blockgroup-area-msg" style="display: none">The Block Group code does not exist.</span>
    </div>
</div>
<div id="load-blockgroup-area-loading" style="display: none;font-size:25px; text-align:center;font-weight:bold;padding-top:20px;">
    Loading...</div>
