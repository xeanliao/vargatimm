﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintSettings.aspx.cs"
    ValidateRequest="false" Inherits="GPS.Website.PrintSettings" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TIMM &gt; Print Maps</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="Style/all.css" type="text/css" rel="stylesheet" />
    <link href="Style/jquery-ui-1.7.2.custom.css" rel="Stylesheet" />
    <link href="Style/print.css" type="text/css" rel="stylesheet" />

    <script type="text/javascript" src="http://ecn.dev.virtualearth.net/mapcontrol/mapcontrol.ashx?v=6.2"></script>

    <script type="text/javascript" src="Javascript/jquery-1.3.2.js"></script>

    <script type="text/javascript" src="Javascript/jquery.ui.all.js"></script>

    <script type="text/javascript" src="Javascript/jquery.layout.js"></script>

    <script type="text/javascript" src="Javascript/gps.js"></script>

    <script type="text/javascript" src="Javascript/gps.toolbar.js"></script>

    <script type="text/javascript" src="Javascript/gps.loading.js"></script>

    <script type="text/javascript" src="Javascript/print/gps.print.js"></script>
    
    <script type="text/javascript" src="Javascript/print/gps.print.extend.js"></script>

    <script type="text/javascript" src="Javascript/print/gps.print.map.js"></script>

    <script type="text/javascript" src="Javascript/print/gps.print.color.js"></script>

    <script type="text/javascript">
        //<![CDATA[
        $(document).ready(function() {
            initLayout();
            OnPageLoad();
        });

        var outerLayout;

        // Initialize the layout.
        function initLayout() {
            outerLayout = $('body').layout({
                center__paneSelector: ".print-outer-center"
		        , north__paneSelector: ".print-ui-layout-north"
		        , north__size: 86
		        , spacing_open: 2 // ALL panes
		        , spacing_closed: 4 // ALL panes
		        , north__spacing_open: 0
            });
        }

        function OnPrintOptionsClick() {
            $('#print-options-dialog').dialog({
                bgiframe: false,
                autoOpen: false,
                height: 460,
                width: 500,
                modal: true,
                buttons: {
                'Close': function() {
                    updateColorLegend();
                    $(this).dialog('close');
                    }
                    //                    ,
                    //                    'Clear All': function() {
                    //                        $('#print-options-dialog input:checkbox').attr('checked', '');
                    //                    },
                    //                    'Check All': function() {
                    //                        $('#print-options-dialog input:checkbox').attr('checked', 'checked');
                    //                    }
                }
            });

            $('#print-options-dialog').dialog('open');
        }
        //]]>
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="CampaignServices/CampaignReaderService.svc" />
        </Services>
    </asp:ScriptManager>
    <div id="print-settings" class="print-ui-layout-north">
        <div class="head-container ui-widget-header ui-corner-all">
            <div class="head-logo-container">
            </div>
            <div style="position: absolute; top: 9px; right: 6px;">
                <asp:LinkButton ID="lbtnPrint" runat="server" CssClass="gps-toolbar-button ui-corner-all"
                    PostBackUrl="~/Print/CampaignPrint.aspx" OnClientClick="javascript:OnPrintClick();"><span class="ui-icon ui-icon-print"></span>Print</asp:LinkButton>
                <a id="print-options-button" href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                    onclick="javascript:OnPrintOptionsClick();"><span class="ui-icon ui-icon-pin-s">
                    </span>Print Options</a> <a href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                        onclick="javascript:window.close();"><span class="ui-icon ui-icon-close"></span>
                        Close</a>
            </div>
        </div>
        <br />
        <div style="font-size: 16px; font-weight: bold;">
            <span>Home&nbsp;&gt;&nbsp;Print Maps</span>
        </div>
        <br />
        <div id="print-options-dialog" style="display: none;" title="Print Options">
            <div>
                Check the following options to customize your print result.</div>
            <div>
                <ul>
                    <li>
                        <input id="cbxSuppressCoverPage" name="cbxSuppressLocations" type="checkbox" onclick="javascript:OnPrintOptionClick(this,'coverpage');" />Suppress
                        Cover Page </li>
                    <li>
                        <input id="cbxSuppressLocations" name="cbxSuppressLocations" type="checkbox" onclick="javascript:OnPrintOptionClick(this,'location');" />Suppress
                        Locations </li>
                    <li>
                        <input id="cbxSuppressRadii" name="cbxSuppressRadii" type="checkbox" onclick="javascript:OnPrintOptionClick(this,'radii');" />Suppress
                        Radii </li>
                    <li>
                        <input id="cbxSuppressNonDeliverables" name="cbxSuppressRadii" type="checkbox" onclick="javascript:OnPrintOptionClick(this,'nondeliverables');" />Suppress
                        non-deliverable areas </li>
                    <li>
                        <input id="cbxSuppressIndividualSubMaps" name="love" type="checkbox" onclick="javascript:OnPrintOptionClick(this,'submaps');" />Suppress
                        Individual Sub Maps </li>
                    <li>
                        <input id="cbxSuppressCountDetail" name="love" type="checkbox" onclick="javascript:OnPrintOptionClick(this,'submapcount');" />Suppress
                        Sub Map Count Detail </li>
                    <li>
                        <input id="cbxSuppressClassificationLevel" name="love" type="checkbox" onclick="javascript:OnPrintOptionClick(this,'classificationoutlines');" />Suppress
                        Classification Level Outlines </li>
                    <li>
                        <input id="cbxShowPenetrationColors" name="love" type="checkbox" onclick="javascript:OnPrintOptionClick(this,'penetrationcolor');" />Show
                        Sub Map Penetration Colors </li>
                    <li>
                        <input id="cbxChangePenetrationColor" name="love" type="checkbox" onclick="javascript:OnPrintOptionClick(this,'changepenetrationcolor');" />Change
                        Sub Map Penetration Color Criteria
                        <div id="print-new-minimum-criteria">
                            New Minimum Criteria:
                            <table style="width: 320px; display: none;">
                                <tr>
                                    <td id="print-color-td1" style="width: 60px;">
                                        <span style="position: relative; left: -5px;">0 </span>
                                    </td>
                                    <td id="print-color-td2" style="width: 60px;">
                                        <input id="print-color-point1" type="text" onchange="javascript:ColorPointChanged(this);"
                                            value="20" class="colorinput" />
                                    </td>
                                    <td id="print-color-td3" style="width: 60px;">
                                        <input id="print-color-point2" type="text" onchange="javascript:ColorPointChanged(this);"
                                            value="40" class="colorinput" />
                                    </td>
                                    <td id="print-color-td4" style="width: 60px;">
                                        <input id="print-color-point3" type="text" onchange="javascript:ColorPointChanged(this);"
                                            value="60" class="colorinput" />
                                    </td>
                                    <td id="print-color-td5" style="width: 60px;">
                                        <input id="print-color-point4" type="text" onchange="javascript:ColorPointChanged(this);"
                                            value="80" class="colorinput" />
                                    </td>
                                    <td>
                                        <span style="position: relative; left: -16px;">100</span>
                                    </td>
                                </tr>
                                <tr>
                                    <td style="background-color: Blue; height: 10px;">
                                    </td>
                                    <td style="background-color: Green; height: 10px;">
                                    </td>
                                    <td style="background-color: Yellow; height: 10px;">
                                    </td>
                                    <td style="background-color: #f75600; height: 10px;">
                                    </td>
                                    <td style="background-color: #bb0000; height: 10px;">
                                    </td>
                                    <td>
                                    </td>
                                </tr>
                            </table>
                            <table style="width: 320px; font-size: 11px;">
                                <tbody>
                                    <tr>
                                        <td style="width: 20px;">
                                            <input id="cbxColorEnabled1" type="checkbox" checked="checked" onclick="javascript:OnPenetrationColorEnabledChange(this);" />
                                        </td>
                                        <td style="background-color: Blue; width: 50px;">
                                        </td>
                                        <td style="width: 50px;">
                                            Blue
                                        </td>
                                        <td style="width: 150px;">
                                            <input id="txtColorMin1" type="text" onchange="javascript:OnPenetrationColorMin(this);"
                                                style="width: 30px;" value="0" />% -
                                            <input id="txtColorMax1" type="text" onchange="javascript:OnPenetrationColorMax(this);"
                                                style="width: 30px;" value="20" />%
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input id="cbxColorEnabled2" type="checkbox" checked="checked" onclick="javascript:OnPenetrationColorEnabledChange(this);" />
                                        </td>
                                        <td style="background-color: Green; width: 50px;">
                                        </td>
                                        <td>
                                            Green
                                        </td>
                                        <td>
                                            <input id="txtColorMin2" type="text" onchange="javascript:OnPenetrationColorMin(this);"
                                                style="width: 30px;" value="20" />% -
                                            <input id="txtColorMax2" type="text" onchange="javascript:OnPenetrationColorMax(this);"
                                                style="width: 30px;" value="40" />%
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input id="cbxColorEnabled3" type="checkbox" checked="checked" onclick="javascript:OnPenetrationColorEnabledChange(this);" />
                                        </td>
                                        <td style="background-color: Yellow; width: 50px;">
                                        </td>
                                        <td>
                                            Yellow
                                        </td>
                                        <td>
                                            <input id="txtColorMin3" type="text" onchange="javascript:OnPenetrationColorMin(this);"
                                                style="width: 30px;" value="40" />% -
                                            <input id="txtColorMax3" type="text" onchange="javascript:OnPenetrationColorMax(this);"
                                                style="width: 30px;" value="60" />%
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input id="cbxColorEnabled4" type="checkbox" checked="checked" onclick="javascript:OnPenetrationColorEnabledChange(this);" />
                                        </td>
                                        <td style="background-color: #f75600; width: 50px;">
                                        </td>
                                        <td>
                                            Orange
                                        </td>
                                        <td>
                                            <input id="txtColorMin4" type="text" onchange="javascript:OnPenetrationColorMin(this);"
                                                style="width: 30px;" value="60" />% -
                                            <input id="txtColorMax4" type="text" onchange="javascript:OnPenetrationColorMax(this);"
                                                style="width: 30px;" value="80" />%
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <input id="cbxColorEnabled5" type="checkbox" checked="checked" onclick="javascript:OnPenetrationColorEnabledChange(this);" />
                                        </td>
                                        <td style="background-color: #bb0000; width: 50px;">
                                        </td>
                                        <td>
                                            Red
                                        </td>
                                        <td>
                                            <input id="txtColorMin5" type="text" onchange="javascript:OnPenetrationColorMin(this);"
                                                style="width: 30px;" value="80" />% -
                                            <input id="txtColorMax5" type="text" onchange="javascript:OnPenetrationColorMax(this);"
                                                style="width: 30px;" value="100" />%
                                        </td>
                                    </tr>
                                </tbody>
                            </table>
                            <div style="color: Red;">
                                The minimum value is included in the range, but the maximum value is excluded except
                                100%.</div>
                        </div>
                    </li>
                </ul>
            </div>
            <!--
            <asp:Button ID="btnPrint" runat="server" Text="Print" Font-Size="Large" OnClientClick="javascript:OnPrintClick();"
                PostBackUrl="~/Print/CampaignPrint.aspx" />
                -->
        </div>
    </div>
    <div id="print-preview" class="print-outer-center">
        <div id="campaign-content" class="ui-widget-content ui-corner-all" style="position: relative;
            height: 100%; overflow: auto;">
            <table id="cover-page" style="text-align: center;" cellpadding="0" cellspacing="2">
                <caption>
                    This Custom Campaign is Presented to:</caption>
                <tbody>
                </tbody>
            </table>
            <div class="spaceline">
            </div>
            <table id="campaign-summary" cellspacing="0" cellpadding="4">
                <caption>
                    Campaign Summary
                </caption>
                <tbody>
                </tbody>
            </table>
            <div class="spaceline">
            </div>
            <table id="campaigninfo-map-table" class="maptable" cellspacing="0" cellpadding="0">
                <caption>
                    Campaign Summary Map
                </caption>
                <tbody>
                    <tr>
                        <td style="border: solid 1px #94b43d;">
                            <div id="campaigninfo-map" class="map">
                            </div>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div class="colorlegend"></div>
              <%-- <table cellpadding="0" cellspacing="0"><tr><td>COLOR LEGEND</td></tr></table>
                <div class="spaceline"></div>
                <table style="width:200px" cellpadding="0" cellspacing="0">
                    <tr>
                        <td width="30px"><div style='height: 12px; width: 25px; background-color:Blue;'></div></td><td width="160px">Blue(0%-20%)</td>
                        <td width="30px"><div style='height: 12px; width: 25px; background-color:Green;'></div></td><td width="175px">Yellow(20%-40%)</td>
                        <td width="30px"><div style='height: 12px; width: 25px; background-color:Yellow;'></div></td><td width="175px">Blue(0%-20%)</td>
                        <td width="30px"><div style='height: 12px; width: 25px; background-color:Yellow;'></div></td><td width="175px">Yellow(20%-40%)</td>
                        <td width="30px"><div style='height: 12px; width: 25px; background-color:Blue;'></div></td><td width="175px">Blue(0%-20%)</td>
                    </tr>
                </table>--%>
            <div class="spaceline">
            </div>
            <table id="campaign-submaps-summary" cellspacing="0" cellpadding="4">
                <caption>
                    Summary of Sub Maps</caption>
                <tbody>
                    <tr>
                        <td class="label">
                            No.
                        </td>
                        <td class="label">
                            Targeting
                        </td>
                        <td class="label">
                            Total
                        </td>
                        <td class="label">
                            Count
                        </td>
                        <td class="label">
                            Penetration
                        </td>
                    </tr>
                </tbody>
            </table>
            <div id="print-submaps-content">
            </div>
            <input type="hidden" id="campaign" name="campaign" />
            <input type="hidden" id="showcoverpage" name="showcoverpage" value="true" />
        </div>
    </div>
    </form>
</body>
</html>
