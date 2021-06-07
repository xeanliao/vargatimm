<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Users.aspx.cs" Inherits="GPS.Website.Users" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>TIMM &gt; Users</title>
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
    
    <script type="text/javascript" src="Javascript/usermanagement/gps.admin.user.js"></script>

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
    </form>
    <div class="head-container ui-widget-header ui-corner-all" style="width: 980px;">
        <div class="head-logo-container">
        </div>
        <div style="position: absolute; top: 9px; right: 6px;">
            Select Type             
            <select id="all-group-select" name="all-group-select" onchange='onGetAllUsers();' style='width:150px'></select>
            <a href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                onclick="javascript:onAddUser();"><span class="ui-icon ui-icon-newwin"></span>Add
                New User</a> <a href="javascript:void(0);" class="gps-toolbar-button ui-corner-all"
                    onclick="javascript:window.close();"><span class="ui-icon ui-icon-close"></span>
                    Save & Close</a>
        </div>
    </div>
    <br />
    <div style="margin-left: auto; margin-right: auto; width: 980px; font-size: 16px;
        font-weight: bold;">
        <span>Home&nbsp;&gt;&nbsp;Users</span>
    </div>
    <br />
    <div style="margin-left: auto; margin-right: auto; width: 980px;" id="tableDiv">
        <table id="test_table" cellspacing="0" cellpadding="0" border="0" class="display">
        </table>
    </div>
    <div id="user_dialog" title="Create New User">
        <p id="validateTips">
            Fields marked with * are required.</p>
        <form action="">
        <fieldset>
            <label for="username">
                User Name *</label>
            <input type="text" name="username" id="username" class="text ui-widget-content ui-corner-all" />
            <label for="fullname">
                Full Name *</label>
            <input type="text" name="fullname" id="fullname" class="text ui-widget-content ui-corner-all" />
            <label for="usercode">
                User Code *</label>
            <input type="text" name="usercode" id="usercode" class="text ui-widget-content ui-corner-all" />
            <label for="password">
                Password *</label>
            <input type="password" name="password" id="password" value="" class="text ui-widget-content ui-corner-all" />
            <label for="password">
                Confirm Password *</label>
            <input type="password" name="confirm_password" id="confirm_password" value="" class="text ui-widget-content ui-corner-all" />
            <%--<label for="role">Role</label>--%>
            <%--<select name="userrole" id="userrole"  visible="false" class="text ui-widget-content ui-corner-all" style="width:250px;">
            </select>&nbsp;&nbsp;&nbsp;&nbsp;--%>
            <%--<input id="is_admin" name="is_admin" type="checkbox" />&nbsp;&nbsp;&nbsp;&nbsp;--%>
            <label for="email">Email</label>
            <input type="text" name="email" id="email" value="" class="text ui-widget-content ui-corner-all" />
			<span>Enabled:</span>
            <input id="enabled" name="enabled" type="checkbox" checked="checked" />
            <label for="group">Types</label>
            <select size="5" id="list-group-name-new" name="list-group-name-new" multiple=multiple></select>
        </fieldset>
        </form>
    </div>
    <div id="edit_user_dialog" title="Edit User">
        <p id="edit_validateTips">
            Fields marked with * are required.</p>
        <form action="">
        <fieldset>
            <label for="edit_username">
                User Name *</label>
            <input type="text" name="edit_username" id="edit_username" disabled="disabled" class="text ui-widget-content ui-corner-all" />
            <input type="text" name="edit_userid" id="edit_userid" style="display:none;" class="text ui-widget-content ui-corner-all" />
            <label for="edit_fullname">
                Full Name *</label>
            <input type="text" name="edit_fullname" id="edit_fullname" class="text ui-widget-content ui-corner-all" />
            <label for="edit_usercode">
                User Code *</label>
            <input type="text" name="edit_usercode" id="edit_usercode" class="text ui-widget-content ui-corner-all" />
            <label for="edit_password">
                Password *</label>
            <input type="password" name="edit_password" id="edit_password" value="" class="text ui-widget-content ui-corner-all" />
            <label for="edit_confirm_password">
                Confirm Password *</label>
            <input type="password" name="edit_confirm_password" id="edit_confirm_password" value=""
                class="text ui-widget-content ui-corner-all" />
            <%--<label for="role" visible=false>Role</label>
            <select name="edit_userrole" id="edit_userrole" class="text ui-widget-content ui-corner-all" style="width:250px;" visible=false>
            </select>&nbsp;&nbsp;&nbsp;&nbsp;--%>
            
            <%--<input id="edit_is_admin" name="edit_is_admin" type="checkbox" />&nbsp;&nbsp;&nbsp;&nbsp;--%>
			<label for="edit_email">Email</label>
            <input type="text" name="edit_email" id="edit_email" value="" class="text ui-widget-content ui-corner-all" />
            <span>Enabled:</span>
            <input id="edit_enabled" name="edit_enabled" type="checkbox" />
            <label for="group">Types</label>
            <select size="5" id="list-group-name" name="list-group-name" multiple=multiple></select>
        </fieldset>
        </form>
    </div>

    <!--Confirmation dialog when deleting a user -->
    <div id="delete_confirm_dialog" title="TIMM" style="display: none;">
        <p>
            Are you sure you want to delete the account?</p>
    </div>
</body>
</html>
