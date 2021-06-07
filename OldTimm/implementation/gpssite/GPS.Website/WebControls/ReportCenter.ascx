<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportCenter.ascx.cs" Inherits="GPS.Website.WebControls.ReportCenter" %>
<%--<%@ Register Src="~/WebControls/CreateCampaign.ascx" TagName="CreateCampaignDialog" TagPrefix="Timm" %>--%>

<div id="div-productivity-center" >
    <div class="midboder2 div-distribution-map-center-padding">
        <div class="icon"><img src="Images/controlcenter/Icon_Campaign.png" alt="" /></div>
        
        <div class="divcontent2">
           <div>
              <span class="boxtitle">Reports</span><br />
              <span class="boxtext">Report of campaigns for customers.</span><div class="clear">
            </div>                     
            <div id="campaign-center-toolbar-productivity" class="control-center-toolbar">
                <div id="divDismissToMonitor" style="display:none">
                    <asp:LinkButton ID="btnDismissToMonitor" runat="server" Text="Dismiss To Monitor" OnClick="btnDismissToMonitor_Click"></asp:LinkButton>
                    <input type="hidden" id="txtSelectedTaskIdList" runat="server" />
                </div>
            </div>
            <div id="campaign-center-report-list" style="display:none">
                <p id="productivity-center-loading" class="control-center-loading-text" >
                    Loading the list...
                    <%--<a href="javascript:void(0)" >Edit</a><input type="checkbox" id="productivityItems" /><a href="Monitor.aspx?id=-56695807" target="_blank" style="text-decoration:underline;"><font color="black">081210-xxx-ad</font></a><a href="Monitor.aspx?id=-56695807" target="_blank" > &nbsp;&nbsp;Monitor</a>&nbsp;&nbsp;<a href="javascript:void(0)" >Time</a><br />
                    <a href="javascript:void(0)" >Edit</a><input type="checkbox" id="Checkbox1" /><a href="Monitor.aspx?id=-1477867006" target="_blank" style="text-decoration:underline;"><font color="black">082610-sss-uu</font></a><a href="Monitor.aspx?id=-1477867006" target="_blank"  > &nbsp;&nbsp;Monitor</a>&nbsp;&nbsp;<a href="javascript:void(0)" >Time</a><br />
                    <a href="javascript:void(0)" >Edit</a><input type="checkbox" id="Checkbox2" /><a href="Monitor.aspx?id=-1817324878" target="_blank" style="text-decoration:underline;"><font color="black">083010-ddd-op</font></a><a href="Monitor.aspx?id=2134788949" target="_blank"  > &nbsp;&nbsp;Monitor</a>&nbsp;&nbsp;<a href="javascript:void(0)" >Time</a><br />--%>
                </p>
            </div>
          </div>
       </div>    
  </div>
</div>