<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LoadGtus.ascx.cs" Inherits="GPS.Website.WebControls.LoadGtus" %>
<script type="text/javascript">
    function HideLoadGtus() {
        $(importDataDialog).dialog('close');
    }

    function onLoadCompleted(r) {
        if (r) {
            $('#load-gtus-msg-successfully').show();
        }
        else {
            $('#load-gtus-msg-failed').show();
        }
    }
    
    function OnLoadGtus() {
        $('#load-gtus-msg-successfully').hide();
        $('#load-gtus-msg-failed').hide();
        $('#edit_validateTip').text('');
        var filename = $('#load-gtus-file-upload').val();
        var bValid = true;

        bValid = bValid && (filename != '');
        if (!bValid) {
            edit_updateTip('Please select a import file.');
            return;
        }
        var gtuReader = new GPS.Website.TrackServices.GtuReaderService();
        gtuReader.LoadGtuFromExcel(filename, onLoadCompleted, null, null);
    }


    function edit_updateTip(t) {
        $('#edit_validateTip').text(t).effect("highlight", {}, 1500);
    }
    
</script>

<div id="load_gtus_section">
    <p id="edit_validateTip"></p>
    <p style="padding-top:10px;padding-bottom:20px;">
        <input id="load-gtus-file-upload" name="load-gtus-file-upload" size="35" style="width: 350px;" type="file" />
    </p>
    <div id="load-gtus-button">
        <input id="load-gtus-load" class="button100" type="button" value="Load" onclick="javascript:OnLoadGtus();" />
        <input id="load-gtus-cancel" class="button100" type="button" value="Cancel" onclick="javascript:HideLoadGtus();" /></div>
    <div>
        <br />
        <span id="load-gtus-msg-successfully" style="display:none;">Load operation is completed successfully.</span><span id="load-gtus-msg-failed" style="display:none;">Load operation is failed.</span>
    </div>
</div>
<div id="load-gtus-progressbar_section" style="display: none">
    Loading...</div>