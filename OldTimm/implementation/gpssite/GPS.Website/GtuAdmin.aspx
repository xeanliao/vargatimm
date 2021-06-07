<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GtuAdmin.aspx.cs" Inherits="GPS.Website.GtuAdmin" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>TIMM &gt;GTUS Management</title>
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
        label, input.text, select.text
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
            text-align: left;
        }
        select.text
        {
            margin-bottom: 4px;
            width: 95%;
            padding: .4em;
            font-size: 11px;
        }
    </style>

    <script type="text/javascript" src="Javascript/jquery-1.3.2.js"></script>

    <script type="text/javascript" src="Javascript/jquery.ui.all.js"></script>

    <script type="text/javascript" src="Javascript/jquery.dataTables.min.js"></script>

    <script type="text/javascript">
        //<![CDATA[
        var SysBrowser = {};
        SysBrowser.ie;
        SysBrowser.firefox;
        SysBrowser.chrome;
        SysBrowser.opera;
        SysBrowser.safari;
        var ua = navigator.userAgent.toLowerCase();
        var s;
        function GetUrlParms() {
            var args = new Object();
            var query = location.search.substring(1);
            var pairs = query.split("&");
            for (var i = 0; i < pairs.length; i++) {
                var pos = pairs[i].indexOf('=');
                if (pos == -1) continue;
                var argname = pairs[i].substring(0, pos);
                var value = pairs[i].substring(pos + 1);
                args[argname] = unescape(value);
            }
            return args;
        }
        var args = GetUrlParms();
        var AssignNameToGTUFlag = args["AssignNameToGTUFlag"];
        (s = ua.match(/msie ([\d.]+)/)) ? SysBrowser.ie = s[1] :
        (s = ua.match(/firefox\/([\d.]+)/)) ? SysBrowser.firefox = s[1] :
        (s = ua.match(/chrome\/([\d.]+)/)) ? SysBrowser.chrome = s[1] :
        (s = ua.match(/opera.([\d.]+)/)) ? SysBrowser.opera = s[1] :
        (s = ua.match(/version\/([\d.]+).*safari/)) ? SysBrowser.safari = s[1] : 0;
        //]]>
    </script>

    <script type="text/javascript" src="Javascript/gps.js"></script>

    <script type="text/javascript" src="Javascript/gps.loading.js"></script>

    <script type="text/javascript" src="Javascript/gps.toolbar.js"></script>

    <script type="text/javascript" src="Javascript/ajaxfileupload.js"></script>

    <script type="text/javascript" src="Javascript/track/track.gtu.js"></script>

    <script type="text/javascript">
    //<!CDATA[
        var testTable, aGtus = [], actionNode;

        function onChangedUser() {
            if ($('#edit_user').val() != 0) {
                var userReader = new TIMM.Website.UserServices.UserReaderService();
                userReader.GetUserById($('#edit_user').val(), onGetSelectedUserSuccess, null, null);
            } else {
                $("#userDetailsDiv").html("");
                $("#userDetailsDiv").css("border", "");
            }
        }

        function onGetSelectedUserSuccess(r) {
            
            if(r!=null){
                var userDetailsCon = "</br/>";
                var taskReader = new TIMM.Website.TaskServices.TaskReaderService();
                taskReader.GetTaskGtuMappingsByUser(r.Id, function(selectedTaskList) {
                    if (selectedTaskList != null && selectedTaskList.length > 0) {
                        for (var i = 0; i < selectedTaskList.length; i++) {
                            userDetailsCon = userDetailsCon + "Task Name: " + selectedTaskList[i].Name + "<br/>";
                            userDetailsCon = userDetailsCon + "Task Date: " + selectedTaskList[i].Date + "<br/><br/>";
                        }
                        
                        $("#userDetailsDiv").html(userDetailsCon);
                        $("#userDetailsDiv").css("border", "1px black dashed");
                        $("#userDetailsDiv").css("font-size", "9px");
                        $("#userDetailsDiv").css("width", "300px");
                        $("#userDetailsDiv").css("overflow", "auto");
                    } else {
                       
                        $("#userDetailsDiv").html("");
                        $("#userDetailsDiv").css("border", "");
                    }

                });
                //alert("c");
                
            }
        }

        
        function onChangedGroup() {
            var userReader = new TIMM.Website.UserServices.UserReaderService();
            userReader.GetAllUsersByGroup($('#edit_user_group').val(), onGetAllUsersSuccess, null, null);

        }

        function onGetAllUsersSuccess(r) {
            $("#edit_user").empty();
            $("<option value='" + "0" + "'>" + "Please Select User" + "</option>").appendTo("#edit_user");
            for (var i = 0; i < r.length; i++) {
               $("<option value='" + r[i].Id + "'>" + r[i].FullName + "</option>").appendTo("#edit_user");
            }
        }
        
        function onGetAllGtus() {
            var gtusReader = new TIMM.Website.TrackServices.GtuReaderService();
            gtusReader.GetAllGtus(onGetAllGtusSuccess, null, null);
        }

        function onGetAllMonitors() {
            var monitorsReader = new TIMM.Website.TaskServices.TaskReaderService();
            monitorsReader.GetAllTasks(onGetAllMonitorsSuccess, null, null);
        }

        function onChangedMonitor() {
            if ($("#filterMonitor").val()) {
                if ($("#filterMonitor").val() != 0) {
                    var monitorsReader = new TIMM.Website.TaskServices.TaskReaderService();
                    monitorsReader.GetTask($("#filterMonitor").val(), function(selectedTask) {
                        var taskinfo = "<table><tr><td>Task Name:</td><td>" + selectedTask.Name + "</td></tr><tr><td>Task Date:</td><td>" + selectedTask.Date + "</td></tr></table>"
                        $("#detailsinfo").html(taskinfo);
                        $("#detailsinfo").css("border", "1px black dashed");
                        //change gtu list
                        var mappinglen = selectedTask.Taskgtuinfomappings.length;
                        var gtu = null;
                        var gtulist = new Array();
                        for (var i = 0; i < mappinglen; i++) {
                            gtu = selectedTask.Taskgtuinfomappings[i].GTU;
                            gtulist.push(gtu);
                        }
                        $("#gtulistDiv").html("<table id=\"test_table\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" class=\"display\"></table>");
                        onGetAllGtusSuccess(gtulist);
                    }, function(e) { alert(e); });
                } else {
                    $("#detailsinfo").html("");
                    $("#detailsinfo").css("border", "");
                    $("#gtulistDiv").html("<table id=\"test_table\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" class=\"display\"></table>");
                    var gtusReader = new TIMM.Website.TrackServices.GtuReaderService();
                    gtusReader.GetAllGtus(onGetAllGtusSuccess, null, null);    
                }
            }
        }

        function fnRemoveSelected(tableLocal) {
            $(tableLocal.fnSettings().aoData).each(function() {
                $(this.nTr).removeClass('row_selected');
            });
        }

        function fnSelectRowByActionNode(tableLocal, actionNodeLocal) {
            fnRemoveSelected(tableLocal);
            $(actionNodeLocal.parentNode.parentNode).addClass('row_selected');
        }

        function fnSelectRowWithDelay(actionNodeLocal) {
            actionNode = actionNodeLocal;
            setTimeout('fnSelectRowByActionNode(testTable, actionNode)', 20);
        }

        function fnGetSelected(tableLocal) {
            var aReturn = new Array();
            var aTrs = tableLocal.fnGetNodes();

            for (var i = 0; i < aTrs.length; i++) {
                if ($(aTrs[i]).hasClass('row_selected')) {
                    aReturn.push(aTrs[i]);
                }
            }
            return aReturn;
        }

        function onAssignGtu(obj, uniqueId) {
            
            // Make the row selected
            fnSelectRowWithDelay(obj);
            var gtuReader = new TIMM.Website.TrackServices.GtuReaderService();
            var currentUser;
            var UId;
            gtuReader.GetGtu(uniqueId, function (r) {
              
                // Assign dialog field values and show the dialog
                $('#edit_uniqueid').val(r.UniqueID);
                $('#edit_model').val(r.Model);
                $('#edit_gtuid').val(r.Id);
                currentUser = r.UserId;
                UId = r.UniqueID;
                if (UId.length > 6) {
                    var len = UId.length;
                    UId = UId.substr(len - 6, 6);
                }
                $('#edit_uniqueid_show').val(UId);
            });

            // init the edit_user select of edit_gtu_dialog div
            var userReader = new TIMM.Website.UserServices.UserReaderService(); 

            userReader.GetAllUsersWithoutAssignedGtu(onGetAllUsersSuccess, null, null);

//            $("#edit_user").empty();
//            $("<option value='" + "0" + "'>" + "Please Select User" + "</option>").appendTo("#edit_user");

            $('#edit_gtu_dialog').dialog('open');
//            var userReader = new TIMM.Website.UserServices.UserReaderService();
//            userReader.GetAllUsers(function(r) {
//                // Assign dialog field values and show the dialog
//                $("#edit_user").empty();
//                $("<option value='" + "0" + "'>" + "Please Select User" + "</option>").appendTo("#edit_user");
//                for (var i = 0; i < r.length; i++) {
//                    if (currentUser == r[i].Id) {

//                        $("<option value='" + r[i].Id + "' selected>" + r[i].FullName + "</option>").appendTo("#edit_user");
//                    }
//                    else {
//                        $("<option value='" + r[i].Id + "'>" + r[i].FullName + "</option>").appendTo("#edit_user");
//                    }
//                }
//                $('#edit_gtu_dialog').dialog('open');
//            }, null, null);
        }

        function onDeleteGtu(obj, uniqueId) {
            // Make the row selected
            fnSelectRowWithDelay(obj);

            // Show confirmation dialog
            $("#delete_confirm_dialog").dialog({
                autoOpen: false,
                modal: true,
                buttons: {
                    'Yes': function() {
                        var gtuWriter = new TIMM.Website.TrackServices.GtuWriterService();
                        gtuWriter.DeleteGtu(uniqueId, function() {
                            var anSelected = fnGetSelected(testTable);
                            var iRow = testTable.fnGetPosition(anSelected[0]);
                            testTable.fnDeleteRow(iRow);
                        }, null, null);

                        $(this).dialog('destroy');
                    },
                    'No': function() {
                        $(this).dialog('close');
                    }
                }
            });
            $("#delete_confirm_dialog").dialog('open');
        }

        function onUnassignGtu(obj, uniqueId) {
            // Make the row selected
            fnSelectRowWithDelay(obj);

            // Show confirmation dialog
            $("#unassign_confirm_dialog").dialog({
                autoOpen: false,
                modal: true,
                buttons: {
                    'Yes': function () {
                        var gtuReader = new TIMM.Website.TrackServices.GtuReaderService();

                        gtuReader.GetGtu(uniqueId, function (gtu) {
                            var gtuWriter = new TIMM.Website.TrackServices.GtuWriterService();

                            gtu.UserId = -1;

                            gtuWriter.UpdateGtu(gtu, function (r) {
                                var anSelected = fnGetSelected(testTable);
                                var iRow = testTable.fnGetPosition(anSelected[0]);
                                testTable.fnUpdate(r.UserName, iRow, 2);
                            }, null, null);
                        });

                        $(this).dialog('destroy');
                    },
                    'No': function () {
                        $(this).dialog('close');
                    }
                }
            });
            $("#unassign_confirm_dialog").dialog('open');
        }

        function onGetAllMonitorsSuccess(result) {
            $("#filterMonitor").empty();
            $("<option value='" + "0" + "'>" + "Please Select Monitor" + "</option>").appendTo("#filterMonitor");
            for (var i = 0; i < result.length; i++) {
                $("<option value='" + result[i].Id + "'>" + result[i].Name + "</option>").appendTo("#filterMonitor");
            }
        }

        function onGetOneMonitorSuccess(result) {
//            $("#filterMonitor").empty();
//            $("<option value='" + "0" + "'>" + "Please Select Monitor" + "</option>").appendTo("#filterMonitor");
//            for (var i = 0; i < result.length; i++) {
//                $("<option value='" + result[i].Id + "'>" + result[i].Name + "</option>").appendTo("#filterMonitor");
//            }
        }

        function onGetAllGtusSuccess(result) {
            var fnBoolRender = function(obj) {
                var sReturn = obj.aData[obj.iDataColumn];
                return sReturn ? 'Yes' : 'No';
            }
            var fnNullRender = function(obj) {
                var sReturn = obj.aData[obj.iDataColumn];
                return (sReturn == null) ? '' : sReturn;
            }
            var fnActionsRender = function(obj) {
                var format;
                if (AssignNameToGTUFlag) {
                    format = [
                    '<a href="javascript:void(0);" onclick="javascript:onAssignGtu(this, \'{3}\')" id="GtuUserDiv">Assign</a>',
                        '&nbsp;&nbsp;',
                        '&nbsp;&nbsp;',
                        '<a href="javascript:void(0);" onclick="javascript:onDeleteGtu(this, \'{4}\')">Delete</a>',
                         '&nbsp;&nbsp;',
                        '&nbsp;&nbsp;',
                        '<a href="javascript:void(0);" onclick="javascript:onUnassignGtu(this, \'{5}\')">Unassign</a>',
                    ].join('');
                }else{
                    format = [
                        '<a href="javascript:void(0);" onclick="javascript:onDeleteGtu(this, \'{4}\')">Delete</a>'
                    ].join('');
                }

                var sReturn = obj.aData[obj.iDataColumn];
                return format.replace('{3}', sReturn).replace('{4}', sReturn).replace('{5}', sReturn);
            }
            aGtus = null;
            aGtus = [];
            for (var i = 0; i < result.length; i++) {

                var r = result[i];
                var UId;
                UId =r.UniqueID;
                if (UId.length > 6) {
                    var len = UId.length;
                    UId = UId.substr(len - 6, 6);
                }
                aGtus.push([
                    UId,
                    r.Model,
                    r.UserName,
                    r.IsEnabled,
                    r.UniqueID
                ]);
            }

            testTable = $('#test_table').dataTable({
                'bJQueryUI': true,
                'sPaginationType': 'full_numbers',
                'bSort': true,
                'aaData': aGtus,
                'bAutoWidth': false,
                'aoColumns': [{
                    'sTitle': 'GTU Unique ID'
                }, {
                    'sTitle': 'Model'
                }, {
                    'sTitle': 'User',
                    "fnRender": fnNullRender
                }, {
                    'sTitle': 'Enabled',
                    "fnRender": fnBoolRender
                }, {
                    'sTitle': 'Actions',
                    'sWidth': '100px',
                    'fnRender': fnActionsRender
}]
                });

                // Round input fields
                $('.fg-toolbar input').addClass('ui-corner-all');
                $('.fg-toolbar input').addClass('ui-widget-content');

                // Fix the display bug in MS IE
                $('#test_table thead tr th').each(function() {
                    if ($.browser.msie) {
                        $(this).css('position', 'relative');
                        var icon = $('.css_right', this)[0];
                        $(icon).css('position', 'absolute');
                        $(icon).css('top', '2px');
                        $(icon).css('right', '0px');
                    }
                });

                // Highlight selected row
                $("#test_table tbody").click(function(event) {
                    $(testTable.fnSettings().aoData).each(function() {
                        $(this.nTr).removeClass('row_selected');
                    });
                    $(event.target.parentNode).addClass('row_selected');
                });
            }
    //]]>
    </script>

