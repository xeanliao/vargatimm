<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TaskMonitorView.aspx.cs" Inherits="GPS.Website.TaskMonitorView" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="cc1" %>
<%@ Register src="WebControls/MonitorTaskGTUs.ascx" tagname="MonitorTaskGTUs" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>GPS.SilverMonitor</title>
    <style type="text/css">
        .watermark
        {
            color:gray;
        }
    </style>
    
    <style type="text/css">
    html, body {
	    height: 100%;
	    overflow: auto;
    }
    body {
	    padding: 0;
	    margin: 0;
    }
    #silverlightControlHost {
	    height: 100%;
	    text-align:center;
    }
    </style>
    <script type="text/javascript" src="Silverlight.js"></script>
    <script type="text/javascript">
        function onSilverlightError(sender, args) {
            var appSource = "";
            if (sender != null && sender != 0) {
                appSource = sender.getHost().Source;
            }

            var errorType = args.ErrorType;
            var iErrorCode = args.ErrorCode;

            if (errorType == "ImageError" || errorType == "MediaError") {
                return;
            }

            var errMsg = "Unhandled Error in Silverlight Application " + appSource + "\n";

            errMsg += "Code: " + iErrorCode + "    \n";
            errMsg += "Category: " + errorType + "       \n";
            errMsg += "Message: " + args.ErrorMessage + "     \n";

            if (errorType == "ParserError") {
                errMsg += "File: " + args.xamlFile + "     \n";
                errMsg += "Line: " + args.lineNumber + "     \n";
                errMsg += "Position: " + args.charPosition + "     \n";
            }
            else if (errorType == "RuntimeError") {
                if (args.lineNumber != 0) {
                    errMsg += "Line: " + args.lineNumber + "     \n";
                    errMsg += "Position: " + args.charPosition + "     \n";
                }
                errMsg += "MethodName: " + args.methodName + "     \n";
            }

            throw new Error(errMsg);
        }
    </script>
    <script type="text/javascript">
        function onUploadImage(btn) {
            btn.click();
        }

        function showGtu(iShowHistory) {
            // get all checked Gtu(s)
            var divTaskGtus = document.getElementById("divTaskGtus");
            var inputList = divTaskGtus.getElementsByTagName("input");

            var gtuIDs = "";
            for (var i = 0; i < inputList.length; i++) {
                var inputElement = inputList[i];
                var gtuid = inputElement.getAttribute("gtuid");
                if (gtuid == null) continue;

                if (inputElement.checked == true) {
                    gtuIDs += "," + gtuid;
                }
            }

            if (gtuIDs.length == 0) {
                alert("please choose at least a GTU");
                return false;
            }

            // Call Silverlight to refresh
            document.getElementById("silverTaskMap").Content.monitorTaskGtus.Call_ShowGtuLocations(iShowHistory, gtuIDs);
        }

        function callSilverToRefreshLocations() {
            document.getElementById("silverTaskMap").Content.monitorTaskGtus.Call_ShowMonitorAddess();
        }

        function showGtuEvents(anchor) {
            var userid = new String(anchor.getAttribute("userid"));
            var pop = window.open("gtuEvents.aspx?userid=" + userid, "gtuevents", "width=500,height=680, resizable=yes", true);
            pop.focus();
        }

        function colorChanged(sender) {
            sender.get_element().style.color = "#" + sender.get_selectedColor();
        }

        function showAssignGtu(taskid) {
            ret = window.showModalDialog("AdminGtuToEmployee.aspx?taskid=" + taskid, "", "dialogHeight:500px;dialogWidth:500px;status:off;resizable:no");
            return true;
        }

        function toggleDiv(divID, img, src1, src2) {
            //debugger;
            var div = document.getElementById(divID);
            if (div.style.display == '')
                div.style.display = 'none';
            else
                div.style.display = '';

            var s = new String(img.src);
            if (s.indexOf(src1) >= 0)
                img.src = s.replace(src1, src2);
            else
                img.src = s.replace(src2, src1);
        }
    </script>
    <link href="Style/jquery-ui-1.7.2.custom.css" rel="Stylesheet" />
    <style type="text/css">
    /*
     * Customization of jQuery styles
     */
    .ui-widget-content
    {
	    background: url(../images/widget-bg.png) repeat-x;
	    background-color: White;
    }

    input.ui-widget-content { background: white; }

    .ui-dialog-content
    {
	    background: url(../images/widget-bg.png) repeat-x;
	    background-color: White;
    }

    .ui-widget {
        font-size: 12px;
    }
    
    </style>
    <script type="text/javascript" src="Javascript/jquery-1.3.2.js"></script>
    <script type="text/javascript" src="Javascript/jquery.ui.all.js"></script>
    <script type="text/javascript">
        $(function () {
            $('#message').dialog({
                modal: true,
                autoOpen: false
            });
        });
    </script>
