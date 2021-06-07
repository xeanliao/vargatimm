<%@ Page Language="C#" AutoEventWireup="true"  CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">


<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:GridView ID="GridView1" runat="server" >
                  

        </asp:GridView>
        <br />
        <br />
        <asp:textbox ID="tb1" runat="server"></asp:textbox>
        <br />
        <br />
        <asp:Button ID="Button1" runat="server" onclick="Button1_Click" Text="Insert" />
&nbsp;&nbsp;&nbsp;
        <asp:Button ID="Button2" runat="server" onclick="Button2_Click" Text="Watch" />
        <asp:Button ID="Button3" runat="server" OnClick="Button3_Click" Text="testALReg" />
        <br/>
        <hr/>
        lat: <asp:TextBox ID="tb2" runat="server"></asp:TextBox>          lon: <asp:TextBox ID="tb3" runat="server"></asp:TextBox>
        <br/>
        <br />
        <asp:Button ID="Button4" runat="server" OnClick="Button4_Click" Text="testInArea" />
        <br/>
        <asp:Label ID="lbl1" runat="server"></asp:Label>
        <br />
        <asp:Label ID="nlbl" runat="server"></asp:Label>
        <hr />
        <asp:TextBox ID="tb4" runat="server"></asp:TextBox>
        <asp:Button ID="Button5" runat="server" OnClick="Button5_Click" Text="testALReg1" />
        <asp:Button ID="Button6" runat="server" OnClick="Button6_Click" Text="testInArea1" />
        <br />
        <asp:Label ID="lbl2" runat="server"></asp:Label>
        <br />
        <asp:Label ID="nlb2" runat="server"></asp:Label>
        <br />
        <asp:Label ID="lbl3" runat="server"></asp:Label>
        <br />
        <asp:Label ID="nlb3" runat="server"></asp:Label>
        <hr />
        <br />
        <asp:Button ID="Button7" runat="server" OnClick="Button7_Click" Text="testAll" BackColor="BurlyWood"/>
        <hr />
        <br />
        <asp:Button ID="Button8" runat="server" OnClick="Button8_Click" Text="testGetGtu" BackColor="LightGoldenrodYellow"/>
        <hr />
        <br />
        <asp:Button ID="Button9" runat="server" OnClick="Button9_Click" Text="testMail" BackColor="Chocolate"/>
        <hr />
        <br />
        <asp:Button ID="Button10" runat="server" OnClick="Button10_Click" Text="testDNDArea" BackColor="Pink"/>
    </div>
    </form>
</body>
</html>
