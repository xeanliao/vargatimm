<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ControlCenter.aspx.cs" Inherits="GPS.Website.ControlCenter" %>
<%@ Register src="WebControls/CampaignCenter.ascx" tagname="CampaignCenter" tagprefix="Timm" %>
<%@ Register src="WebControls/AdminCenter.ascx" tagname="AdminCenter" tagprefix="Timm" %>
<%@ Register src="WebControls/DistributionMapCenter.ascx" tagname="DistributionMapCenter" tagprefix="Timm" %>
<%@ Register src="WebControls/ProductivityCenter.ascx" tagname="ProductivityCenter" tagprefix="Timm" %>
<%@ Register src="WebControls/ReportCenter.ascx" tagname="ReportCenter" tagprefix="Timm" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>TIMM Control Center</title>
    
    <link href="Style/jquery-ui-1.7.2.custom.css" rel="Stylesheet" />
    <link href="Style/all.css" rel="stylesheet" type="text/css" />
    <link href="Style/controlcenter.css" rel="stylesheet" type="text/css" />
    
    <script type="text/javascript" src="Javascript/jquery-1.3.2.js"></script>
    <script type="text/javascript" src="Javascript/jquery.ui.all.js"></script>
    <script type="text/javascript" src="Javascript/ajaxfileupload.js"></script>
    <script type="text/javascript" src="Javascript/gps.js"></script>
    <script type="text/javascript" src="Javascript/gps.loading.js"></script>
    <script type="text/javascript" src="Javascript/gps.eventtrigger.js"></script>
    <script type="text/javascript" src="Javascript/controlcenter/gps.controlcenter.js"></script>
    <script type="text/javascript" src="Javascript/login.js"></script>
    <script type="text/javascript">
        function onDismissToMonitor(btn) 
        {
            var selectedTaskIDs = "";
            var id = new String(btn.id);
            var hiddenTextBoxId = id.replace("btnDismissToMonitor", "txtSelectedTaskIdList");

            var divTaskList = document.getElementById("campaign-center-report-list");
            var taskList = divTaskList.getElementsByTagName("INPUT");
            for (var i = 0; i < taskList.length; i++) 
            {
                var radio = taskList[i];
                if (radio.checked) {
                    if (radio.name == "tasklist") {
                        selectedTaskIDs += radio.getAttribute("timmtaskid") + ",";
                    }
                }
            }

            if (selectedTaskIDs == "")
            {
                alert("Please select a task first");
                return false;
            }

            document.getElementById(hiddenTextBoxId).value = selectedTaskIDs;
            return confirm("Do you really want to move report back to GPS Montor?");
        }

        function openAuditorManagementWindow() {
            var pop = window.open("AuditorManagememt.aspx", "AuditorManagement", "width=600px,height=700px,resizable=yes");
            pop.focus();
        }

        function openAdminDistributorWindow() {
            var pop = window.open("AdminDistributorCompany.aspx", "AdminDistributor", "width=600px,height=700px,resizable=yes");
            pop.focus();
        }

        function showGtuMonitor() {
            var divTask = document.getElementById("productivity-center-campaign-list");
            var inputList = divTask.getElementsByTagName("input");

            for (var i = 0; i < inputList.length; i++) {
                var task = inputList[i];

                if (task.checked == true) {
                    var taskid = task.getAttribute("timmtaskid");
                    window.open("TaskMonitor.aspx?taskid=" + taskid, "taskmonitor", "width=900,height=700,resizable=yes");
                    return;
                }
            }

            alert("please choose a task first");
        }

        function showGtuBagManagement() {
            var pop = window.open('AdminGtuToBag.aspx', 'gtuBagManagement', 'width=500,height=700,resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
            pop.focus();
        }

        function openAuditorManagementWindow() {
            var pop = window.open('AdminGtuBagToAuditor.aspx', 'AuditorGtuBagManagement', 'width=500,height=700,resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
            pop.focus();
        }

    </script>

</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="~/CampaignServices/CampaignReaderService.svc" />
                <asp:ServiceReference Path="~/CampaignServices/CampaignWriterService.svc" />
                <asp:ServiceReference Path="~/UserServices/UserReaderService.svc" />
                <asp:ServiceReference Path="~/DistributionMapServices/DMReaderService.svc" />
                <asp:ServiceReference Path="~/TaskServices/TaskWriterService.svc" />
                <asp:ServiceReference Path="~/TaskServices/TaskReaderService.svc" />
                <asp:ServiceReference Path="~/TrackServices/GtuReaderService.svc" />
            </Services>
        </asp:ScriptManager>

    <div id="control-center-container">
        <div id="Header">
            <div class="topleft">
                <img src="Images/controlcenter/2_02.gif" alt="" /></div>
            <div class="topmiddle">
                <div class="headertxetleft">
                    Timm Control Center</div>
                <div id="div-logout">
                    <img src="Images/controlcenter/logout_icn.gif" alt="" />&nbsp;<span style="cursor:pointer;" onclick="javascript:OnLogOutClick();">Logout</span>
                </div>
            </div>
            <div class="topright">
                <img src="Images/controlcenter/2_04.gif" style="clear:both" alt="" /></div>
            <div id="logo">
                <img src="Images/controlcenter/2_07.gif" width="334px" height="111px" alt="TIMM Control Center" />
                <div class="clear">
                </div>
            </div>
            <div id="div-cclogo-title">
                Welcome to <span>TIMM Control Center</span>
              
            </div>
            <div style="margin:75px 50px 0 0; float:right;"><a href="newControlCenter.aspx">switch to new control center</a></div>  
            <div class="clear">
            </div>
            <div class="navbg">
            </div>
        </div>
        <div id="PageBody">
            <div id="control-center-module-grid" style="margin-left:auto; margin-right:auto; background:none; width:1016px">
                <Timm:CampaignCenter ID="CampaignCenter1" runat="server" />
                <Timm:DistributionMapCenter ID="dmc" runat="server" />
                <Timm:ProductivityCenter ID="AdminCenter2" runat="server" />
                <Timm:ReportCenter ID="AdminCenter3" runat="server" />
                <Timm:AdminCenter ID="AdminCenter1" runat="server" />
                <%--<div style="clear:both;"></div>--%>
            </div>
        </div>
        <div id="footerline"></div>
        <div id="footer"></div>
    </div>
    </form>
</body>
</html>