</head>
<body>
    <form id="form1" runat="server" style="height:100%">
    <asp:ScriptManager ID="MonitorScriptManager" runat="server"></asp:ScriptManager>
    <asp:Timer runat="server" id="UpdateTimer" interval="60000" ontick="UpdateTimer_Tick" />

    <table id="mainContent" runat="server" cellpadding="0" cellspacing="1" width="100%" style="height:100%">
        <tr style="height:24px">
            <td><asp:Label ID="lblBreadcrumb" runat="server">Campaign > Task Name</asp:Label> </td>
            <td></td>
        </tr>
        <tr>
            <td style="vertical-align:top; height:100%" >
                <div id="silverlightControlHost" style="height:95%">
                    <object id="silverTaskMap" data="data:application/x-silverlight-2," 
                        type="application/x-silverlight-2" width="100%" height="100%">
		              <param name="source" value="ClientBin/GPS.SilverMonitor.xap?v2013021"/>
		              <param name="onError" value="onSilverlightError" />
		              <param name="background" value="white" />
		              <param name="minRuntimeVersion" value="5.0.60818.0" />
		              <param name="autoUpgrade" value="true" />
                      <param name="InitParams" value="TaskId=<%= this.mTaskID %>,MapKey=<%= this.m_BinMap %>" />
                      <param name="windowless" value="true" />
		              <a href="http://go.microsoft.com/fwlink/?LinkID=149156&v=5.0.60818.0" style="text-decoration:none">
 			              <img src="http://go.microsoft.com/fwlink/?LinkId=161376" alt="Get Microsoft Silverlight" style="border-style:none"/>
		              </a>
	                </object>
                    <iframe id="_sl_historyFrame" style="visibility:hidden;height:0px;width:0px;border:0px"></iframe>
                </div>
            </td>
            <td style="width:320px; background-color:#F0FFF0; vertical-align:top; overflow:scroll">
                <asp:UpdatePanel ID="updatePanelMain" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="UpdateTimer" EventName="Tick" />
                    </Triggers>
                    <ContentTemplate>

                <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0">
                    <cc1:TabPanel runat="server" HeaderText="<b>GTUs</b>" Font-Bold="true" ID="TabPanel1">
                        <ContentTemplate>
                            <table cellpadding="0" cellspacing="2px" border="0" >
                                <tr>
                                    <!--
                                    <td>
                                        <asp:Image ID="Image1" ImageUrl="~/Images/icons/bullet_arrow_down.png" runat="server" />
                                    </td>
                                    -->
                                    <td colspan="3">
                                        <asp:Label ID="lblAuditor" runat="server" Text="Auditor"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="Image2" ImageUrl="~/Images/icons/user_green.png" runat="server" />
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lblAuditorGtu"></asp:Label>
                                    </td>
                                    <td>
                                        <asp:Label runat="server" ID="lblAuditorName" Text="Jose Abuela"></asp:Label>
                                    </td>
                                </tr>
                            </table>

                            <!-- DMap -->
                            <br />
                            <asp:Panel runat="server" ID="pnlAuditorDMaps">
                                <uc1:MonitorTaskGTUs ID="ctlMonitorTaskGTUs"  ViewOnly="true" runat="server" />
                            </asp:Panel>

                            <!-- Unassigned Gtus -->
                            <br />
                            <table cellpadding="0" cellspacing="4" border="0" width="100%" >
                                <tr>
                                    <td>
                                        <img src="images/icons/bullet_arrow_down.png" alt="" border="0" onclick="toggleDiv('divUnassignedGtus', this, 'bullet_arrow_down.png', 'resultset_next.png' )" /> 
                                    </td>
                                    <td>
                                        
                                    </td>
                                    <td style="text-align:right">
                                        
                                    </td>

                                </tr>
                            </table>
                            <div id="divUnassignedGtus">
                            <asp:GridView ID="UnassignedGtuGrid" runat="server" AutoGenerateColumns="false" Width="100%"
                                GridLines="None" style="background-color:#F0FFF0" >
                                <Columns>
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:Image ID="Image1" runat="server" ImageUrl="~/Images/icons/transmit_go.png" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                    <asp:TemplateField HeaderText="UnAssigned GTU" HeaderStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("ShortUniqueId") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                            </div>
                        </ContentTemplate>
                    </cc1:TabPanel>
                </cc1:TabContainer>

                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
    </table>
        

    <!--dialog box-->
    <div id="message" style="display:none;" title="Message"><%=Message %></div>
    </form>
</body>
</html>
