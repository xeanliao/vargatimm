<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignWalkers.ascx.cs" Inherits="GPS.Website.WebControls.Distribution.AssignWalkers" %>
<div id="assignwalkers-progressbar_section" style="display: none;text-align:center;">Loading...</div>
     <div id="walkers-users-list" class="ui-widget-content ui-corner-all"></div>
     <div id="walkermenu-div">
        &nbsp;&nbsp;<input type="button" name="Assign" id="btnAssignWalker"  value=" >> " onclick="javascript:OnAssignWalker();" /><br /><br />&nbsp;&nbsp;<input type="button" name="Unassign" id="btnUnAssignWalker"  value=" << " onclick="javascript:OnUnAssignWalker();" />
     </div>
     <div id="walkers-list" class="ui-widget-content ui-corner-all"></div>