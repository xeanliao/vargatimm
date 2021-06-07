<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DistributionMapCenter.ascx.cs" Inherits="GPS.Website.WebControls.DistributionMapCenter" %>
<%@ Register Src="~/WebControls/NoPublishCampaign.ascx" TagName="NoPublishCampaignDialog" TagPrefix="Timm" %>
<div id="div-distribution-map-center">
    <div class="midboder2 div-distribution-map-center-padding">
        <div class="icon"><img src="Images/controlcenter/Icon_DM.png" alt="" /></div>
        <div class="divcontent2">
            <div>
                <span class="boxtitle">Distribution Maps</span><br />
                <span class="boxtext">Create and manage distribution maps for well defined campaigns, and monitor distribution jobs.</span>
            </div>
            <div id="campaign-center-toolbar-dm" class="control-center-toolbar" style="display:none">
                <a href="javascript:void(0)" onclick="javascript:dmCenter.OnBulkNoPublishCampaigns()" >Dismiss Selections</a>
                &nbsp;|&nbsp;<a href="javascript:void(0)" onclick="javascript:dmCenter.OnBulkPublishDMs()">Publish To Monitors</a>
            </div>
            <div id="dm-center-campaign-list" style="display:none">
                <p id="dm-center-loading" class="control-center-loading-text">Loading the list...</p>
            </div>
        </div>
    </div>
    <div id="div-no-publish-campaign-dialog" style="display:none"><Timm:NoPublishCampaignDialog ID="NoPublsihCampaign" runat="server" /></div>
</div>

