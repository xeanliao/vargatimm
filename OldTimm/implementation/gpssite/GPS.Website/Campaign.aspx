<%@ Page Title="" Language="C#" MasterPageFile="~/Main.Master" AutoEventWireup="true" CodeBehind="Campaign.aspx.cs" Inherits="GPS.Website.CampaignPage" %>
<%@ Register src="WebControls/Campaign.ascx" tagname="Campaign" tagprefix="uc2" %>

<asp:Content ID="Content1" ContentPlaceHolderID="content" runat="server">
    <script type="text/javascript">
        var searchedAreas = new Array();
    </script>

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Services>
            <asp:ServiceReference Path="CampaignServices/CampaignReaderService.svc" />
            <asp:ServiceReference Path="CampaignServices/CampaignWriterService.svc" />
            <asp:ServiceReference Path="AreaServices/AreaReaderService.svc" />
            <asp:ServiceReference Path="AreaServices/AreaWriterService.svc" />
        </Services>
    </asp:ScriptManagerProxy>

    <uc2:Campaign ID="Campaign2" runat="server" />
</asp:Content>
