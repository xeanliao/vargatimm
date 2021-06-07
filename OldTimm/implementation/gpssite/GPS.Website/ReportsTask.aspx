<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportsTask.aspx.cs" Inherits="GPS.Website.ReportsTask"  EnableEventValidation="false" %>
<%@ Register Src="~/WebControls/ReportWalkerTask.ascx" TagName="ReportWalkerTask" TagPrefix="Timm" %>
<%@ Register Src="~/WebControls/ReportDriverTask.ascx" TagName="ReportDriverTask" TagPrefix="Timm" %>
<%@ Register Src="~/WebControls/ReportAuditorTask.ascx" TagName="ReportAuditorTask" TagPrefix="Timm" %>
<%@ Import Namespace="GPS.DomainLayer.Entities" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TIMM &gt; Reports</title>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <link href="Style/jquery-ui-1.7.2.custom.css" rel="Stylesheet" />
    <link href="Style/jquery.datatable/css/dataTables_page.css" rel="Stylesheet" />
    <link href="Style/jquery.datatable/css/dataTables_table_jui.css" rel="Stylesheet" />
    <link href="Style/all.css" type="text/css" rel="stylesheet" />
    <style type="text/css">
        label
        {
            padding-bottom: 4px;
        }
        label, input.text
        {
            display: block;
        }
        input.text
        {
            margin-bottom: 4px;
            width: 95%;
            padding: .4em;
            font-size: 11px;
        }
        fieldset
        {
            padding: 0;
            border: 0;
            margin-top: 25px;
        }
        #test_table
        {
            width: 100%;
        }
        #test_table thead tr th
        {
            text-align: center;
        }
        #test_table td
        {
            text-align: center;
            font-size: xx-small;
        }
        
    </style>

    <script type="text/javascript" src="Javascript/jquery-1.3.2.js"></script>

    <script type="text/javascript" src="Javascript/jquery.ui.all.js"></script>

    <script type="text/javascript" src="Javascript/jquery.dataTables.min.js"></script>

    <script type="text/javascript" src="Javascript/gps.toolbar.js"></script>
    
    <script type="text/javascript" src="Javascript/report/report.js"></script>

