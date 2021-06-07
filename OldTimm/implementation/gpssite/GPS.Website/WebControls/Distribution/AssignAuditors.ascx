<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssignAuditors.ascx.cs" Inherits="GPS.Website.WebControls.Distribution.AssignAuditors" %>
<div id="assignauditors-progressbar_section" style="display: none;text-align:center;">Loading...</div>

     <div id="auditor-users-list" class="ui-widget-content ui-corner-all"></div>
     <div id="auditormenu-div">
        &nbsp;&nbsp;<input type="button" name="Assign" id="btnAssignAuditor"  value=" >> " onclick="javascript:OnAssignAuditor();" /><br /><br />&nbsp;&nbsp;<input type="button" name="Unassign" id="btnUnAssignAuditor"  value=" << " onclick="javascript:OnUnAssignAuditor();" />
     </div>
     <div id="auditors-list" class="ui-widget-content ui-corner-all"></div>
