<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateCampaign.ascx.cs" Inherits="GPS.Website.WebControls.CreateCampaign" %>
<div class="create-campaign-dialog">
    <div><label for="txt-client-name">Client Name</label></div><div><input id="txt-client-name" class="text350" type="text" maxlength=64 /></div>
    <div><label for="txt-contact-name">Contact Name</label></div><div><input id="txt-contact-name" class="text350" type="text" maxlength=64 /></div>
    <div><label for="txt-client-code">Client Code</label></div><div><input id="txt-client-code" class="text350" type="text" maxlength=64 /></div>
    <div id="campaign-logo-container">
        <label for="btn-insert-logo">Insert Logo&nbsp;&nbsp;</label><input id="btn-insert-logo" value="Upload" type="button" onclick="javascript:GPSUploadFile('campaignimage',OnUploadImageEndForNewCampaign);" />
        <img id="img-insert-logo" src="" alt="" style="display: none; height:200px;" /><input id="hidden-insert-logo" type="hidden" />
    </div>
    
    <div>
        <label for="txt-area-description">Area Description</label>
    </div>
    <div>
        <input disabled="disabled" id="txt-area-description" class="text350" type="text"maxlength=128  />
    </div>

    <div>
        <label for="select-total-type">Total Type</label>
    </div>
    <div>
        <select id="select-total-type">
            <option> &lt;select total type&gt; </option>
            <option> APT + HOME </option>
            <option> APT ONLY </option>
            <option> HOME ONLY </option>
        </select>
    </div>

    <div id="select-saler-container" style='display:none'>
        <label for="txt-sales-name">Sales</label><p>
        <select id="all-sales-select" name="all-sales-select" style='width:160px'></select>
    </div>
    <div><label for="campaign-creation-date">Date</label></div><div><input id="campaign-creation-date" type="text" readonly="readonly" /></div>
    <div id="sequence-no-container" style="display:none;"><label for="txt-sequence-no">Sequence No.</label><br /><input id="txt-sequence-no" type="text" style="width:50px;" maxlength=10 /></div>
</div>

<script type="text/javascript">
    //<![CDATA[
    var campaignDatePicker;

    $(function() {
        campaignDatePicker = $('#campaign-creation-date').datepicker();
        campaignDatePicker.datepicker('setDate', new Date());
    });

    // Return a Date object representing the campaign creation date specified
    // by the user.
    //
    function GetCampaignCreationDate() {
        return campaignDatePicker.datepicker('getDate');
    }
    
    function OnUploadImageEndForNewCampaign(data) {
        $("#img-insert-logo").attr("src", "Files/Images/" + data.Name);
        $("#hidden-insert-logo").val(data.Name);
        $("#img-insert-logo").show();
    }

    function BindCampaignCreationDate(date) {
        campaignDatePicker.datepicker('setDate', date);
    }
    //]]>
</script>
