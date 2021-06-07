<%@ Page Language="C#" MasterPageFile="~/DM.Master" AutoEventWireup="true" CodeBehind="DistributionMap.aspx.cs" Inherits="GPS.Website.DistributionMap" %>

<%@ Register src="WebControls/Distribution/DistributionMap.ascx" tagname="DistributionMap" tagprefix="uc2" %>
<asp:Content ID="Content1" ContentPlaceHolderID="content" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="DistributionMapServices/DMReaderService.svc" />
            <asp:ServiceReference Path="DistributionMapServices/DMWriterService.svc" />
            <asp:ServiceReference Path="DistributionMapServices/DJReaderService.svc" />
            <asp:ServiceReference Path="DistributionMapServices/DJWriterService.svc" />
            <asp:ServiceReference Path="UserServices/UserReaderService.svc" />
            <asp:ServiceReference Path="TrackServices/GtuReaderService.svc" />
            <asp:ServiceReference Path="CampaignServices/CampaignReaderService.svc" />
            <asp:ServiceReference Path="CampaignServices/CampaignWriterService.svc" />
            <asp:ServiceReference Path="AreaServices/AreaReaderService.svc" />
            <asp:ServiceReference Path="AreaServices/AreaWriterService.svc" />
        </Services>
    </asp:ScriptManagerProxy>

    <uc2:DistributionMap ID="DistributionMap1" runat="server" />
</asp:Content>

