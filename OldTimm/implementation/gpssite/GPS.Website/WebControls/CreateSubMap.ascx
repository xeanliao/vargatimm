<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateSubMap.ascx.cs"
    Inherits="GPS.Website.WebControls.CreateSubMap" %>
    <div>
        Sub Map Name</div>
    <div>
        <input id="txt-sub-map-name" class="text350" type="text" /></div>
    <div>
        Select Areas</div>
    <div id="create-sub-map-dialog-body">
        <ul>
            <li><input type="checkbox" />Total:100 Percetage:80%</li>
            <li><input type="checkbox" />Total:100 Percetage:80%</li>
            <li><input type="checkbox" />Total:100 Percetage:80%</li>
            <li><input type="checkbox" />Total:100 Percetage:80%</li>
            <li><input type="checkbox" />Total:100 Percetage:80%</li>
            <li><input type="checkbox" />Total:100 Percetage:80%</li>
            <li><input type="checkbox" />Total:100 Percetage:80%</li>
            <li><input type="checkbox" />Total:100 Percetage:80%</li>
            <li><input type="checkbox" />Total:100 Percetage:80%</li>
        </ul>
    </div>
    <div id="create-sub-map-dialog-bottom"><input type=button class="button100" value="Save" />&nbsp;&nbsp;<input type=button class="button100" value=Cancel  onclick="javascript:HideDialog(submapdialog);"/></div>