</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="UserServices/UserReaderService.svc" />
                <asp:ServiceReference Path="UserServices/UserWriterService.svc" />
                <asp:ServiceReference Path="GroupServices/GroupReaderService.svc" />
                <asp:ServiceReference Path="GroupServices/GroupWriterService.svc" />
            </Services>
        </asp:ScriptManager>
    </div>
    
    <div class="head-container ui-widget-header ui-corner-all" style="width: 980px;">
        <div class="head-logo-container">
        </div>
        <asp:Panel ID="divToolbar" style="position: absolute; top: 9px; right: 6px;" runat="server">
        <%--<div id="divToolbar" style="position: absolute; top: 9px; right: 6px;">--%>
            <table>
                <tr>
                    <td>
                        <a href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                    onclick="javascript:window.close();"><span class="ui-icon ui-icon-close"></span>
                    Close</a>
                    </td>
                    <td>
                        <asp:LinkButton ID="btnPrint" runat="server" CssClass="gps-toolbar-button ui-corner-all" onclick="btnPrint_Click" OnClientClick="document.getElementById('divToolbar').style.display = 'none';" ><span class="ui-icon ui-icon-print"></span>Print</asp:LinkButton>
                    </td>
                </tr>
            </table>
        </asp:Panel>    
        <%--</div>--%>
    </div>
    </form>
    <br />
    <div style="margin-left: auto; margin-right: auto; width: 980px; font-size: 16px;
        font-weight: bold;">
        <span>Home&nbsp;&gt;&nbsp;Reports</span>
    </div>
    <br />
    <div style="margin-left: auto; margin-right: auto; width: 980px;" id="tableDivs">
        <table>
            <tr><td><b>Task&nbsp;#:&nbsp;&nbsp;&nbsp;&nbsp;<asp:Label id="tName" name="tName" runat="server"/></b></td></tr>
            <tr><td><b>Walkers:</b></td></tr>
        </table>
        <table cellspacing="0" cellpadding="0" border="0" id="test_table" name="walkerTotalinfo">
            <tr>
                <th style="width:10%;font-size: x-small;">Campaign</th>
                <th style="width:14%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">YTD(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">Lifttime(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
            </tr>
            <tr>
                <td>Total Units Tracked:</td>
                <td></td>
                <td></td>
                <td></td>
                <td>Total Units Tracked:</td>
                <td></td>
                <td></td>
                <td></td>
                <td>Total Units Tracked:</td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td>Avg.Speed(MPH):</td>
                <td><asp:Label id="avgSpeedNW" name="avgSpeedNW" runat="server"/></td>
                <td><asp:Label id="highSpeedNW" name="highSpeedNW" runat="server"/></td>
                <td><asp:Label id="lowSpeedNW" name="lowSpeedNW" runat="server"/></td>
                <td>Avg.Speed(MPH):</td>
                <td><asp:Label id="avgSpeedYW" name="avgSpeedYW" runat="server"/></td>
                <td><asp:Label id="highSpeedYW" name="highSpeedYW" runat="server"/></td>
                <td><asp:Label id="lowSpeedYW" name="lowSpeedYW" runat="server"/></td>
                <td>Avg.Speed(MPH):</td>
                <td><asp:Label id="avgSpeedAW" name="avgSpeedAW" runat="server"/></td>
                <td><asp:Label id="highSpeedAW" name="highSpeedAW" runat="server"/></td>
                <td><asp:Label id="lowSpeedAW" name="lowSpeedAW" runat="server"/></td>
            </tr>
            <tr>
                <td>Avg.Ground Covered(MILES):</td>
                <td><asp:Label id="avgGroundNW" name="avgGroundNW" runat="server"/></td>
                <td><asp:Label id="highGroundNW" name="highGroundNW" runat="server"/></td>
                <td><asp:Label id="lowGroundNW" name="lowGroundNW" runat="server"/></td>
                <td>Avg.Ground Covered(MILES):</td>
                <td><asp:Label id="avgGroundYW" name="avgGroundYW" runat="server"/></td>
                <td><asp:Label id="highGroundYW" name="highGroundYW" runat="server"/></td>
                <td><asp:Label id="lowGroundYW" name="lowGroundYW" runat="server"/></td>
                <td>Avg.Ground Covered(MILES):</td>
                <td><asp:Label id="avgGroundAW" name="avgGroundAW" runat="server"/></td>
                <td><asp:Label id="highGroundAW" name="highGroundAW" runat="server"/></td>
                <td><asp:Label id="lowGroundAW" name="lowGroundAW" runat="server"/></td>
            </tr>
            <tr>
                <td>Avg.Stops:</td>
                <td><asp:Label id="avgStopNW" name="avgStopNW" runat="server"/></td>
                <td><asp:Label id="highStopNW" name="highStopNW" runat="server"/></td>
                <td><asp:Label id="lowStopNW" name="lowStopNW" runat="server"/></td>
                <td>Avg.Stops:</td>
                <td><asp:Label id="avgStopYW" name="avgStopYW" runat="server"/></td>
                <td><asp:Label id="highStopYW" name="highStopYW" runat="server"/></td>
                <td><asp:Label id="lowStopYW" name="lowStopYW" runat="server"/></td>
                <td>Avg.Stops:</td>
                <td><asp:Label id="avgStopAW" name="avgStopAW" runat="server"/></td>
                <td><asp:Label id="highStopAW" name="highStopAW" runat="server"/></td>
                <td><asp:Label id="lowStopAW" name="lowStopAW" runat="server"/></td>
            </tr>
        </table>
        <br/>
        <table>
            <tr><td><b>Details:</b></td></tr>
        </table>
        <%--<table cellspacing="0" cellpadding="0" border="0" id="walkerDetailinfoTitle" name="walkerDetailinfoTitle">
            <tr>
                <th style="width:10%">Name</th>
                <th style="width:7%">Days</th>
                <th style="width:7%">Actual</th>
                <th style="width:7%">High</th>
                <th style="width:7%">Low</th>
                <th style="width:10%">YTD(optional)</th>
                <th style="width:7%">Actual</th>
                <th style="width:7%">High</th>
                <th style="width:7%">Low</th>
                <th style="width:10%">Lifttime(optional)</th>
                <th style="width:7%">Actual</th>
                <th style="width:7%">High</th>
                <th style="width:7%">Low</th>
            </tr>
        </table>        --%>
        <%-- <asp:DataGrid ID="dgWalker" runat="server" BorderWidth="0" AutoGenerateColumns="false"  Width="100%">
                <Columns>
                    <asp:TemplateColumn >
                        <ItemTemplate>        
                        <%# ((User)Container.DataItem).UserName %> :                
                           <Timm:ReportWalker ID="ReportWalker" runat="server"  CurrentUser='<%# (User)Container.DataItem%>'/>
                        </ItemTemplate>
                    </asp:TemplateColumn>
                </Columns>
            </asp:DataGrid>      --%>
            
            
            
              
        
        <asp:Repeater ID="dgWalker" runat="server">
        
        <HeaderTemplate>
         <table cellspacing="0" cellpadding="0" border="0" id="test_table" name="walkerDetailinfoTitle">
            <tr>
                <th style="width:10%;font-size: x-small;">Name</th>
                <th style="width:7%;font-size: x-small;">Days</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">YTD(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">Lifttime(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
            </tr>
          </HeaderTemplate>
          <ItemTemplate>
                    
               <Timm:ReportWalkerTask ID="ReportWalkerTask" runat="server"  CurrentUser='<%# (User)Container.DataItem%>'/>
          
           </ItemTemplate>          
          <FooterTemplate>
          </table>
          </FooterTemplate>       
          
        
        
        </asp:Repeater>
            <%--<tr>
                <td><b>Total Units Tracked:</b></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td><b>Total Units Tracked:</b></td>
                <td></td>
                <td></td>
                <td></td>
                <td><b>Total Units Tracked:</b></td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td><b>Avg.Speed(MPH):</b></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td><b>Avg.Speed(MPH):</b></td>
                <td></td>
                <td></td>
                <td></td>
                <td><b>Avg.Speed(MPH):</b></td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td><b>Avg.Ground Covered(MILES):</b></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td><b>Avg.Ground Covered(MILES):</b></td>
                <td></td>
                <td></td>
                <td></td>
                <td><b>Avg.Ground Covered(MILES):</b></td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td><b>Avg.Stops:</b></td>
                <td></td>
                <td></td>
                <td></td>
                <td></td>
                <td><b>Avg.Stops:</b></td>
                <td></td>
                <td></td>
                <td></td>
                <td><b>Avg.Stops:</b></td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
        </table>--%>
    </div>
    
    <br />
    <div style="margin-left: auto; margin-right: auto; width: 980px;" id="Div1">
        <table>
            <tr><td><b>Drivers:</b></td></tr>
        </table>
        <table cellspacing="0" cellpadding="0" border="0" id="test_table" name="driverTotalinfo">
            <tr>
                <th style="width:10%;font-size: x-small;">Campaign</th>
                <th style="width:14%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">YTD(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">Lifttime(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
            </tr>
            <tr>
                <td>Total Units Tracked:</td>
                <td></td>
                <td></td>
                <td></td>
                <td>Total Units Tracked:</td>
                <td></td>
                <td></td>
                <td></td>
                <td>Total Units Tracked:</td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td>Avg.Speed(MPH):</td>
                <td><asp:Label id="avgSpeedND" name="avgSpeedND" runat="server"/></td>
                <td><asp:Label id="highSpeedND" name="highSpeedND" runat="server"/></td>
                <td><asp:Label id="lowSpeedND" name="lowSpeedND" runat="server"/></td>
                <td>Avg.Speed(MPH):</td>
                <td><asp:Label id="avgSpeedYD" name="avgSpeedYD" runat="server"/></td>
                <td><asp:Label id="highSpeedYD" name="highSpeedYD" runat="server"/></td>
                <td><asp:Label id="lowSpeedYD" name="lowSpeedYD" runat="server"/></td>
                <td>Avg.Speed(MPH):</td>
                <td><asp:Label id="avgSpeedAD" name="avgSpeedAD" runat="server"/></td>
                <td><asp:Label id="highSpeedAD" name="highSpeedAD" runat="server"/></td>
                <td><asp:Label id="lowSpeedAD" name="lowSpeedAD" runat="server"/></td>
            </tr>
            <tr>
                <td>Avg.Ground Covered(MILES):</td>
                <td><asp:Label id="avgGroundND" name="avgGroundND" runat="server"/></td>
                <td><asp:Label id="highGroundND" name="highGroundND" runat="server"/></td>
                <td><asp:Label id="lowGroundND" name="lowGroundND" runat="server"/></td>
                <td>Avg.Ground Covered(MILES):</td>
                <td><asp:Label id="avgGroundYD" name="avgGroundYD" runat="server"/></td>
                <td><asp:Label id="highGroundYD" name="highGroundYD" runat="server"/></td>
                <td><asp:Label id="lowGroundYD" name="lowGroundYD" runat="server"/></td>
                <td>Avg.Ground Covered(MILES):</td>
                <td><asp:Label id="avgGroundAD" name="avgGroundAD" runat="server"/></td>
                <td><asp:Label id="highGroundAD" name="highGroundAD" runat="server"/></td>
                <td><asp:Label id="lowGroundAD" name="lowGroundAD" runat="server"/></td>
            </tr>
            <tr>
                <td>Avg.Stops:</td>
                <td><asp:Label id="avgStopND" name="avgStopND" runat="server"/></td>
                <td><asp:Label id="highStopND" name="highStopND" runat="server"/></td>
                <td><asp:Label id="lowStopND" name="lowStopND" runat="server"/></td>
                <td>Avg.Stops:</td>
                <td><asp:Label id="avgStopYD" name="avgStopYD" runat="server"/></td>
                <td><asp:Label id="highStopYD" name="highStopYD" runat="server"/></td>
                <td><asp:Label id="lowStopYD" name="lowStopYD" runat="server"/></td>
                <td>Avg.Stops:</td>
                <td><asp:Label id="avgStopAD" name="avgStopAD" runat="server"/></td>
                <td><asp:Label id="highStopAD" name="highStopAD" runat="server"/></td>
                <td><asp:Label id="lowStopAD" name="lowStopAD" runat="server"/></td>
            </tr>
        </table>
        <br/>
        <table>
            <tr><td><b>Details:</b></td></tr>
        </table>
          
              
        
        <asp:Repeater ID="dgDriver" runat="server">
        
        <HeaderTemplate>
         <table cellspacing="0" cellpadding="0" border="0" id="test_table" name="driverDetailinfoTitle">
            <tr>
                <th style="width:10%;font-size: x-small;">Name</th>
                <th style="width:7%;font-size: x-small;">Days</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">YTD(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">Lifttime(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
            </tr>
          </HeaderTemplate>
          <ItemTemplate>
                    
               <Timm:ReportDriverTask ID="ReportDriverTask" runat="server"  CurrentUser='<%# (User)Container.DataItem%>'/>
          
           </ItemTemplate>          
          <FooterTemplate>
          </table>
          </FooterTemplate>       
          
        
        
        </asp:Repeater>
            
    </div>
    
    <br />
    <div style="margin-left: auto; margin-right: auto; width: 980px;" id="Div2">
        <table>
            <tr><td><b>Auditors:</b></td></tr>
        </table>
        <table cellspacing="0" cellpadding="0" border="0" id="test_table" name="auditorTotalinfo">
            <tr>
                <th style="width:10%;font-size: x-small;">Campaign</th>
                <th style="width:14%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">YTD(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">Lifttime(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
            </tr>
            <tr>
                <td>Total Units Tracked:</td>
                <td></td>
                <td></td>
                <td></td>
                <td>Total Units Tracked:</td>
                <td></td>
                <td></td>
                <td></td>
                <td>Total Units Tracked:</td>
                <td></td>
                <td></td>
                <td></td>
            </tr>
            <tr>
                <td>Avg.Speed(MPH):</td>
                <td><asp:Label id="avgSpeedNA" name="avgSpeedNA" runat="server"/></td>
                <td><asp:Label id="highSpeedNA" name="highSpeedNA" runat="server"/></td>
                <td><asp:Label id="lowSpeedNA" name="lowSpeedNA" runat="server"/></td>
                <td>Avg.Speed(MPH):</td>
                <td><asp:Label id="avgSpeedYA" name="avgSpeedYA" runat="server"/></td>
                <td><asp:Label id="highSpeedYA" name="highSpeedYA" runat="server"/></td>
                <td><asp:Label id="lowSpeedYA" name="lowSpeedYA" runat="server"/></td>
                <td>Avg.Speed(MPH):</td>
                <td><asp:Label id="avgSpeedAA" name="avgSpeedAA" runat="server"/></td>
                <td><asp:Label id="highSpeedAA" name="highSpeedAA" runat="server"/></td>
                <td><asp:Label id="lowSpeedAA" name="lowSpeedAA" runat="server"/></td>
            </tr>
            <tr>
                <td>Avg.Ground Covered(MILES):</td>
                <td><asp:Label id="avgGroundNA" name="avgGroundNA" runat="server"/></td>
                <td><asp:Label id="highGroundNA" name="highGroundNA" runat="server"/></td>
                <td><asp:Label id="lowGroundNA" name="lowGroundNA" runat="server"/></td>
                <td>Avg.Ground Covered(MILES):</td>
                <td><asp:Label id="avgGroundYA" name="avgGroundYA" runat="server"/></td>
                <td><asp:Label id="highGroundYA" name="highGroundYA" runat="server"/></td>
                <td><asp:Label id="lowGroundYA" name="lowGroundYA" runat="server"/></td>
                <td>Avg.Ground Covered(MILES):</td>
                <td><asp:Label id="avgGroundAA" name="avgGroundAA" runat="server"/></td>
                <td><asp:Label id="highGroundAA" name="highGroundAA" runat="server"/></td>
                <td><asp:Label id="lowGroundAA" name="lowGroundAA" runat="server"/></td>
            </tr>
            <tr>
                <td>Avg.Stops:</td>
                <td><asp:Label id="avgStopNA" name="avgStopNA" runat="server"/></td>
                <td><asp:Label id="highStopNA" name="highStopNA" runat="server"/></td>
                <td><asp:Label id="lowStopNA" name="lowStopNA" runat="server"/></td>
                <td>Avg.Stops:</td>
                <td><asp:Label id="avgStopYA" name="avgStopYA" runat="server"/></td>
                <td><asp:Label id="highStopYA" name="highStopYA" runat="server"/></td>
                <td><asp:Label id="lowStopYA" name="lowStopYA" runat="server"/></td>
                <td>Avg.Stops:</td>
                <td><asp:Label id="avgStopAA" name="avgStopAA" runat="server"/></td>
                <td><asp:Label id="highStopAA" name="highStopAA" runat="server"/></td>
                <td><asp:Label id="lowStopAA" name="lowStopAA" runat="server"/></td>
            </tr>
        </table>
        <br/>
        <table>
            <tr><td><b>Details:</b></td></tr>
        </table>
          
              
        
        <asp:Repeater ID="dgAuditor" runat="server">
        
        <HeaderTemplate>
         <table cellspacing="0" cellpadding="0" border="0" id="test_table" name="auditorDetailinfoTitle">
            <tr>
                <th style="width:10%;font-size: x-small;">Name</th>
                <th style="width:7%;font-size: x-small;">Days</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">YTD(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
                <th style="width:10%;font-size: x-small;">Lifttime(optional)</th>
                <th style="width:7%;font-size: x-small;">Actual</th>
                <th style="width:7%;font-size: x-small;">High</th>
                <th style="width:7%;font-size: x-small;">Low</th>
            </tr>
          </HeaderTemplate>
          <ItemTemplate>
                    
               <Timm:ReportAuditorTask ID="ReportAuditorTask" runat="server"  CurrentUser='<%# (User)Container.DataItem%>'/>
          
           </ItemTemplate>          
          <FooterTemplate>
          </table>
          </FooterTemplate>       
          
        
        
        </asp:Repeater>
            
    </div>
    
</body>
</html>