</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server">
        <Services>
            <asp:ServiceReference Path="TrackServices/GtuReaderService.svc" />
            <asp:ServiceReference Path="TrackServices/GtuWriterService.svc" />
            <asp:ServiceReference Path="UserServices/UserReaderService.svc" />
            <asp:ServiceReference Path="UserServices/UserWriterService.svc" />
            <asp:ServiceReference Path="TaskServices/TaskReaderService.svc" />
        </Services>
    </asp:ScriptManager>
    <div class="head-container ui-widget-header ui-corner-all" style="width: 980px;">
        <div class="head-logo-container">
        </div>
        <div style="position: absolute; top: 9px; right: 386px;">
             Select Monitor
            <select id="filterMonitor" name="filterMonitor" onchange="onChangedMonitor();">
            </select>
        </div>
        <div style="position: absolute; top: 9px; right: 6px;">
            
            <a href="javascript:void(0);" class="gps-toolbar-button ui-corner-all" onclick="javascript:OnMenuClick('gtu');">
                <span class="ui-icon ui-icon-newwin"></span>Load GTUs</a> 
                
                <a href="javascript:void(0);" class="gps-toolbar-button ui-corner-all" onclick="javascript:window.close();"><span
                        class="ui-icon ui-icon-close"></span>Save & Close</a>
        </div>
    </div>
    <br />
    <div style="margin-left: auto; margin-right: auto; width: 980px; font-size: 16px;
        font-weight: bold;">
        <span>Home&nbsp;&gt;&nbsp;GTUS<div id="detailsinfo" name="detailsinfo" style="margin-left: 600px; width: 300px; font-size: 10px;"></div></span>
    </div>
    <br />
    <div style="margin-left: auto; margin-right: auto; width: 980px;" id="gtulistDiv" name="gtulistDiv">
        <table id="test_table" cellspacing="0" cellpadding="0" border="0" class="display">
        </table>
    </div>
    <div id="edit_gtu_dialog" title="Assign GTU">
        <p id="edit_validateTips">
            Please select a user to assign the GTU.</p>
        <form action="">
        <fieldset>
            <label for="edit_uniqueid">
                Unique ID</label>
            <hiden type="text" name="edit_uniqueid" id="edit_uniqueid" disabled="disabled" class="text ui-widget-content ui-corner-all" />
            <input type="text" name="edit_uniqueid_show" id="edit_uniqueid_show" disabled="disabled" class="text ui-widget-content ui-corner-all" />
            <label for="edit_model">
                Model</label>
            <input type="text" name="edit_model" id="edit_model" class="text ui-widget-content ui-corner-all"
                disabled="disabled" />
                <input type="text" name="edit_gtuid" id="edit_gtuid" style="display:none" class="text ui-widget-content ui-corner-all" />
            <%--<label for="edit_enable">
                User Group</label>
            <select name="edit_user_group" id="edit_user_group" class="text ui-widget-content ui-corner-all" onchange="onChangedGroup();">
                <option value="0">Please Select Group</option>
                <option value="50">Auditor</option>
                <option value="48">Driver</option>
                <option value="1">Walker</option>
            </select>--%>

            <label for="edit_enable">
                User</label>
            <select name="edit_user" id="edit_user" class="text ui-widget-content ui-corner-all" onchange="onChangedUser();">
            </select>
            <div id="userDetailsDiv" >                
            </div>
        </fieldset>
        </form>
    </div>
    <!--Confirmation dialog when deleting a user -->
    <div id="delete_confirm_dialog" title="TIMM" style="display: none;">
        <p>
            Are you sure you want to delete this GTU?</p>
    </div>

    <!--Confirmation dialog when deleting a user -->
    <div id="unassign_confirm_dialog" title="TIMM" style="display: none;">
        <p>
            Are you sure you want to unassgin this GTU?</p>
    </div>
    </form>
