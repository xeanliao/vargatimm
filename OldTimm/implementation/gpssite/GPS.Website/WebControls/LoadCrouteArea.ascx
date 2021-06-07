<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoadCrouteArea.ascx.cs" Inherits="GPS.Website.WebControls.LoadCrouteArea" %>
<script type="text/javascript">
    var loadCrouteAreaDialog = null;
    function ShowLoadCrouteAreaDialog() {
        $('#load-croute-area-msg').hide();
        $('#load-croute-code-line').val('');
        $('#load-croute-area-loading').hide();
        $('#load-croute-area').show();
        if (!loadCrouteAreaDialog) {
            loadCrouteAreaDialog = $('#load-croute-area-dialog').dialog({
                width: 350, modal: true, overlay: { opacity: 0.5 }
            });
        }
        $(loadCrouteAreaDialog).dialog('open');
    }

    function HideLoadCrouteAreaDialog() {
        $(loadCrouteAreaDialog).dialog('close');
    }

    function OnLoadCrouteAreaCallBack(data) {
        if (data.length > 0) {
            //            if (map) { map.SignCrouteAreas(data); }
            LoadCrouteArea(data);
            HideLoadCrouteAreaDialog();
        }
        else {
            $('#load-croute-area-loading').hide();
            $('#load-croute-area').show();
            $('#load-croute-area-msg').show();
        }
    }

    function OnLoadCrouteAreaSubmit() {
        var code = $('#load-croute-code-line').val();
        $('#load-croute-area').hide();
        $('#load-croute-area-loading').show();
        var service = new TIMM.Website.AreaServices.AreaReaderService();
        service.GetCrouteAreas(code, OnLoadCrouteAreaCallBack, null);
    }
    
    

</script>

<div id="load-croute-area">
    <div style="padding-top:20px;">
        <span id="load-croute-area-title">Enter a carrier route code:</span>
    </div>
    <div>
        <input id="load-croute-code-line" name="load-croute-code-line" style="margin-top:6px;width:96%;" type="text" />
    </div>
    <div id="load-croute-area-button" style="margin-top: 10px;">
        <input id="submit-load-croute-area" class="button100" type="button" value="Submit"
            onclick="javascript:OnLoadCrouteAreaSubmit();" />
        <input id="cancel-load-croute-area" class="button100" type="button" value="Cancel"
            onclick="javascript:HideLoadCrouteAreaDialog();" /></div>
    <div style="padding-top:10px;">
        <span id="load-croute-area-msg" style="display: none">The carrier route code does not exist.</span>
    </div>
</div>
<div id="load-croute-area-loading" style="display: none;font-size:25px; text-align:center;font-weight:bold;padding-top:20px;">
    Loading...</div>
