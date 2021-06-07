<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoadFiveZipArea.ascx.cs"
    Inherits="GPS.Website.WebControls.LoadFiveZipArea" %>

<script type="text/javascript">
    var loadFiveZipAreaDialog = null;
    function ShowLoadFiveZipAreaDialog() {
        $('#load-fivezip-area-msg').hide();
        $('#load-fivezip-code-line').val('');
        $('#load-fivezip-area-loading').hide();
        $('#load-fivezip-area').show();
        if (!loadFiveZipAreaDialog) {
            loadFiveZipAreaDialog = $('#load-fivezip-area-dialog').dialog({
                width: 350, modal: true, overlay: { opacity: 0.5 }
            });
        }
        $(loadFiveZipAreaDialog).dialog('open');
    }

    function HideLoadFiveZipAreaDialog() {
        $(loadFiveZipAreaDialog).dialog('close');
    }

    function OnLoadFiveZipAreaCallBack(data) {
        if (data.length > 0) {
            //            if (map) { map.SignFiveZipAreas(data); }
            LoadFiveZipArea(data);
            HideLoadFiveZipAreaDialog();

            // highlight the searched area
            for (var i in data) {
                var divId = "zip" + data[i].Id;
                searchedAreas.push(divId);

                var divArea = document.getElementById(divId);
                if (divArea != null) {
                    divArea.className = "z5iconA";
                }
            }
        }
        else {
            $('#load-fivezip-area-loading').hide();
            $('#load-fivezip-area').show();
            $('#load-fivezip-area-msg').show();
        }
    }

    function OnLoadFiveZipAreaSubmit() {
        var zipcode = $('#load-fivezip-code-line').val();
        $('#load-fivezip-area').hide();
        $('#load-fivezip-area-loading').show();
        var service = new TIMM.Website.AreaServices.AreaReaderService();
        service.GetFiveZipAreas(zipcode, OnLoadFiveZipAreaCallBack, null);
    }
    
    

</script>

<div id="load-fivezip-area">
    <div style="padding-top:20px;">
        <span id="load-fivezip-area-title">Enter a 5 zip code:</span>
    </div>
    <div>
        <input id="load-fivezip-code-line" name="load-fivezip-code-line" style="margin-top:6px;width:96%;" type="text" />
    </div>
    <div id="load-fivezip-area-button" style="margin-top: 10px;">
        <input id="submit-load-fivezip-area" class="button100" type="button" value="Find"
            onclick="javascript:OnLoadFiveZipAreaSubmit();" />
        <input id="cancel-load-fivezip-area" class="button100" type="button" value="Cancel"
            onclick="javascript:HideLoadFiveZipAreaDialog();" /></div>
    <div style="padding-top:10px;">
        <span id="load-fivezip-area-msg" style="display: none">The 5 zip code does not exist.</span>
    </div>
</div>
<div id="load-fivezip-area-loading" style="display: none;font-size:25px; text-align:center;font-weight:bold;padding-top:20px;">
    Loading...</div>
