<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DistributionMap.ascx.cs" Inherits="GPS.Website.WebControls.Distribution.DistributionMap" %>
<%@ Register Src="~/WebControls/CreateCampaign.ascx" TagName="CreateCampaign" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/CreateSubMap.ascx" TagName="CreateSubMap" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/CampaignList.ascx" TagName="CampaignList" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/ImportData.ascx" TagName="ImportData" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/LoadAddresses.ascx" TagName="LoadAddresses" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/NewAddress.ascx" TagName="NewAddress" TagPrefix="uc2" %>
<%@ Register Src="~/WebControls/LoadFiveZipArea.ascx" TagName="LoadFiveZipArea" TagPrefix="uc3" %>
<%@ Register Src="~/WebControls/Distribution/AssignDrivers.ascx" TagName="AssignDrivers" TagPrefix="uc3" %>
<%@ Register Src="~/WebControls/Distribution/AssignAuditors.ascx" TagName="AssignAuditors" TagPrefix="uc4" %>
<%@ Register Src="~/WebControls/Distribution/AssignWalkers.ascx" TagName="AssignWalkers" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/Distribution/AssignGtus.ascx" TagName="AssignGtus" TagPrefix="uc2" %>

<script type="text/javascript">
    //$(document).ready(function() {
        //LoadSubMap();
        //LoadDistributionJobs();
    //});
</script>
<script type="text/javascript" src="Javascript/campaign.js"></script>

<div class="outer-east">
    <div class="east-inner-north">
        <div class="ui-widget-content ui-corner-all">
            <div id="lower-classifications-title">
                <asp:Label ID="lb_Lower_Classifications" runat="server" Text="Lower Classifications"></asp:Label>
            </div>
            <div id="lower-classifications">
                <div id="div_lower_first">
                    <div id="div_trk">
                        <input id="ck_TRK" type="checkbox" onclick="javascript:ShowHideMapClassification(this,2);" />
                        <label for="ck_TRK">
                            TRK</label>
                        <span id="lb_trk_color" class="color_span"></span>
                    </div>
                    <div id="div_croute">
                        <input id="ck_CRoute" type="checkbox" onclick="javascript:ShowHideMapClassification(this,15);" />
                        <label for="ck_Z3">
                            CRoute</label>
                        <span id="lb_croute_color" class="color_span" for="ck_CRoute"></span>
                    </div>
                </div>
                <div id="div_lower_second">
                    <div id="div_bg">
                        <input id="ck_BG" type="checkbox" onclick="javascript:ShowHideMapClassification(this,3);" />
                        <label for="ck_BG">
                            B.G.'s</label>
                        <span id="lb_bg_color" class="color_span"></span>
                    </div>
                </div>
            </div>
            <div id="higher-classifications-title">
                <asp:Label ID="lb_Upper_Classifications" runat="server" Text="Upper Classifications"></asp:Label>
            </div>
            <div id="higher-classifications">
                <div id="div_upper_first">
                    <div id="div_Z3">
                        <input id="ck_Z3" type="checkbox" onclick="javascript:ShowHideMapClassification(this,0);" />
                        <label for="ck_Z3">
                            3 Zip</label>
                        <span id="lb_Z3_color" class="color_span" for="ck_Z3"></span>
                    </div>
                    <div id="div_CBSA">
                        <input id="ck_CBSA" type="checkbox" onclick="javascript:ShowHideMapClassification(this,4);" />
                        <label for="ck_CBSA">
                            CBSA</label>
                        <span id="lb_CBSA_color" class="color_span"></span>
                    </div>
                    <div id="div_Urban">
                        <input id="ck_Urban" type="checkbox" onclick="javascript:ShowHideMapClassification(this,5);" />
                        <label for="ck_Urban">
                            Urban</label>
                        <span id="lb_Urban_color" class="color_span"></span>
                    </div>
                    <div id="div_County">
                        <input id="ck_County" type="checkbox" onclick="javascript:ShowHideMapClassification(this,6);" />
                        <label for="ck_County">
                            County</label>
                        <span id="lb_County_color" class="color_span"></span>
                    </div>
                    <div id="div_SLD_House">
                        <input id="ck_SLD_House" type="checkbox" onclick="javascript:ShowHideMapClassification(this,8);" />
                        <label for="ck_SLD_House">
                            SLD (House)</label>
                        <span id="lb_SLD_House_color" class="color_span"></span>
                    </div>
                </div>
                <div id="div_upper_second">
                    <div id="div_Z5">
                        <input id="ck_Z5" type="checkbox" onclick="javascript:ShowHideMapClassification(this,1);" />
                        <label for="ck_Z5">
                            5 Zip</label>
                        <span id="lb_Z5_color" class="color_span"></span>
                    </div>
                    <div id="div_SD_Elem">
                        <input id="ck_SD_Elem" type="checkbox" onclick="javascript:ShowHideMapClassification(this,10);" />
                        <label for="ck_SD_Elem">
                            SD (Elem.)</label>
                        <span id="lb_SD_Elem_color" class="color_span"></span>
                    </div>
                    <div id="div_SD_Secondary">
                        <input id="ck_SD_Secondary" type="checkbox" onclick="javascript:ShowHideMapClassification(this,11);" />
                        <label for="ck_SD_Secondary">
                            SD (Sec.)</label>
                        <span id="lb_SD_Secondary_color" class="color_span"></span>
                    </div>
                    <div id="div_SD_Unified">
                        <input id="ck_SD_Unified" type="checkbox" onclick="javascript:ShowHideMapClassification(this,12);" />
                        <label for="ck_SD_Unified">
                            SD (Unified)</label>
                        <span id="lb_SD_Unified_color" class="color_span"></span>
                    </div>
                    <div id="div_SLD_Senate">
                        <input id="ck_SLD_Senate" type="checkbox" onclick="javascript:ShowHideMapClassification(this,7);" />
                        <label for="ck_SLD_Senate">
                            SLD (Senate)</label>
                        <span id="lb_SLD_Senate_color" class="color_span"></span>
                    </div>
                    <div id="div_Voting_District" style="display: none;">
                        <input id="ck_Voting_District" type="checkbox" onclick="javascript:ShowHideMapClassification(this,9);" />
                        <label for="ck_Voting_District">
                            Voting District</label>
                        <span id="lb_Voting_District_color" class="color_span"></span>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <!--end of east-inner-north-->
    <div class="east-inner-center">
        <div id="sub-map-container-inner">
            <div id="sub-map-title" style="display: none;">
                <span id="remove-all" onclick="javascript:RemoveAllExportItems();">Remove All</span><span>Sub
                    Maps</span>
            </div>
            <div id="sub-map-inner">
                <div id="sub-map-container">
                    <ul>
                        <li><a style="width: 210px; text-align: center;" href="#sub-map-panel">Distribution Maps</a></li>
                        <%--<li><a style="width: 80px; text-align: center;" href="#distribution-job-panel">Jobs</a></li>--%>
                    </ul>
                    <div id="sub-map-panel">
                    </div>
                    <%--<div id="distribution-job-panel">
                    </div>--%>
                </div>
            </div>
        </div>
    </div>
    <!--end of east-inner-center-->
    <div class="east-inner-south">
        <div class="ui-widget-content ui-corner-all" style="overflow: hidden;">
            <div id="summary-area-content">
                <div style="white-space: nowrap;">
                    <span id="lb_Master_Campaign" class="label">MC #:</span> <span id="lbMasterCampaignNumber"
                        style="white-space: nowrap; overflow: hidden;">New Campaign</span>
                </div>
                <div id="div_campaign_total">
                    <span id="lb_Total" class="label">Total:</span> <span id="lbCampaignTotal">0</span>
                </div>
                <div>
                    <span id="lb_CampaignCount" class="label">Count:</span> <span id="lbCampaignCount">0</span>
                </div>
                <div id="div_campaign_pen">
                    <span id="lb_Pen" class="label">Pen.:</span> <span id="lbCampaignPen">0%</span>
                </div>
                <div id="targeting-label">
                    Targeting:<asp:TextBox ID="txtTargeting" Wrap="false" runat="server" class="search-area-box"></asp:TextBox>
                </div>
            </div>
        </div>
    </div>
    <!--end of east-inner-south-->
