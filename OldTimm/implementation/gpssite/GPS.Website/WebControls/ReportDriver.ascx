<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ReportDriver.ascx.cs" Inherits="GPS.Website.WebControls.ReportDriver" %>
<%--<table cellspacing="0" cellpadding="0" border="0" id="walkerDetailinfo" name="walkerDetailinfo">
--%>    <%--<tr>
        <th style="width:10%">Name</th>
        <th style="width:7%">Days</th>
        <th style="width:7%">Actual</th>
        <th style="width:7%">High</th>
        <th style="width:7%">Low</th>
        <th style="width:10%">YTD(optional)</th>
        <th style="width:7%">Actual</th>
        <th style="width:7%">High</th>
        <th style="width:7%">Low</th>
        <th style="width:10%">Lifttime(optional)</th>
        <th style="width:7%">Actual</th>
        <th style="width:7%">High</th>
        <th style="width:7%">Low</th>
    </tr>--%>
    <tr>
     <td colspan="13" style="text-align:left;">
       <b><asp:Label ID="lblDriverName" runat="server" Text=""></asp:Label></b>
     </td>
    </tr>
   <%-- <tr>
        <td style="width:10%"><b>Total Units Tracked:</b></td>
        <td style="width:7%"></td>
        <td style="width:7%"></td>
        <td style="width:7%"></td>
        <td style="width:7%"></td>
        <td style="width:10%"><b>Total Units Tracked:</b></td>
        <td style="width:7%"></td>
        <td style="width:7%"></td>
        <td style="width:7%"></td>
        <td style="width:10%"><b>Total Units Tracked:</b></td>
        <td style="width:7%"></td>
        <td style="width:7%"></td>
        <td style="width:7%"></td>
    </tr>--%>
    <tr>
        <td>&nbsp;&nbsp;&nbsp;&nbsp;Avg.Speed(MPH):</td>
        <td></td>
        <td><asp:Label ID="avgSpeedN" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="highSpeedN" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="lowSpeedN" runat="server" Text=""></asp:Label></td>
        <td>Avg.Speed(MPH):</td>
        <td><asp:Label ID="avgSpeedYTD" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="highSpeedYTD" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="lowSpeedYTD" runat="server" Text=""></asp:Label></td>
        <td>Avg.Speed(MPH):</td>
        <td><asp:Label ID="avgSpeedLF" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="highSpeedLF" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="lowSpeedLF" runat="server" Text=""></asp:Label></td>
    </tr>
    <tr>
        <td>&nbsp;&nbsp;&nbsp;&nbsp;Avg.Ground Covered(MILES):</td>
        <td></td>
        <td><asp:Label ID="avgGroundN" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="highGroundN" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="lowGroundN" runat="server" Text=""></asp:Label></td>
        <td>Avg.Ground Covered(MILES):</td>
        <td><asp:Label ID="avgGroundYTD" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="highGroundYTD" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="lowGroundYTD" runat="server" Text=""></asp:Label></td>
        <td>Avg.Ground Covered(MILES):</td>
        <td><asp:Label ID="avgGroundLF" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="highGroundLF" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="lowGroundLF" runat="server" Text=""></asp:Label></td>
    </tr>
    <tr>
        <td>&nbsp;&nbsp;&nbsp;&nbsp;Avg.Stops:</td>
        <td></td>
        <td><asp:Label ID="avgStopN" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="highStopN" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="lowStopN" runat="server" Text=""></asp:Label></td>
        <td>Avg.Stops:</td>
        <td><asp:Label ID="avgStopYTD" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="highStopYTD" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="lowStopYTD" runat="server" Text=""></asp:Label></td>
        <td>Avg.Stops:</td>
        <td><asp:Label ID="avgStopLF" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="highStopLF" runat="server" Text=""></asp:Label></td>
        <td><asp:Label ID="lowStopLF" runat="server" Text=""></asp:Label></td>
    </tr>
    <tr>
     <td colspan="13">&nbsp;</td>
    </tr>
<%--</table>--%>
    

