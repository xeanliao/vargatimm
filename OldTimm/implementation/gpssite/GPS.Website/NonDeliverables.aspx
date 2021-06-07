<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="NonDeliverables.aspx.cs"
    Inherits="GPS.Website.NonDeliverables" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Non Deliverables</title>
    <link href="Style/all.css" type="text/css" rel="stylesheet" />
    <link href="Style/mappanel.css" type="text/css" rel="stylesheet" />
    <link href="Style/jquery-ui-1.7.2.custom.css" rel="Stylesheet" />

    <script type='text/javascript' src='https://www.bing.com/api/maps/mapcontrol'></script>
    <script type="text/javascript" src="Javascript/BingMapsV63ToV8Shim.js"></script>

    <script type="text/javascript" src="Javascript/jquery-1.3.2.js"></script>

    <script type="text/javascript" src="Javascript/ajaxfileupload.js"></script>

    <script type="text/javascript" src="Javascript/jquery.ui.all.js"></script>

    <script type="text/javascript" src="Javascript/jquery.layout.js"></script>

    <script type="text/javascript" src="Javascript/gps.js"></script>

    <script type="text/javascript" src="Handler/mapsettings.ashx"></script>

    <script type="text/javascript" src="Javascript/gps.loading.js"></script>

    <script type="text/javascript" src="Javascript/gps.eventtrigger.js"></script>

    <script type="text/javascript" src="Javascript/map/gps.map.js"></script>

    <script type="text/javascript" src="Javascript/map/gps.map.settings.js"></script>

    <script type="text/javascript" src="Javascript/map/gps.map.arealayer.js"></script>

    <script type="text/javascript" src="Javascript/map/gps.map.areabox.js"></script>

    <script type="text/javascript" src="Javascript/map/gps.map.area.js"></script>

    <script type="text/javascript" src="Javascript/map/gps.map.mapdrawing.js"></script>

    <script type="text/javascript" src="Javascript/gps.container.js"></script>

    <script type="text/javascript" src="Javascript/nondeliverables/gps.nd.js"></script>

    <script type="text/javascript" src="Javascript/nondeliverables/GPS.Nd.NdAreaDialog.js"></script>

    <script type="text/javascript" src="Javascript/nondeliverables/gps.nd.mappanel.js"></script>