</div>
<div class="outer-center">
    <div class="inner-center ui-widget-content ui-corner-all">
        <div id="map-container-inner">
            <div id="map-inner" class="ui-widget-content ui-corner-all" style="position: relative;
                height: 100%; overflow: auto;">
            </div>
        </div>
        <div id="map-board">
            <a class="close" data-dismiss="alert" href="#">×</a>
            <ul></ul>            
        </div>
    </div>
    <!--end of inner-center-->
</div>
<div id="div_import_data" title="Import Data" style="display: none">
    <uc1:ImportData ID="ctlImportData" runat="server" />
</div>
<div id="export_classifications" title="Export Data" style="display: none">
    <p style="margin: 0px; padding: 4px;">
        Select the classifications to export:</p>
    <div id="export-options">
    </div>
    <p style="margin: 0px; padding: 4px;">
        Select the export format:&nbsp;
        <select id="export-file-format">
            <option value='txt' selected="selected">Text Files(*.txt)</option>
            <option value='csv'>CSV Files(*.csv)</option>
            <option value='excel'>Microsoft Excel Files(*.xls)</option>
        </select>
    </p>
    <br />
    <div id="export_button_panle" style="text-align: center;">
        <input id="btnExport" type="button" value="Export" onclick="javascript:On_Export();" />
        <input id="btnCancelExport" type="button" value="Cancel" onclick="javascript:On_Cancel_Export();" />
    </div>
</div>
<div id="div-create-campaign-dialog" style="display: none">
    <uc1:CreateCampaign ID="CreateCampaign" runat="server" />
</div>
<%--<div id="create-sub-map-dialog" title="Create New Sub Map" style="display: none">
    <uc1:CreateSubMap ID="ctlCreateSubMap" runat="server" />
</div>--%>
<div id="load-addresses-dialog" title="Load Addresses" style="display: none;">
    <uc1:LoadAddresses ID="ctlLoadAddresses" runat="server" />
</div>
<div id="load-areas-of-circle-dialog" title="Loading coverage of radiuses" style="display: none;
    text-align: center;">
    Loading...
</div>
<div id="new-address-dialog" title="New Address" style="display: none;">
    <uc2:NewAddress ID="ctlNewAddress" runat="server" />
</div>


<div id="assign_drivers_dialog" style="display: none;" title="Assign Drivers">
   <uc3:AssignDrivers ID="AssignDrivers1" runat="server" />
</div>

<div id="assign_auditors_dialog" style="display: none;" title="Assign Auditors">
   <uc4:AssignAuditors ID="AssignAuditors1" runat="server" />
</div>

<div id="assign_walkers_dialog" style="display: none;" title="Assign Walkers">
   <uc1:AssignWalkers ID="AssignWalkers1" runat="server" />
</div>

<div id="assign_gtus_dialog" style="display: none;" title="Assign GTUs">
   <uc2:AssignGtus ID="AssignGtus1" runat="server" />
</div>