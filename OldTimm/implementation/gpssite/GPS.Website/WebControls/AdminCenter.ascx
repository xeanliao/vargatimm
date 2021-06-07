<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminCenter.ascx.cs" Inherits="GPS.Website.WebControls.AdminCenter" %>
<div id="div-Admin" >
    <div class="midboder2 div-distribution-map-center-padding" style="height:320px">
        <div class="icon"><img src="Images/controlcenter/Icon_Admin.png" alt="" /></div>
        <div class="divcontent2">
            <div>
                <span class="boxtitle">Administration</span><br />
                <span class="boxtext">Administer groups,user accounts, <br />non-deliverables and GTUs.</span>
            </div>
            <div id="div-administration-list">
                <ul>
                    <%--<li onclick="javascript:GPS.AdminCenter.OnGroupsClick();">Group Management</li>
                    <div class="commenttext">Create, modify and delete group information.</div>--%>
                    <li id="UserManagementDiv" onclick="javascript:GPS.AdminCenter.OnUsersClick();" style="display:none">User Management</li>
                    <div class="commenttext">Create, modify and delete user accounts.</div>
                    <li id="NonDeliverablesDiv" onclick="javascript:GPS.AdminCenter.OnNonDeliverablesClick();" style="display:none">Non-Deliverables</li>
                    <div class="commenttext">You may manage non-deliverables by clicking here.</div>
                    <li id="GTUManagementDiv" onclick="javascript:GPS.AdminCenter.OnGtusClick();" style="display:none">GTU Management</li>
                    <div class="commenttext">Create, modify and delete GTUs.</div>
                    <li id="GTUAvailableDiv" onclick="javascript:GPS.AdminCenter.OnAvailableGTUListClick();" style="display:none">GTU Available List</li>
                    <div class="commenttext">Show available GTU list.</div>
                    <li onclick="showGtuBagManagement()" > GTU bag Management </li>
                    <div class="commenttext">Move GTUs to Bags</div>
                    <li id="Li1" onclick="javascript:openAuditorManagementWindow();" >Assign GTU-Bags to Auditors</li>
                    <div class="commenttext">Auditor Management</div>
                    <li id="Li2" onclick="javascript:openAdminDistributorWindow();" >Distributor Management</li>
                    <div class="commenttext">Add/Edit distributor company</div>
                </ul>
            </div>
        </div>
        <div class="clear"></div>
    </div>
</div>
