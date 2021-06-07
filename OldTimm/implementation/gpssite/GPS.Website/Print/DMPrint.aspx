<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DMPrint.aspx.cs"
    Inherits="GPS.Website.Print.DMPrint" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Distribution Print</title>
    <style>
        body
        {
            font-family: Arial, Verdana;
            font-size: 15px;
            margin: 0;
            padding: 0;
            letter-spacing: 1px;
        }
        table
        {
            width: 100%;
        }
        table caption
        {
            text-transform: uppercase;
            font-size: 24px;
            font-weight: bold;
            padding: 4px;
            background-color: #94b43d;
        }
        hr
        {
            page-break-before: always;
            margin: 0;
            padding: 0;
            height: 0;
        }
        .pagebreaker
        {
            page-break-before: always;
            margin: 0;
            padding: 0;
            height: 0;
        }
        .rowlabel
        {
            width: 575px;
            font-weight: bold;
            vertical-align: top;
        }
        .collabel
        {
            font-weight: bold;
        }
        .maptable td
        {
            border: solid 1px #94b43d;
        }
        .spaceline
        {
            height: 10px;
        }
        .biglabel
        {
            font-weight: bold;
        }
        #firstpage
        {
            font-size: 23px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <%--<asp:Panel ID="plCoverPage" runat="server">
            <table id="firstpage" style="text-align: center;" cellpadding="0" cellspacing="2">
                <caption>
                    This Custom Distribution Map is Presented to:</caption>
                <tbody>
                    <tr>
                        <td style="height: 80px;">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Image ID="imgLogo" ImageUrl="~/Images/norms-logo.png" Height="70px" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 20px;">
                        </td>
                    </tr>
                    <tr>
                        <td class="biglabel">
                            Client Name:
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Literal ID="ltlClient" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 20px;">
                        </td>
                    </tr>
                    <tr>
                        <td class="biglabel">
                            Created for:
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Literal ID="ltlCreatedFor" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 20px;">
                        </td>
                    </tr>
                    <tr>
                        <td class="biglabel">
                            Created on:
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Literal ID="ltlCreatedOn" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 80px;">
                        </td>
                    </tr>
                    <tr>
                        <td class="biglabel">
                            Presented by:
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 20px;">
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Image ImageUrl="~/Images/vargainc-logo.png" Height="90px" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 20px;">
                        </td>
                    </tr>
                    <tr>
                        <td class="biglabel">
                            Master Campaign #:
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Literal ID="ltlMasterCampaign" runat="server"></asp:Literal>
                        </td>
                    </tr>
                    <tr>
                        <td style="height: 20px;">
                        </td>
                    </tr>
                    <tr>
                        <td class="biglabel">
                            Created by:
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Literal ID="ltlCreatedBy" runat="server"></asp:Literal>
                        </td>
                    </tr>
                </tbody>
            </table>
            <div class="pagebreaker">
            </div>
        </asp:Panel>--%>
        <%--<table id="campaign-summary" cellspacing="0" cellpadding="4">
            <caption>
                Campaign Summary
            </caption>
            <tbody>
                <tr>
                    <td class="rowlabel">
                        MASTER CAMPAIGN #:
                    </td>
                    <td>
                        <asp:Literal ID="ltlCampaign" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="rowlabel">
                        CLIENT NAME:
                    </td>
                    <td>
                        <asp:Literal ID="ltlClientName" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="rowlabel">
                        CONTACT NAME:
                    </td>
                    <td>
                        <asp:Literal ID="ltlContactName" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="rowlabel">
                        TARGETING METHOD:
                    </td>
                    <td style="font-family: Arial; font-size: 15px;">
                        <asp:Literal ID="ltlTargetingMethod" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="rowlabel">
                        TOTAL HOUSEHOLDS:
                    </td>
                    <td>
                        <asp:Literal ID="ltlTotal" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="rowlabel">
                        TARGET HOUSEHOLDS:
                    </td>
                    <td>
                        <asp:Literal ID="ltlCount" runat="server"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td class="rowlabel">
                        PENETRATION:
                    </td>
                    <td>
                        <asp:Literal ID="ltlPen" runat="server"></asp:Literal>
                    </td>
                </tr>
            </tbody>
        </table>
        <div class="spaceline">
        </div>--%>
        <%--<table class="maptable" cellspacing="0" cellpadding="0">
            <caption>
                Campaign Summary Map
            </caption>
            <tbody>
                <tr>
                    <td colspan="2" style="padding: 0px 1px 0px 1px;">
                        <asp:Image ID="imgCampaignMap" Width="100%" Style="margin-bottom: -2px;" BorderStyle="None"
                            runat="server" />
                    </td>
                </tr>
                <tr>
                    <td style="width: 90%; border-style: none; vertical-align: top;">
                        <asp:Repeater ID="rptColors" runat="server">
                            <HeaderTemplate>
                                COLOR LEGEND<br />
                                <table cellpadding="0" cellspacing="0" border="0" style="border-style: none; margin-top: 10px;">
                                    <tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <td style="border-style: none;">
                                    <div style='height: 12px; width: 25px; background-color: <%# Eval("ColorString")%>;'>
                                </td>
                                <td style="border-style: none;">
                                    </div>
                                    <%# Eval("Name")%>
                                    (<%# Convert.ToInt32((Convert.ToDouble(Eval("Min")) * 100)) %>% -
                                    <%# Convert.ToInt32((Convert.ToDouble(Eval("Max")) * 100)) %>%)
                                </td>
                                <td style="width: 10px; border-style: none;">
                                </td>
                            </ItemTemplate>
                            <FooterTemplate>
                                </tr></table></FooterTemplate>
                        </asp:Repeater>
                    </td>
                    <td style="width: 10%; text-align: right; border-style: none;">
                        <asp:Image ImageUrl="~/Images/direction-legend.png" Height="50px" runat="server" />
                    </td>
                </tr>
            </tbody>
        </table>--%>
        <%--<asp:Repeater ID="rptSubMapsSummary" runat="server">
            <HeaderTemplate>
                <div class="pagebreaker">
                </div>
                <table id="campaign-submaps-summary" cellspacing="0" cellpadding="4">
                    <caption>
                        Summary of Distribution Maps</caption>
                    <tbody>
                        <tr style="background-color: #eeeeee;">
                            <td class="collabel" style="width: 10%;">
                                #
                            </td>
                            <td class="collabel" style="width: 30%;">
                                DISTRIBUTION MAP NAME
                            </td>
                            <td class="collabel" style="width: 20%; text-align: right;">
                                TOTAL H/H
                            </td>
                            <td class="collabel" style="width: 20%; text-align: right;">
                                TARGET H/H
                            </td>
                            <td class="collabel" style="width: 20%; text-align: right;">
                                PENETRATION
                            </td>
                        </tr>
            </HeaderTemplate>
            <ItemTemplate>
                <tr>
                    <td>
                        <%# Eval("Id")%>
                    </td>
                    <td>
                        <%# Eval("Name")%>
                    </td>
                    <td style="text-align: right;">
                        <%# Eval("Total")%>
                    </td>
                    <td style="text-align: right;">
                        <%# Eval("Count")%>
                    </td>
                    <td style="text-align: right;">
                        <%# Eval("Pen")%>
                    </td>
                </tr>
            </ItemTemplate>
            <AlternatingItemTemplate>
                <tr style="background-color: #eeeeee;">
                    <td>
                        <%# Eval("Id")%>
                    </td>
                    <td>
                        <%# Eval("Name")%>
                    </td>
                    <td style="text-align: right;">
                        <%# Eval("Total")%>
                    </td>
                    <td style="text-align: right;">
                        <%# Eval("Count")%>
                    </td>
                    <td style="text-align: right;">
                        <%# Eval("Pen")%>
                    </td>
                </tr>
            </AlternatingItemTemplate>
            <FooterTemplate>
                </tbody> </table></FooterTemplate>
        </asp:Repeater>--%>
        <asp:Repeater ID="rptDM" runat="server" OnItemDataBound="rptDM_ItemDataBound">
            <ItemTemplate>
                <div class="pagebreaker">
                </div>
                <table cellspacing="0" cellpadding="4">
                    <caption>
                        DM MAP
                        <%# Eval("Id")%>
                        (<%# Eval("Name")%>)</caption>
                    <tbody>
                        <tr>
                            <td class="rowlabel">
                                DM MAP #:
                            </td>
                            <td>
                                <%# Eval("Id")%>
                            </td>
                        </tr>
                        <tr>
                            <td class="rowlabel">
                                DISTRIBUTION MAP NAME:
                            </td>
                            <td>
                                <%# Eval("Name")%>
                            </td>
                        </tr>
                        <%--<tr>
                            <td class="rowlabel">
                                TARGETING METHOD:
                            </td>
                            <td>
                                <%# TargetingMethod %>
                            </td>
                        </tr>--%>
                        <tr>
                            <td class="rowlabel">
                                TOTAL:
                            </td>
                            <td>
                                <%# Eval("Total")%>
                            </td>
                        </tr>
                        <tr>
                            <td class="rowlabel">
                                COUNT:
                            </td>
                            <td>
                                <%# Eval("Count")%>
                            </td>
                        </tr>
                        <tr>
                            <td class="rowlabel">
                                PENETRATION:
                            </td>
                            <td>
                                <%# Eval("Pen")%>
                            </td>
                        </tr>
                    </tbody>
                </table>
                <div class="spaceline">
                </div>
                <table class="maptable" cellspacing="0" cellpadding="0">
                    <caption>
                       <%#  campaignName + " - " + Eval("Name")%>
                    </caption>
                    <tbody>
                        <tr>
                            <td colspan="2" style="padding: 0px 1px 0px 1px;">
                                <img style="width: 100%; margin-bottom: -2px;" src='<%# Eval("MapImgUrl") %>' />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 90%; border-style: none; vertical-align: top;">
                                <%--<asp:Repeater ID="rptColors" runat="server">
                                    <HeaderTemplate>
                                        COLOR LEGEND<br />
                                        <table cellpadding="0" cellspacing="0" border="0" style="border-style: none; margin-top: 10px;">
                                            <tr>
                                    </HeaderTemplate>
                                    <ItemTemplate>
                                        <td style="border-style: none;">
                                            <div style='height: 12px; width: 25px; background-color: <%# Eval("ColorString")%>;'>
                                        </td>
                                        <td style="border-style: none;">
                                            </div>
                                            <%# Eval("Name")%>
                                            (<%# Convert.ToInt32((Convert.ToDouble(Eval("Min")) * 100)) %>% -
                                            <%# Convert.ToInt32((Convert.ToDouble(Eval("Max")) * 100)) %>%)
                                        </td>
                                        <td style="width: 10px; border-style: none;">
                                        </td>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        </tr></table></FooterTemplate>
                                </asp:Repeater>--%>
                            </td>
                            <td style="width: 10%; text-align: right; border-style: none;">
                                <asp:Image ID="imgDirectionLegend" ImageUrl="~/Images/direction-legend.png" Height="50px" runat="server" />
                            </td>
                        </tr>
                    </tbody>
                </table>
                <%--<div class='spaceline'></div>--%>
                <div>
                    <%# Eval("Nd")%>   
                </div>
                <%--<div class="pagebreaker">
                </div>--%>
               <%-- <asp:Repeater ID="rptFiveZips" runat="server">
                    <HeaderTemplate>
                        <table cellspacing="0" cellpadding="4">
                            <caption>
                                ZIP CODES CONTAINED IN
                                <asp:Literal ID="ltlName" runat="server"></asp:Literal></caption>
                            <tbody>
                                <tr style="background-color: #eeeeee;">
                                    <td class="collabel" style="width: 10%;">
                                        #
                                    </td>
                                    <td class="collabel" style="width: 30%;">
                                        ZIP CODE
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        TOTAL H/H
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        TARGET H/H
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        PENETRATION
                                    </td>
                                </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("OrderId")%>
                            </td>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Total")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Count")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Pen")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr style="background-color: #eeeeee;">
                            <td>
                                <%# Eval("OrderId")%>
                            </td>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Total")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Count")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Pen")%>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </tbody> </table><div class="spaceline">
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Repeater ID="rptCRoutes" runat="server">
                    <HeaderTemplate>
                        <table cellspacing="0" cellpadding="4">
                            <caption>
                                CROUTES CONTAINED IN
                                <asp:Literal ID="ltlName" runat="server"></asp:Literal></caption>
                            <tbody>
                                <tr style="background-color: #eeeeee;">
                                    <td class="collabel" style="width: 10%">
                                        #
                                    </td>
                                    <td class="collabel" style="width: 30%">
                                        CROUTE #
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        TOTAL H/H
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        TARGET H/H
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        PENETRATION
                                    </td>
                                </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("OrderId")%>
                            </td>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Total")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Count")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Pen")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr style="background-color: #eeeeee;">
                            <td>
                                <%# Eval("OrderId")%>
                            </td>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Total")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Count")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Pen")%>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </tbody> </table><div class="spaceline">
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Repeater ID="rptTracts" runat="server">
                    <HeaderTemplate>
                        <table cellspacing="0" cellpadding="4">
                            <caption>
                                TRACTS CONTAINED IN
                                <asp:Literal ID="ltlName" runat="server"></asp:Literal></caption>
                            <tbody>
                                <tr style="background-color: #eeeeee;">
                                    <td class="collabel" style="width: 10%">
                                        #
                                    </td>
                                    <td class="collabel" style="width: 30%">
                                        TRACT #
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        TOTAL H/H
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        TARGET H/H
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        PENETRATION
                                    </td>
                                </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("OrderId")%>
                            </td>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Total")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Count")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Pen")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr style="background-color: #eeeeee;">
                            <td>
                                <%# Eval("OrderId")%>
                            </td>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Total")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Count")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Pen")%>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </tbody> </table><div class="spaceline">
                        </div>
                    </FooterTemplate>
                </asp:Repeater>
                <asp:Repeater ID="rptBlockGroups" runat="server">
                    <HeaderTemplate>
                        <table cellspacing="0" cellpadding="4">
                            <caption>
                                BG'S CONTAINED IN
                                <asp:Literal ID="ltlName" runat="server"></asp:Literal></caption>
                            <tbody>
                                <tr style="background-color: #eeeeee;">
                                    <td class="collabel" style="width: 10%;">
                                        #
                                    </td>
                                    <td class="collabel" style="width: 30%;">
                                        BLOCK GROUP #
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        TOTAL H/H
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        TARGET H/H
                                    </td>
                                    <td class="collabel" style="width: 20%; text-align: right;">
                                        PENETRATION
                                    </td>
                                </tr>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("OrderId")%>
                            </td>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Total")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Count")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Pen")%>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <tr style="background-color: #eeeeee;">
                            <td>
                                <%# Eval("OrderId")%>
                            </td>
                            <td>
                                <%# Eval("Code") %>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Total")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Count")%>
                            </td>
                            <td style="text-align: right;">
                                <%# Eval("Pen")%>
                            </td>
                        </tr>
                    </AlternatingItemTemplate>
                    <FooterTemplate>
                        </tbody> </table></FooterTemplate>
                </asp:Repeater>--%>
            </ItemTemplate>
        </asp:Repeater>
    </div>
    </form>
</body>
</html>
