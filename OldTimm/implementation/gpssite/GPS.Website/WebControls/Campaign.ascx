<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="Campaign.ascx.cs" Inherits="GPS.Website.WebControls.Campaign" %>
<%@ Register Src="~/WebControls/CreateCampaign.ascx" TagName="CreateCampaign" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/CreateSubMap.ascx" TagName="CreateSubMap" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/CampaignList.ascx" TagName="CampaignList" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/ImportData.ascx" TagName="ImportData" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/LoadAddresses.ascx" TagName="LoadAddresses" TagPrefix="uc1" %>
<%@ Register Src="~/WebControls/NewAddress.ascx" TagName="NewAddress" TagPrefix="uc2" %>
<%@ Register Src="~/WebControls/LoadFiveZipArea.ascx" TagName="LoadFiveZipArea" TagPrefix="uc3" %>
<%@ Register Src="~/WebControls/LoadBlockGroupArea.ascx" TagName="LoadBlockGroupArea" TagPrefix="uc3" %>
<%@ Register Src="~/WebControls/LoadCrouteArea.ascx" TagName="LoadCrouteArea" TagPrefix="uc3" %>
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
                        <li><a style="width: 91px; text-align: center;" href="#sub-map-panel">Sub Maps</a></li>
                        <li><a style="width: 90px; text-align: center;" href="#boundary">Addresses</a></li>
                    </ul>
                    <div id="sub-map-panel">
                    </div>
                    <div id="boundary">
                        <div id="address_toolbar">
                            <a id="address-toolbar-new" href="javascript:void(0)" onclick="javascript:ShowNewAddressDialog();">
                                New</a>|<a id="address-toolbar-edit" href="javascript:void(0)" onclick="javascript:on_edit_address();">Edit</a>|<a
                                    id="address-toolbar-delete" href="javascript:void(0)" onclick="javascript:on_delete_address();">Delete</a>
                        </div>
                        <div id="boundary_toolbar_2">
                            <input id="cb_largest" type="checkbox" onclick="javascript:on_show_Radius(this,3);" /><label
                                for="cb_largest">Outer</label>
                            <input id="cb_middle" type="checkbox" onclick="javascript:on_show_Radius(this,2);" /><label
                                for="cb_middle">Middle</label>
                            <input id="cb_smallest" type="checkbox" onclick="javascript:on_show_Radius(this,1);" /><label
                                for="cb_smallest">Inner</label>
                               <br/><input id="btnApplyAll" type="button" onclick="javascript:on_show_Radius(this,1);" value="Apply to All" />
                        </div>
                        <div id="boundary_info" style="display: none;">
                            Total:&nbsp;<span id="total"></span>&nbsp;&nbsp;&nbsp;&nbsp;Pen:&nbsp;<span id="pen"></span>
                        </div>
                        <div id="address_list">
                        </div>
                        <div id="baundary_create_form" class="hidden">
                            <div>
                                <span class="address_title_name">Address:</span>
                                <input id="address_title" type="text" disabled="disabled" class="address_title_content" />
                            </div>
                            <div id="center_lat">
                                <span class="center_coordinate_name">Center Lat:</span> <span id="address_lat" class="center_coordinate_content">
                                </span>&nbsp;&nbsp; <a id="button-center-lat-up" href="javascript:void(0);" class="gps-toolbar-small-button ui-corner-all"
                                    onclick="javascript:on_increase_center_lat();"><span title="Move Up" class="ui-icon ui-icon-circle-plus">
                                    </span></a><a id="button-center-lat-down" href="javascript:void(0);" class="gps-toolbar-small-button ui-corner-all"
                                        onclick="javascript:on_decrease_center_lat();"><span title="Move Down" class="ui-icon ui-icon-circle-minus">
                                        </span></a>
                            </div>
                            <div id="center_lon">
                                <span class="center_coordinate_name">Center Long:</span> <span id="address_lon" class="center_coordinate_content">
                                </span>&nbsp;&nbsp; <a id="button-center-lon-up" href="javascript:void(0);" class="gps-toolbar-small-button ui-corner-all"
                                    onclick="javascript:on_increase_center_lon();"><span title="Move Right" class="ui-icon ui-icon-circle-plus">
                                    </span></a><a id="button-center-lon-down" href="javascript:void(0);" class="gps-toolbar-small-button ui-corner-all"
                                        onclick="javascript:on_decrease_center_lon();"><span title="Move Left" class="ui-icon ui-icon-circle-minus">
                                        </span></a>
                            </div>
                            <div style="display: none">
                                <a id="pick_button" class="boundary_button_text" href="javascript:void(0)" class="pick_center"
                                    onclick="javascript:on_pick_a_center()">Pick a Center</a>
                            </div>
                            <div>
                                <input id="rdMile" name="units" value="Mile" type="radio" onclick="javascript:on_change_radius_units(this);" />
                                <span class="radius_units">Miles</span>
                                <input id="rdKM" name="units" value="KM" type="radio" onclick="javascript:on_change_radius_units(this);" />
                                <span class="radius_units">KM</span>
                            </div>
                            <span id="radiuMeasure1" class="radius_title">Inner Radius(Miles):&nbsp;</span>
                            <input id="radius0" type="text" class="radius_content" onblur="javascript:on_change_radius(this,'slider0');" />
                            <div id="slider0" style="margin: 0px;">
                            </div>
                            <span id="radiuMeasure2" class="radius_title">Middle Radius(Miles):&nbsp;</span>
                            <input id="radius1" type="text" class="radius_content" onblur="javascript:on_change_radius(this,'slider1');" />
                            <div id="slider1" style="margin: 0px;">
                            </div>
                            <span id="radiuMeasure3" class="radius_title">Outer Radius(Miles):&nbsp;</span>
                            <input id="radius2" type="text" class="radius_content" onblur="javascript:on_change_radius(this,'slider2');" />
                            <div id="slider2" style="margin: 0px;">
                            </div>
                            <div id="save_radius">
                                <a id="button-save-radius-save" href="javascript:void(0)" onclick="javascript:on_save_address();">
                                    Save</a> <a id="button-save-radius-reset" href="javascript:void(0)" onclick="javascript:on_reset_address();">
                                        Reset</a> <a id="button-save-radius-cancel" href="javascript:void(0)" onclick="javascript:on_cancle_edit_address();">
                                            Cancel</a>
                            </div>
                        </div>
                    </div>
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
                <div id="map-container">
                </div>
            </div>
        </div>
    </div>
    <!--end of inner-center-->