</head>
<body>
    <form id="form1" runat="server">
    <input type="hidden" id="bingMapKey" value="<%= System.Configuration.ConfigurationManager.AppSettings["BinMapKey"] %>" />
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="AreaServices/AreaReaderService.svc" />
            <asp:ServiceReference Path="AreaServices/AreaWriterService.svc" />
        </Services>
    </asp:ScriptManager>
    <div class="non-deliverables-content">
        <div id="map-panel" class="ui-widget-content ui-corner-all" style="position: relative;
            height: 100%; overflow: auto;">
        </div>
    </div>
    <div class="non-deliverables-content-right">
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
                        <asp:Label ID="lb_trk_color" runat="server" CssClass="color_span" Height="4px" Text=""></asp:Label>
                    </div>
                    <div id="div_croute">
                        <input id="ck_CRoute" type="checkbox" onclick="javascript:ShowHideMapClassification(this,15);" />
                        <label for="ck_CRoute">
                            CRoute</label>
                        <asp:Label ID="lb_croute_color" runat="server" CssClass="color_span" for="ck_CRoute"
                            Height="4px" Text=""></asp:Label>
                    </div>
                </div>
                <div id="div_lower_second">
                    <div id="div_bg">
                        <input id="ck_BG" type="checkbox" onclick="javascript:ShowHideMapClassification(this,3);" />
                        <label for="ck_BG">
                            B.G.'s</label>
                        <asp:Label ID="lb_bg_color" runat="server" CssClass="color_span" Height="4px" Text=""></asp:Label>
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
                        <asp:Label ID="lb_Z3_color" runat="server" CssClass="color_span" for="ck_Z3" Height="4px"
                            Text=""></asp:Label>
                    </div>
                    <div id="div_CBSA">
                        <input id="ck_CBSA" type="checkbox" onclick="javascript:ShowHideMapClassification(this,4);" />
                        <label for="ck_CBSA">
                            CBSA</label>
                        <asp:Label ID="lb_CBSA_color" runat="server" CssClass="color_span" Height="4px" Text=""></asp:Label>
                    </div>
                    <div id="div_Urban">
                        <input id="ck_Urban" type="checkbox" onclick="javascript:ShowHideMapClassification(this,5);" />
                        <label for="ck_Urban">
                            Urban</label>
                        <asp:Label ID="lb_Urban_color" runat="server" CssClass="color_span" Height="4px"
                            Text=""></asp:Label>
                    </div>
                    <div id="div_County">
                        <input id="ck_County" type="checkbox" onclick="javascript:ShowHideMapClassification(this,6);" />
                        <label for="ck_County">
                            County</label>
                        <asp:Label ID="lb_County_color" runat="server" CssClass="color_span" Height="4px"
                            Text=""></asp:Label>
                    </div>
                    <div id="div_SLD_House">
                        <input id="ck_SLD_House" type="checkbox" onclick="javascript:ShowHideMapClassification(this,8);" />
                        <label for="ck_SLD_House">
                            SLD (House)</label>
                        <asp:Label ID="lb_SLD_House_color" runat="server" CssClass="color_span" Height="4px"
                            Text=""></asp:Label>
                    </div>
                </div>
                <div id="div_upper_second">
                    <div id="div_Z5">
                        <input id="ck_Z5" type="checkbox" onclick="javascript:ShowHideMapClassification(this,1);" />
                        <label for="ck_Z5">
                            5 Zip</label>
                        <asp:Label ID="lb_Z5_color" runat="server" CssClass="color_span" Height="4px" Text=""></asp:Label>
                    </div>
                    <div id="div_SD_Elem">
                        <input id="ck_SD_Elem" type="checkbox" onclick="javascript:ShowHideMapClassification(this,10);" />
                        <label for="ck_SD_Elem">
                            SD (Elem.)</label>
                        <asp:Label ID="lb_SD_Elem_color" runat="server" CssClass="color_span" Height="4px"
                            Text=""></asp:Label>
                    </div>
                    <div id="div_SD_Secondary">
                        <input id="ck_SD_Secondary" type="checkbox" onclick="javascript:ShowHideMapClassification(this,11);" />
                        <label for="ck_SD_Secondary">
                            SD (Sec.)</label>
                        <asp:Label ID="lb_SD_Secondary_color" runat="server" CssClass="color_span" Height="4px"
                            Text=""></asp:Label>
                    </div>
                    <div id="div_SD_Unified">
                        <input id="ck_SD_Unified" type="checkbox" onclick="javascript:ShowHideMapClassification(this,12);" />
                        <label for="ck_SD_Unified">
                            SD (Unified)</label>
                        <asp:Label ID="lb_SD_Unified_color" runat="server" CssClass="color_span" Height="4px"
                            Text=""></asp:Label>
                    </div>
                    <div id="div_SLD_Senate">
                        <input id="ck_SLD_Senate" type="checkbox" onclick="javascript:ShowHideMapClassification(this,7);" />
                        <label for="ck_SLD_Senate">
                            SLD (Senate)</label>
                        <asp:Label ID="lb_SLD_Senate_color" runat="server" CssClass="color_span" Height="4px"
                            Text=""></asp:Label>
                    </div>
                    <div id="div_Voting_District" style="display: none;">
                        <input id="ck_Voting_District" type="checkbox" onclick="javascript:ShowHideMapClassification(this,9);" />
                        <label for="ck_Voting_District">
                            Voting District</label>
                        <asp:Label ID="lb_Voting_District_color" runat="server" CssClass="color_span" Height="4px"
                            Text=""></asp:Label>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <div class="non-deliverables-north">
        <div class="head-container ui-widget-header ui-corner-all">
            <div class="head-logo-container">
            </div>
            <ul id="menu-container">
                <li class="head-link">Areas
                    <ul>
                        <li id="menu-item-new-campaign" class="ui-widget-header" title="Add Custom Area" onclick="javascript:OnMenuClick('custom');">
                            Add Custom Area</li>
                        <li id="menu-item-open-campaign" class="ui-widget-header" title="Add Zip Code Area" onclick="javascript:OnMenuClick('zipcode');">
                            Add Zip Code Area</li>
                        <li id="menu-item-save-campaign" class="ui-widget-header" title="Add Tract Area" onclick="javascript:OnMenuClick('tract');">
                            Add Tract Area</li>
                        <li id="menu-item-delete-campaign" class="ui-widget-header" title="Add Block Group Area"
                            onclick="javascript:OnMenuClick('blockgroup');">Add Block Group Area</li>
                    </ul>
                </li>
                <li class="head-link" style="width: 156px;">Addresses
                    <ul style="width: 155px;">
                        <li id="menu-item-load-addresses-green" class="ui-widget-header" title="Add Address"
                            onclick="javascript:OnMenuClick('address');">Add Address</li>
                        <li id="menu-item-load-addresses-red" class="ui-widget-header" title="Load Addresses"
                            onclick="javascript:OnMenuClick('addresses');">Load Addresses</li>
                    </ul>
                </li>
                <li class="head-link"><a href="javascript:void(0);" onclick="javascript:window.close();"
                    style="color: #000;">Save & Close</a></li>
            </ul>
            <div style="position: absolute; display: none; top: 9px; right: 6px;">
                <a id="print-options-button" href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                    onclick="javascript:OnMenuClick('custom');"><span class="ui-icon ui-icon-pin-s">
                    </span>Add Custom Area</a> <a id="A1" href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                        onclick="javascript:OnMenuClick('zipcode');"><span class="ui-icon ui-icon-pin-s">
                        </span>Add Zip Code Area</a> <a id="A2" href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                            onclick="javascript:OnMenuClick('tract');"><span class="ui-icon ui-icon-pin-s"></span>
                            Add Tract Area</a> <a id="A3" href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                                onclick="javascript:OnMenuClick('blockgroup');"><span class="ui-icon ui-icon-pin-s">
                                </span>Add Block Group Area</a> <a id="A4" href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                                    onclick="javascript:OnMenuClick('address');"><span class="ui-icon ui-icon-pin-s">
                                    </span>Add Address</a><a id="A5" href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                                        onclick="javascript:OnMenuClick('addresses');"><span class="ui-icon ui-icon-pin-s">
                                        </span>Upload Address</a> <a href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                                            onclick="javascript:window.close();"><span class="ui-icon ui-icon-close"></span>
                                            Save & Close</a>
            </div>
        </div>
    </div>
    <div id="dialog-custom-area" style="display: none;">
        <table>
            <tr>
                <td>
                    Name
                </td>
                <td>
                    <input id="txtcaName" type="text" maxlength="20" /><br />
                    <label id="valcaName" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    H/H
                </td>
                <td>
                    <input id="txtcaTotal" type="text" /><br />
                    <label id="valcaTotal" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    Description
                </td>
                <td>
                    <input id="txtcaDescription" maxlength="100" type="text" />
                </td>
            </tr>
        </table>
    </div>
    <!-- The container of the dialog used to edit non-deliverable 5 digit zip area -->
    <div id="nd-5zip-dialog" style="display: none;">
        <table>
            <tr>
                <td>
                    Code
                </td>
                <td>
                    <input id="nd-txt-5zip-code" type="text" maxlength="20" /><br />
                    <label id="nd-error-5zip-code" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    H/H
                </td>
                <td>
                    <input id="nd-txt-5zip-hh" type="text" /><br />
                    <label id="nd-error-5zip-hh" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    Description
                </td>
                <td>
                    <input id="nd-txt-5zip-description" type="text" maxlength="100" />
                </td>
            </tr>
        </table>
    </div>
    <!-- The container of the dialog used to edit non-deliverable tract area -->
    <div id="nd-tract-dialog" style="display: none;">
        <table>
            <tr>
                <td>
                    State Code
                </td>
                <td>
                    <input id="nd-txt-tract-state-code" type="text" maxlength="20" /><br />
                    <label id="nd-error-tract-state-code" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    County Code
                </td>
                <td>
                    <input id="nd-txt-tract-county-code" type="text" maxlength="20" /><br />
                    <label id="nd-error-tract-county-code" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    Tract Code
                </td>
                <td>
                    <input id="nd-txt-tract-code" type="text" maxlength="20" /><br />
                    <label id="nd-error-tract-code" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    H/H
                </td>
                <td>
                    <input id="nd-txt-tract-hh" type="text" /><br />
                    <label id="nd-error-tract-hh" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    Description
                </td>
                <td>
                    <input id="nd-txt-tract-description" type="text" maxlength="100" />
                </td>
            </tr>
        </table>
    </div>
    <!-- The container of the dialog used to edit non-deliverable tract area -->
    <div id="nd-bg-dialog" style="display: none;">
        <table>
            <tr>
                <td>
                    State Code
                </td>
                <td>
                    <input id="nd-txt-bg-state-code" type="text" maxlength="20" /><br />
                    <label id="nd-error-bg-state-code" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    County Code
                </td>
                <td>
                    <input id="nd-txt-bg-county-code" type="text" maxlength="20" /><br />
                    <label id="nd-error-bg-county-code" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    Tract Code
                </td>
                <td>
                    <input id="nd-txt-bg-tract-code" type="text" maxlength="20" /><br />
                    <label id="nd-error-bg-tract-code" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    BG Code
                </td>
                <td>
                    <input id="nd-txt-bg-code" type="text" maxlength="20" /><br />
                    <label id="nd-error-bg-code" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    H/H
                </td>
                <td>
                    <input id="nd-txt-bg-hh" type="text" /><br />
                    <label id="nd-error-bg-hh" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    Description
                </td>
                <td>
                    <input id="nd-txt-bg-description" type="text" maxlength="100" />
                </td>
            </tr>
        </table>
    </div>
    <!-- The container of the dialog used to edit non-deliverable address -->
    <div id="nd-address-dalog" style="display: none;">
        <table>
            <tr>
                <td>
                    Street Address
                </td>
                <td>
                    <input id="nd-txt-address-street" type="text" /><br />
                    <label id="nd-error-address-street" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    ZIP Code
                </td>
                <td>
                    <input id="nd-txt-address-zipcode" type="text" /><br />
                    <label id="nd-error-address-zipcode" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    Geofence Scope
                </td>
                <td>
                    <input id="nd-txt-address-geofence" type="text" /><br />
                    <label id="nd-error-address-geofence" style="color: Red;">
                    </label>
                </td>
            </tr>
            <tr>
                <td>
                    Description
                </td>
                <td>
                    <input id="nd-txt-address-description" type="text" maxlength="100" />
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