</body>
</html>

<script type="text/javascript">
    function updateTips(t) {
        $('#validateTips').text(t).effect("highlight", {}, 1500);
    }

    function edit_updateTips(t) {
        $('#edit_validateTips').text(t).effect("highlight", {}, 1500);
    }

    function checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            //o.addClass('ui-state-error');
            updateTips("Length of " + n + " must be between " + min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    }

    function edit_checkLength(o, n, min, max) {
        if (o.val().length > max || o.val().length < min) {
            //o.addClass('ui-state-error');
            edit_updateTips("Length of " + n + " must be between " + min + " and " + max + ".");
            return false;
        } else {
            return true;
        }
    }

    $(document).ready(function () {
        //get all monitors
        onGetAllMonitors();

        // Populate all users in the user list
        onGetAllGtus();


        // Fields
        var edit_uniqueid = $('#edit_uniqueid'),
                edit_model = $('#edit_model'),
                edit_user = $('#edit_user');
        edit_user_group = $('#edit_user_group');
        edit_gtuid = $('#edit_gtuid');
        var edit_allFields = $([])
                .add(edit_uniqueid)
                .add(edit_model)
                .add(edit_user)
                .add(edit_gtuid);

        $("#edit_gtu_dialog").dialog({
            bgiframe: false,
            autoOpen: false,
            height: 500,
            width: 400,
            modal: true,
            resizable: false,
            buttons: {
                'Save Changes': function () {
                    var bValid = true;
                    edit_allFields.removeClass('ui-state-error');

                    bValid = bValid && (edit_user.val() != '0') && (edit_user_group.val() != '0');
                    if (!bValid) {
                        edit_updateTips('Please select a group and a user before you submit the change.');
                        return;
                    }

                    if (bValid) {
                        var uniqueID = edit_uniqueid.val();
                        var userID = $("#edit_user").val();
                        var gtuID = edit_gtuid.val();
                        var gtu = {
                            Id: gtuID,
                            UniqueID: edit_uniqueid.val(),
                            Model: '',
                            IsEnabled: true,
                            UserId: $("#edit_user").val()
                        };

                        var gtuWriter = new TIMM.Website.TrackServices.GtuWriterService();

                        gtuWriter.UpdateGtu(gtu, function (r) {
                            var anSelected = fnGetSelected(testTable);
                            var iRow = testTable.fnGetPosition(anSelected[0]);
                            testTable.fnUpdate(r.UserName, iRow, 2);

                        }, null, null);

                        $(this).dialog('close');
                    }

                },
                Cancel: function () {
                    $(this).dialog('close');
                }
            },
            close: function () {
                edit_allFields.val('').removeClass('ui-state-error');
            }
        });
    });
</script>

