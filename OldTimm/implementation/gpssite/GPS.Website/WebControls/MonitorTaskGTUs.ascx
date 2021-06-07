<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MonitorTaskGTUs.ascx.cs" Inherits="GPS.Website.WebControls.MonitorTaskGTUs" %>

<table width="100%">
    <tr>
        <td style="width:16px">
            <img src="images/icons/bullet_arrow_down.png" alt="" border="0" onclick="toggleDiv('divTaskGtus', this, 'bullet_arrow_down.png', 'resultset_next.png')" />
        </td>
        <td>
            <asp:Label ID="lblDMapName" runat="server" Text="DMap Name1" Font-Bold="true"></asp:Label>
            <asp:Label ID="lblTaskStatus" runat="server"></asp:Label>
            <input type="hidden" id="txtTaskID" runat="server" />
        </td>
    </tr>
</table>

<div id="divTaskGtus" style="background:#F0FFF0; width:100%">
<table width="100%">
    <tr>
        <td>
            <asp:ImageButton ID="imgPlay" runat="server" ImageUrl="~/Images/icons/control_play_blue.png" OnClick="imgPlay_Click" />
            <asp:ImageButton ID="imgPause" runat="server" ImageUrl="~/Images/icons/control_pause_blue.png" OnClick="imgPause_Click" />
            <asp:ImageButton ID="imgStop" runat="server" ImageUrl="~/Images/icons/control_stop_blue.png" OnClick="imgStop_Click" 
                OnClientClick="return confirm('Are you sure you want to permanently stop this distribution?')" />
        </td>
    </tr>
    <tr>
        <td>
            <div style="float:left">
                <a href="#" onclick="showGtu(0)" style="text-decoration:underline; color:#303090">Show GTU</a>
            </div>
            <div style="float:right">
                <a href="#" onclick="showGtu(1)" style="text-decoration:underline; color:#303090">Show GTU History</a>
            </div>
        </td>
    </tr>
    <tr>
        <td>
            <asp:GridView runat="server" ID="GtuStatusGrid" ShowHeader="false" AutoGenerateColumns="false" 
                GridLines="None" Width="100%"
                OnRowDataBound="GtuStatusGrid_RowDataBound">
                <Columns>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <input type="checkbox" gtuid='<%# Eval("GtuID") %>' />
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgUserRole" Width="16px" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <!-- ~/Images/icons/car_add.png -->
                            <asp:Image runat="server" ID="imgGtuStatus" Width="16px" />
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("UniqueID") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField HeaderText="Employee">
                        <ItemTemplate>
                            <%if (!ViewOnly)
                              { %>
                            <a href="#" onclick="showGtuEvents(this)" userid='<%# Eval("UserID") %>' style="text-decoration:underline"><%# Eval("FullName")%></a>
                            <%} %>
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </td>
    </tr>
</table>
</div>