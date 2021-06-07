<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImportData.ascx.cs"
    Inherits="GPS.Website.WebControls.ImportData" %>
<div id="import_data_section">
    <p style="padding-top:20px;padding-bottom:20px;">
    <input id="FileUpload" name="FileUpload" size="35" style="width: 350px;" type="file" />
    </p>
    <div id="import-data-button">
        <input id="btnImportData" class="button100" type="button" value="Import Data" />
        <input id="btnCancelImport" class="button100" type="button" value="Cancel" /></div>
    <div>
        <span id="lbmsg"></span>
    </div>
</div>
<div id="progressbar_section" style="display: none">
    Loading...</div>
