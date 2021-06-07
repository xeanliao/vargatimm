<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CampaignCenter.ascx.cs" Inherits="GPS.Website.WebControls.CampaignCenter" %>
<%@ Register Src="~/WebControls/CreateCampaign.ascx" TagName="CreateCampaignDialog" TagPrefix="Timm" %>
<%@ Register Src="~/WebControls/PublishCampaign.ascx" TagName="PublishCampaignDialog" TagPrefix="Timm" %>
<div id="div-campaign" >
    <div class="midboder2 div-distribution-map-center-padding">
        <div class="icon"><img src="Images/controlcenter/Icon_Campaign.png" alt="" /></div>
        <div class="divcontent">
            <span class="boxtitle">Campaigns</span><br /><span class="boxtext">Create and define campaigns for customers.</span><div class="clear"></div>
            <div id="campaign-center-toolbar" class="control-center-toolbar" style="display:block">
                <a href="javascript:void(0)" onclick="javascript:campaignCenter.OnBulkCopyCampaigns()" id="cpycampaignDiv" style="display:none">Copy Selections</a><label id="cpycampaignsepdiv" style="display:none">|&nbsp;</label>
                <a href="javascript:void(0)" onclick="javascript:campaignCenter.OnNewCampaignClicked()" id="newcampaignDiv" style="display:none">New Campaign</a><label id="newcampaignsepdiv" style="display:none">&nbsp;|&nbsp;</label>
                <a href="javascript:void(0)" onclick="javascript:campaignCenter.OnBulkDeleteCampaigns()" id="deletecampaignDiv" style="display:none">Delete Selections</a><label id="delcampaignsepdiv" style="display:none">|&nbsp;</label>
                <a href="javascript:void(0)" onclick="javascript:campaignCenter.OnBulkPublishCampaigns()" id="publishDiv" style="display:none">Publish To DMs</a>
            </div>
            <div id="campaign-center-campaign-list" style="display:none"><p id="campaign-center-loading" class="control-center-loading-text">Loading the list...</p></div>
        </div>
    </div>
    <div id="div-create-campaign-dialog" style="display: none"><Timm:CreateCampaignDialog ID="CreateCampaign" runat="server" /></div>
    <div id="div-publish-campaign-dialog" style="display: none"><Timm:PublishCampaignDialog ID="PublsihCampaign" runat="server" /></div>
</div>
