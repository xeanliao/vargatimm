<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Groups.aspx.cs" Inherits="GPS.Website.Groups" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
    <title>TIMM &gt; Groups</title>
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
            text-align: left;
        }
    </style>

    <script type="text/javascript" src="Javascript/jquery-1.3.2.js"></script>

    <script type="text/javascript" src="Javascript/jquery.ui.all.js"></script>

    <script type="text/javascript" src="Javascript/jquery.dataTables.min.js"></script>

    <script type="text/javascript" src="Javascript/gps.toolbar.js"></script>
    
    <script type="text/javascript" src="Javascript/usermanagement/gps.admin.group.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
            <Services>
                <asp:ServiceReference Path="GroupServices/GroupReaderService.svc" />
                <asp:ServiceReference Path="GroupServices/GroupWriterService.svc" />
            </Services>
        </asp:ScriptManager>
    </div>
    </form>
    <div class="head-container ui-widget-header ui-corner-all" style="width: 980px;">
        <div class="head-logo-container">
        </div>
        <div style="position: absolute; top: 9px; right: 6px;">
            <a href="javascript:void(0);" class="gps-toolbar-button ui-corner-all" onclick="javascript:onAddGroup();">
                <span class="ui-icon ui-icon-newwin"></span>Add New Group</a> <a href="javascript:void(0);"
                    class="gps-toolbar-button ui-corner-all" onclick="javascript:window.close();"><span
                        class="ui-icon ui-icon-close"></span>Save & Close</a>
        </div>
    </div>
    <br />
    <div style="margin-left: auto; margin-right: auto; width: 980px; font-size: 16px;
        font-weight: bold;">
        <span>Home&nbsp;&gt;&nbsp;Groups</span>
    </div>
    <br />
    <div style="margin-left: auto; margin-right: auto; width: 980px;">
        <table id="test_table" cellspacing="0" cellpadding="0" border="0" class="display">
        </table>
    </div>
    <div id="group_dialog" title="Create New Group">
        <p id="validateTips">
            Fields marked with * are required.</p>
        <form action="">
        <fieldset>
            <%--<label for="ID">
                Group ID  </label>
            <input type="text" name="ID" id="ID" disabled="disabled" class="text ui-widget-content ui-corner-all" />--%>
            <label for="name">
                Group Name *</label>
            <input type="text" name="name" id="name" class="text ui-widget-content ui-corner-all" />
            <label for="privilege">Privileges</label>
            <select size="8" id="list-privilege-name-new" name="list-privilege-name-new" multiple=multiple></select>
        </fieldset>
        </form>
    </div>
    <div id="edit_group_dialog" title="Edit Group">
        <p id="edit_validateTips">
            Fields marked with * are required.</p>
        <form action="">
        <fieldset>
           <label for="ID">
                Group ID  </label>
            <input type="text" name="edit_ID" id="edit_ID" disabled="disabled" class="text ui-widget-content ui-corner-all" />
            <label for="Name">
                Group Name *</label>
            <input type="text" name="edit_groupname" id="edit_groupname" class="text ui-widget-content ui-corner-all" />
            <label for="privilege">Privileges</label>
            <select size="8" id="list-privilege-name-edit" name="list-privilege-name-edit" multiple=multiple></select>
        </fieldset>
        </form>
    </div>

    <!--Confirmation dialog when deleting a group -->
    <div id="delete_confirm_dialog" title="TIMM" style="display: none;">
        <p>
            Are you sure you want to delete the account?</p>
    </div>
</body>
</html>
