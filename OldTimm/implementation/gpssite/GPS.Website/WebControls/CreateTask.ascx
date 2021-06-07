<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CreateTask.ascx.cs" Inherits="GPS.Website.WebControls.CreateTask" %>
<div class="create-task-dialog">
    <table style="height:400px;">
        <%--<tr>
            <td align="left">
                <label for="sel-task-campaigns">Campaigns</label>
            </td>
            <td align="left">
                <select id="sel-task-campaigns" onchange="onGetSubmaps();" class="text500"></select>
            </td>
        </tr>
        <tr>
            <td align="left">
                <label for="sel-task-submaps">Submaps</label>
            </td>
            <td align="left">
                <select id="sel-task-submaps" onchange="onGetDMs();" class="text500"></select>
            </td>
        </tr>--%>
        <tr>
            <td align="left">
                <input type="hidden" id="sel-task-id"/>
            </td>
            <td align="left">
                <input type="hidden" id="sel-task-dms"/>
            </td>
        </tr>
        <tr>
            <td align="left">
                <label for="txt-task-name">Name</label>
            </td>
            <td align="left">
                <input id="txt-task-name" class="text500" type="text" disabled/>
            </td>
        </tr>
        <tr>
            <td align="left">
                <label for="txt-task-date">Distribution Date</label>
            </td>
            <td align="left">
                <input id="txt-task-date" type="text" />
            </td>
        </tr>
        <tr>
            <td align="left">
                <label for="select-task-auditor">Select Auditor</label>
            </td>
            <td align="left">
                <select id="select-task-auditor" class="text500"></select>
            </td>
        </tr>
        
        
        
        <tr>
            <td align="left">
                <label for="tex-task-email">Email</label>
            </td>
            <td align="left">
                <input id="tex-task-email" type="text" class="text500"/>
            </td>
        </tr>
        <tr>
            <td align="left">
                <label for="tex-task-tel">Telephone</label>
            </td>
            <td align="left">
                <input id="tex-task-tel" type="text" class="text500"/>
            </td>
        </tr>
        <tr>
            <td align="left">
                <label for="tex-task-tel-post">Telecommunications Operator</label>
            </td>
            <td align="left">
                <select id="tex-task-tel-post" class="text500">
                    <option value="@message.alltel.com">Alltel</option>
                    <option value="@txt.att.net">AT&T</option>
                    <option value="@messaging.nextel.com">Nextel</option>
                    <option value="@messaging.sprintpcs.com">Sprint</option>
                    <option value="@tms.suncom.com">SunCom</option>
                    <option value="@tmomail.net">T-mobile</option>
                    <option value="@voicestream.net">VoiceStream</option>
                    <option value="@vtext.com">Verizon(text only)</option>
                </select>
            </td>
        </tr>
    </table>
    
    <%--<div>
        <div><label for="sel-task-campaigns">Campaigns</label>&nbsp;&nbsp;&nbsp;&nbsp;<select id="sel-task-campaigns" onchange="onGetSubmaps();" class="text500"></select></div>
        <div><label for="sel-task-submaps">Submaps</label>&nbsp;&nbsp;&nbsp;&nbsp;<select id="sel-task-submaps" onchange="onGetDMs();" class="text500"></select></div>
        <div><label for="sel-task-dms">Distribution maps</label>&nbsp;&nbsp;&nbsp;&nbsp;<select id="sel-task-dms" class="text500"></select></div>
    </div>
    
    <div><label for="txt-task-name">Name</label>&nbsp;&nbsp;&nbsp;&nbsp;<input id="txt-task-name" class="text500" type="text" /></div>
    <div><label for="txt-task-date">Distribution Date</label>&nbsp;&nbsp;&nbsp;&nbsp;<input id="txt-task-date" type="text" /></div>
    <div><label for="select-task-auditor">Select Auditor</label>&nbsp;&nbsp;&nbsp;&nbsp;<select id="select-task-auditor" class="text500"></select></div>--%>
</div>

<script type="text/javascript">
    //<![CDATA[
    var taskDatePicker;

    $(function() {
        taskDatePicker = $('#txt-task-date').datepicker();
        taskDatePicker.datepicker('setDate', new Date());
    });

    function GetTaskCreationDate() {
        return taskDatePicker.datepicker('getDate');
    }

    function BindTaskCreationDate(date) {
        taskDatePicker.datepicker('setDate', date);
    }
    //]]>
</script>
