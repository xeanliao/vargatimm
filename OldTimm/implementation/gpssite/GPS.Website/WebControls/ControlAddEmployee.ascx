<%@ Register assembly="AjaxControlToolkit" namespace="AjaxControlToolkit" tagprefix="cc1" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ControlAddEmployee.ascx.cs" Inherits="GPS.Website.WebControls.ControlAddEmployee" %>
<asp:Label ID="lblMessage" runat="server" ForeColor="Red" EnableViewState="false"></asp:Label>
                    <table cellpadding="0" cellspacing="4">
                        <tr>
                            <td>
                                <!-- choose distributor company -->
                                <asp:DropDownList runat="server" ID="distributorDropDown" DataTextField="Name" DataValueField="Id">
                                    <asp:ListItem Text="Distributor" Value=""></asp:ListItem>
                                </asp:DropDownList> 
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="distributorDropDown" ErrorMessage="*" ValidationGroup="addEmployee"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:TextBox ID="txtFullName" runat="server"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ValidationGroup="addEmployee" runat="server" ControlToValidate="txtFullName" ErrorMessage="*"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:DropDownList runat="server" ID="employeeRoleDropDown" DataTextField="Name" DataValueField="Id">
                                    <asp:ListItem Text="Choose Role" Value=""></asp:ListItem>
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="employeeRoleDropDown" ErrorMessage="*" ValidationGroup="addEmployee"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td><asp:TextBox id="txtCellPhone" runat="server"></asp:TextBox> </td>
                        </tr>
                        <tr>
                            <td>
                                <b>birthday </b><br />
                                <asp:DropDownList runat="server" ID="birthMonthDropDown">
                                    <asp:ListItem Text="MM" Value=""></asp:ListItem>
                                    <asp:ListItem Text="01" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="02" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="03" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="04" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="05" Value="5"></asp:ListItem>
                                    <asp:ListItem Text="06" Value="6"></asp:ListItem>
                                    <asp:ListItem Text="07" Value="7"></asp:ListItem>
                                    <asp:ListItem Text="08" Value="8"></asp:ListItem>
                                    <asp:ListItem Text="09" Value="9"></asp:ListItem>
                                    <asp:ListItem Text="00" Value="0"></asp:ListItem>
                                    <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                    <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                </asp:DropDownList>
                                <asp:DropDownList runat="server" ID="birthDayDropDown">
                                    <asp:ListItem Text="DD" Value=""></asp:ListItem>
                                    <asp:ListItem Text="01" Value="1"></asp:ListItem>
                                    <asp:ListItem Text="02" Value="2"></asp:ListItem>
                                    <asp:ListItem Text="03" Value="3"></asp:ListItem>
                                    <asp:ListItem Text="04" Value="4"></asp:ListItem>
                                    <asp:ListItem Text="05" Value="5"></asp:ListItem>
                                    <asp:ListItem Text="06" Value="6"></asp:ListItem>
                                    <asp:ListItem Text="07" Value="7"></asp:ListItem>
                                    <asp:ListItem Text="08" Value="8"></asp:ListItem>
                                    <asp:ListItem Text="09" Value="9"></asp:ListItem>
                                    <asp:ListItem Text="10" Value="10"></asp:ListItem>
                                    <asp:ListItem Text="11" Value="11"></asp:ListItem>
                                    <asp:ListItem Text="12" Value="12"></asp:ListItem>
                                    <asp:ListItem Text="13" Value="13"></asp:ListItem>
                                    <asp:ListItem Text="14" Value="14"></asp:ListItem>
                                    <asp:ListItem Text="15" Value="15"></asp:ListItem>
                                    <asp:ListItem Text="16" Value="16"></asp:ListItem>
                                    <asp:ListItem Text="17" Value="17"></asp:ListItem>
                                    <asp:ListItem Text="18" Value="18"></asp:ListItem>
                                    <asp:ListItem Text="19" Value="19"></asp:ListItem>
                                    <asp:ListItem Text="20" Value="20"></asp:ListItem>
                                    <asp:ListItem Text="21" Value="21"></asp:ListItem>
                                    <asp:ListItem Text="22" Value="22"></asp:ListItem>
                                    <asp:ListItem Text="23" Value="23"></asp:ListItem>
                                    <asp:ListItem Text="24" Value="24"></asp:ListItem>
                                    <asp:ListItem Text="25" Value="25"></asp:ListItem>
                                    <asp:ListItem Text="26" Value="26"></asp:ListItem>
                                    <asp:ListItem Text="27" Value="27"></asp:ListItem>
                                    <asp:ListItem Text="28" Value="28"></asp:ListItem>
                                    <asp:ListItem Text="29" Value="29"></asp:ListItem>
                                    <asp:ListItem Text="30" Value="30"></asp:ListItem>
                                    <asp:ListItem Text="31" Value="31"></asp:ListItem>
                               </asp:DropDownList>
                               <asp:TextBox ID="txtBirthYear" runat="server"></asp:TextBox>
                           </td>
                        </tr>
                        <tr>
                            <td>
                                Photo: <asp:FileUpload ID="photoFileUpload" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <b>notes</b><br />
                                <asp:TextBox ID="txtNotes" TextMode="MultiLine" runat="server" Rows="4" Width="400px"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <cc1:TextBoxWatermarkExtender ID="watermarkEx2" runat="server" TargetControlID="txtFullName" WatermarkText="full name" WatermarkCssClass="watermark">
                    </cc1:TextBoxWatermarkExtender>
                    <cc1:TextBoxWatermarkExtender ID="watermarkEx3" runat="server" TargetControlID="txtBirthYear" WatermarkText="Year" WatermarkCssClass="watermark">
                    </cc1:TextBoxWatermarkExtender>
                    <cc1:TextBoxWatermarkExtender ID="watermarkEx4" runat="server" TargetControlID="txtCellPhone" WatermarkText="Cell Phone" WatermarkCssClass="watermark">
                    </cc1:TextBoxWatermarkExtender>
