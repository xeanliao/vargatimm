<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GtuEvents.aspx.cs" Inherits="GPS.Website.GtuEvents" %>

<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Register src="WebControls/ControlAddEmployee.ascx" tagname="ControlAddEmployee" tagprefix="uc1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body style="height:100%">
    <form id="form1" runat="server">
    <asp:ScriptManager runat="server"></asp:ScriptManager>
    <div><asp:Label runat="server" ID="lblError" ForeColor="Red" EnableViewState="false"></asp:Label> </div>
    <div style="display:">
            
        <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Height="540px">
            <!-- Employee -->
            <cc1:TabPanel runat="server" HeaderText="tabEmployee" ID="tabEmployee">
                <ContentTemplate>
                    <asp:Panel runat="server" ID="pnlEditEmployee" Visible="false">
                        <uc1:ControlAddEmployee ID="ctlAddEmployee" runat="server" AllowNameChange="false" />
                        <asp:Button runat="server" ID="btnCancel" Text="Cancel" OnClick="btnCancelEmployeeEdit_Click" />
                        <asp:Button runat="server" ID="btnSave" Text="Save" OnClick="btnSaveEmployee_Click" />
                    </asp:Panel>

                    <asp:Panel runat="server" ID="pnlEmployeeInfo">
                        <div style="padding:10px 0 10px 0">
                            <asp:LinkButton runat="server" ID="btnEdit" OnClick="btnEdit_Click" Text="Edit"></asp:LinkButton>
                        </div>
                        Distributor: <asp:Label runat="server" ID="lblDistributor"></asp:Label><br />
                        <asp:Label runat="server" ID="lblCell">Cell: </asp:Label><br />
                        <asp:Label runat="server" ID="lblBirthdate">Birthday: </asp:Label><br />
                        <div style="padding:10px">
                            <asp:Label runat="server" id="lblNotes">Notes:</asp:Label><br />
                        </div>
                        <asp:Image runat="server" ID="imgEmployee" Width="95%" />
                    </asp:Panel>
                </ContentTemplate>
            </cc1:TabPanel>

            <!-- Gtu Events -->
            <cc1:TabPanel ID="TabPanel2" runat="server" HeaderText="Event History">
                <ContentTemplate>
                    <asp:GridView ID="gridGtuEvents" runat="server" BackColor="White"
                        BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" CellPadding="4" 
                        ForeColor="Black" GridLines="Vertical" Width="100%" Height="100%"
                        AutoGenerateColumns="false" OnRowDataBound="gridGtuEvents_RowDataBound"
                        AllowPaging="true" PageSize="16" Font-Size="Small" OnPageIndexChanging="gridGtuEvents_PageIndexChanging"
                        AllowSorting="true"
                        >
                        <AlternatingRowStyle BackColor="White" />
                        <FooterStyle BackColor="#CCCC99" />
                        <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                        <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                        <RowStyle BackColor="#F7F7DE" />
                        <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                        <SortedAscendingCellStyle BackColor="#FBFBF2" />
                        <SortedAscendingHeaderStyle BackColor="#848384" />
                        <SortedDescendingCellStyle BackColor="#EAEAD3" />
                        <SortedDescendingHeaderStyle BackColor="#575357" />
                        <Columns>
                            <asp:BoundField DataField="InsertedTime" HeaderText="Event Time" DataFormatString="{0:yyyy-MM-dd hh:mm}" />
                            <asp:TemplateField HeaderText="Event Detail">
                                <ItemTemplate>
                                    <asp:Label ID="lblEventDetail" runat="server"></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>    

                </ContentTemplate>
            </cc1:TabPanel>
        </cc1:TabContainer>
    
    </div>
    </form>
</body>
</html>