</div>
<div id="div_import_data" title="Import Data" style="display: none">
    <uc1:ImportData ID="ctlImportData" runat="server" />
</div>
<div id="export_classifications" title="Export Data" style="display: none">
    
    <p id="export-address-lable" style="margin: 0; padding: 4px;">Select the classifications to export:</p>
    <div id="export-options"></div>

    

    

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
<div id="campaign-list-dialog" title="Open Campaign" style="display: none">
    <p style="margin: 0; padding-top: 10px; padding-bottom: 10px;">
        Select a campaign from the list below:</p>
    <uc1:CampaignList ID="ctlCampaignList" runat="server" />
</div>
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
<div id="load-fivezip-area-dialog" title="Find 5 Zip Area" style="display: none;">
    <uc3:LoadFiveZipArea ID="ctlLoadFiveZipArea" runat="server" />
</div>
<div id="load-blockgroup-area-dialog" title="Find Block Group Area" style="display: none;">
    <uc3:LoadBlockGroupArea ID="ctlLoadBlockGroupArea" runat="server" />
</div>
<div id="load-croute-area-dialog" title="Find Carrier Route Area" style="display: none;">
    <uc3:LoadCrouteArea ID="ctlLoadCrouteArea" runat="server" />
</div>
<!-- The container of the dialog used to edit non-deliverable address -->
<div id="adjust-count-dialog" style="display: none;">
    <table>
        <tr>
            <td colspan="2">
                <div id="adjust-count-dialog-map" style="position: relative; width: 770px; height: 500px;">
                </div>
            </td>
        </tr>
        <tr id="campaign-row-adjust-total">
            <td>
                Total
            </td>
            <td>
                <input id="campaign-txt-adjust-total" type="text" /><br />
                <label id="campaign-error-adjust-total" style="color: Red;">
                </label>
            </td>
        </tr>
        <tr id="campaign-row-adjust-count">
            <td>
                Count
            </td>
            <td>
                <input id="campaign-txt-adjust-count" type="text" /><br />
                <label id="campaign-error-adjust-count" style="color: Red;">
                </label>
            </td>
        </tr>
        <tr id="campaign-row-adjust-per">
            <td>
                Percentage:
            </td>
            <td>
                <input id="campaign-txt-adjust-per" type="text" />%<br />
                <label id="campaign-error-adjust-per" style="color: Red;">
                </label>
            </td>
        </tr>
    </table>
</div>
