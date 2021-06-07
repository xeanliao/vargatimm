<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="GPS.Website._Default" %>

<%@ Register Assembly="GPS.Map" Namespace="GPS.Map" TagPrefix="ControlMap" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>GPS</title>
    <link type="text/css" rel="Stylesheet" href="Style/default.css" />
</head>
<body>
    <form id="frmGPS" runat="server">
    <asp:ScriptManager ID="scrtiptManagerMap" runat="server">
    </asp:ScriptManager>
    <div>
        <div class="pTitle">
            <h2>GPS</h2></div>
        <table>
            <tr>
                <td>
                    <div class="topClassifications">
                        <h3>T.I.M.M.</h3></div>
                </td>
                <td>
                    <div class="topClassifications">
                        Upper Classifications:
                        <asp:RadioButtonList ID="rbUpperClassifications" RepeatColumns="5" runat="server"
                            OnSelectedIndexChanged="rbUpperClassifications_SelectedIndexChanged">
                            <asp:ListItem>MSA</asp:ListItem>
                            <asp:ListItem>Urban</asp:ListItem>
                            <asp:ListItem>County</asp:ListItem>
                            <asp:ListItem>SLD (Senate)</asp:ListItem>
                            <asp:ListItem>SLD (House)</asp:ListItem>
                            <asp:ListItem>Voting District</asp:ListItem>
                            <asp:ListItem>SD (Elem.)</asp:ListItem>
                            <asp:ListItem>SD (Secondary)</asp:ListItem>
                            <asp:ListItem>SD (Unified)</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </td>
                <td>
                    <div class="topClassifications">
                        Lower Classifications:
                        <asp:RadioButtonList ID="rbLowerClassifications" RepeatColumns="3" runat="server"
                            AutoPostBack="True" OnSelectedIndexChanged="rbLowerClassifications_SelectedIndexChanged">
                            <asp:ListItem>3 Zip</asp:ListItem>
                            <asp:ListItem>5 Zip</asp:ListItem>
                            <asp:ListItem>B.G.’s</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="Map">
                        <ControlMap:GPSMap runat="server" ID="veGPSMap" Width="800" Height="483" Zoom="4"
                            Latitude="43.67" Longitude="-102.37" />
                    </div>
                </td>
                <td>
                    <div class="Map">
                        SUB MAPS
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="MapNote">
                        <p>
                            Targeting:<asp:TextBox ID="txtTargeting" runat="server" Width="520px"></asp:TextBox></p>
                        <p>
                            Notes:&nbsp;&nbsp;&nbsp;&nbsp;
                            <asp:TextBox ID="txtNotes" runat="server" Width="519px"></asp:TextBox></p>
                    </div>
                </td>
                <td>
                    <div class="MapNote">
                        <p>
                            Master Campaign # :
                            <asp:Label ID="lbMasterCampaignNumber" runat="server" Text=""></asp:Label></p>
                        <p>
                            Master Counts:<asp:Label ID="lbMasterCounts" runat="server" Text=""></asp:Label></p>
                        <p>
                            Total:<asp:Label ID="lbTotal" runat="server" Text=""></asp:Label>
                            Pen.:<asp:Label ID="lbPen" runat="server" Text=""></asp:Label></p>
                    </div>
                </td>
            </tr>
        </table>
    </div>
    </form>
</body>
</html>
