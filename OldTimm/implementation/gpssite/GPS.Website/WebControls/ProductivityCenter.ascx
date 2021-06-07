<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProductivityCenter.ascx.cs" Inherits="GPS.Website.WebControls.ProductivityCenter" %>
<%@ Register Src="~/WebControls/CreateTask.ascx" TagName="CreateTaskDialog" TagPrefix="Timm" %>
<%@ Register Src="~/WebControls/AssignGTUsToTasks.ascx" TagName="AssignGTUsToTasksDialog" TagPrefix="Timm" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="ajax" %>

<div id="div-productivity-center">
    <div class="midboder2 div-distribution-map-center-padding">
        <div class="icon"><img src="Images/controlcenter/Icon_DM.png" alt="" /></div>
        <div class="divcontent2">
            <div>
                <span class="boxtitle">GPS Monitor</span><br />
                <span class="boxtext">Create and define monitors for customers.</span>
            </div>
            <div id="campaign-center-toolbar-productivity" class="control-center-toolbar">
                <%--<a id="tasknewA" href="javascript:void(0)" onclick="javascript:productivityCenter.OnNewTaskClicked()" style="display:none">New</a><label id="tasknewSep" style="display:none">&nbsp;|&nbsp;</label>--%>
                <a id="taskdeleteA" href="javascript:void(0)" onclick="javascript:productivityCenter.OnBulkDeleteTasks()" style="display:none">Dismiss to DM</a>
                <label id="assigngtuSep" style="display:none">|</label>&nbsp;
                <!--
                <a href="javascript:productivityCenter.OnBulkAssignGTUsToTask()" id="assigngtuA" style="display:none">Assign GTUs</a>
                <label id="markfinishsepdiv" style="display:none">&nbsp;|&nbsp;</label>
                -->
                <a id="markfinish" href="javascript:void(0)" onclick="javascript:productivityCenter.OnMarkFinishClicked()" style="display:none">Mark Finished</a>
            </div>
            <div id="productivity-center-campaign-list" style="display:none">
                <p id="productivity-center-loading" class="control-center-loading-text">
                    Loading the list...
                    <%--<a href="javascript:void(0)" >Edit</a><input type="checkbox" id="productivityItems" /><a href="Monitor.aspx?id=-56695807" target="_blank" style="text-decoration:underline;"><font color="black">081210-xxx-ad</font></a><a href="Monitor.aspx?id=-56695807" target="_blank" > &nbsp;&nbsp;Monitor</a>&nbsp;&nbsp;<a href="javascript:void(0)" >Time</a><br />
                    <a href="javascript:void(0)" >Edit</a><input type="checkbox" id="Checkbox1" /><a href="Monitor.aspx?id=-1477867006" target="_blank" style="text-decoration:underline;"><font color="black">082610-sss-uu</font></a><a href="Monitor.aspx?id=-1477867006" target="_blank"  > &nbsp;&nbsp;Monitor</a>&nbsp;&nbsp;<a href="javascript:void(0)" >Time</a><br />
                    <a href="javascript:void(0)" >Edit</a><input type="checkbox" id="Checkbox2" /><a href="Monitor.aspx?id=-1817324878" target="_blank" style="text-decoration:underline;"><font color="black">083010-ddd-op</font></a><a href="Monitor.aspx?id=2134788949" target="_blank"  > &nbsp;&nbsp;Monitor</a>&nbsp;&nbsp;<a href="javascript:void(0)" >Time</a><br />--%>
                </p>
            </div>
        </div>
    </div>
    <div id="div-create-task-dialog" style="display: none"><Timm:CreateTaskDialog ID="CreateTask" runat="server" /></div>
    <div id="div-assign-GTUs-task-dialog" style="display: none"><Timm:AssignGTUsToTasksDialog ID="AssignGTUsToTasks" runat="server" /></div>
    <div id="div-del-Task-Dialog" style="display:none"><div ><div class="create-campaign-dialog"><label for="list-user-name-task">Assign to</label></div><div><select size="10" id="list-user-name-task" name="list-user-name-task" style="width:300px;"></select></div>
    </div>    
    </div>
    
    <style type="text/css">
        /*Modal Popup*/
        .modalBackground {
	        background-color:Gray;
	        filter:alpha(opacity=70);
	        opacity:0.7;
        }

        .modalPopup {
	        background-color:#f0f0f0;
	        border-width:1px;
	        border-style:solid;
	        border-color:Gray;
	        padding:0px;
        }

        .modalPopup p {
            padding: 5px;
        }
    </style>
    
    <asp:Panel ID="pnlPop" runat="server" Style="display: none" CssClass="modalPopup">
        <table width="280px" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td style="background-color:Gray; text-align:center; font-weight:bold; padding:4px;">
                    Import GTU Info
                </td>
            </tr>
            <tr>
                <td style="background-color:White; height:100px; padding:4px">
                    <div style="text-align:left; width:100%; display:none">Task ID: <asp:TextBox ID="txtTaskID" runat="server"></asp:TextBox> </div>
                    <div style="vertical-align:top; height:100%">
                        <asp:FileUpload ID="FileUpload1" runat="server" Width="100%" />
                    </div>
                    
                    <div style="vertical-align:bottom; border-top:solid 1px gray; text-align:right">
                        <asp:Button ID="btnOK" runat="server" Text="Upload" style="display:none" />
                        <input type="button" value="Submit" onclick="return onOk()" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" />
                    </div>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <!-- programatically click this button when user choose a text file and click OK -->
    <div id="divImport" style="display:none">
        <asp:Button id="btnImport" runat="server" Text="Import" OnClick="btnImport_Click" />
        <a href="#" style="display:none;visibility:hidden;" 
           onclick="return false" ID="dummyLink" runat="server">dummy</a>               
    </div>
    <!-- ModalPopupExtender -->
    <ajax:ModalPopupExtender ID="ajaxModalEx" runat="server" 
        TargetControlID="dummyLink"
        PopupControlID="pnlPop"
        BackgroundCssClass="modalBackground" 
        DropShadow="true" 
        OkControlID="btnOK" 
        CancelControlID="btnCancel" 
        >
    </ajax:ModalPopupExtender>   
    <asp:Label ID="lblMessage" runat="server" EnableViewState="false"></asp:Label> 

    <script type="text/javascript">
    </script>
   
</div>