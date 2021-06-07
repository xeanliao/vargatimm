<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminGtuToEmployee.aspx.cs" Inherits="GPS.Website.AdminGtuToEmployee" %>
<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>

<%@ Register src="WebControls/ControlAddEmployee.ascx" tagname="ControlAddEmployee" tagprefix="uc1" %>
<%@ Register src="WebControls/AdminGtuToEmployeeControl.ascx" tagname="AdminGtuToEmployeeControl" tagprefix="uc2" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <base target="_self" />
    <style type="text/css">
        .watermark
        {
            color:gray;
        }
    </style>
    <script type="text/javascript">
        function colorChanged(sender) 
        {
            sender.get_element().style.color = "#" + sender.get_selectedColor();
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
    <asp:Label ID="lblError" runat="server" EnableViewState="false" ForeColor="Red"></asp:Label>
    <div>
        <asp:Label ID="lblInfo" runat="server" EnableViewState="false" ForeColor="Green"></asp:Label>
    </div>
    <div style="overflow:auto; width:90%">

        <cc1:TabContainer ID="TabContainer1" runat="server" ActiveTabIndex="0" Width="100%">
            <cc1:TabPanel runat="server" HeaderText="Assign GTUs" ID="tabAssignGtus">
                <ContentTemplate>
                    <div style="margin:4px 4px 0 0; font-weight:bold; background-color:#D0FFD0; width:100%; padding:4px">
                        <asp:Label runat="server" ID="lblBreadcrumb" Text="Campaign Name > Dmap Name"></asp:Label>
                    </div>
                    <uc2:AdminGtuToEmployeeControl ID="AdminGtuToEmployeeControl1" runat="server" />
                    <br />
                    <div style="margin:4px 4px 0 0; font-weight:bold; background-color:#D0FFD0; width:100%; padding:4px">Not-Transmitting GTUs </div>
                    <asp:GridView AutoGenerateColumns="False" runat="server" ID="gridNoSignalGtus">
                        <Columns>
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Image ID="Image1" runat="server" ImageUrl="Images/icons/transmit_go.png" />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:TemplateField HeaderText="GTU#">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("ShortUniqueId") %>'></asp:Label>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>

                </ContentTemplate>
            </cc1:TabPanel>
            <cc1:TabPanel ID="tabAddEmployee" runat="server" HeaderText="Add Employee">
                <ContentTemplate>
                    <!-- Add New Empoyee Tab -->
                    <uc1:ControlAddEmployee ID="ctlAddEmployee" runat="server" />
                    <asp:Button ID="btnCancelEmployeeAdd" runat="server" Text="Cancel" OnClick="btnCancelEmployeeAdd_Click" />
                    <asp:Button ID="btnSaveEmployee" runat="server" Text="Save" OnClick="btnSaveEmployee_Click" />
                </ContentTemplate>
            </cc1:TabPanel>
        </cc1:TabContainer>
    </div>
    </form>
</body>
</html>
