<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignDrivers.ascx.cs" Inherits="GPS.Website.WebControls.Distribution.AssignDrivers" %>
<div id="assigndrivers-progressbar_section" style="display: none;text-align:center;">Loading...</div>

     <div id="users-list" class="ui-widget-content ui-corner-all"></div>
     <div id="drivermenu-div">
        &nbsp;&nbsp;<input type="button" name="Assign" id="btnAssign"  value=" >> " onclick="javascript:OnAssignDriver();" /><br /><br />&nbsp;&nbsp;<input type="button" name="Unassign" id="btnUnAssign"  value=" << " onclick="javascript:OnUnAssignDriver();" />
     </div>
     <div id="drivers-list" class="ui-widget-content ui-corner-all"></div>
