<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="export_data.aspx.cs" Inherits="Requirement._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>Export Data</title>
    <style type="text/css">
    p { line-height: 30px; font-family: Verdana, Tahoma; }
    td { border:solid thin #33CCFF;}
    .selected {color:red;}
    caption {font-size: 20px;
             text-align:left;
             font-weight:bold; 
             line-height:40px;
             background:#ffff99;
             padding-left:10px;
             border:solid thin #00ccff;
             }
    </style>
</head>
<body style="margin: 20px; font-size: 20px">
    <p style="padding: 10px; background-color: #00CCFF; font-size: 40px;">
    
        Requirement Description</p>
    <p style="padding: 10px; background-color: #00CCFF;">
    
        Requirement Name: Export Data<br />
        Version: 1.1<br />
        Author: Harry Duan</p><br />
        <table cellpadding="10" cellspacing="0" rules="all" style="border: thin solid #33CCFF; width: 100%; background-color: #FFFFFF;">
            <caption>Revision History</caption>
            <tr>
                <td>
                    Version</td>
                <td>
                    Description</td>
                <td>
                    Date</td>
            </tr>
            <tr>
                <td>
                    1.0</td>
                <td>
                    Created</td>
                <td>
                    13-Oct-09</td>
            </tr>
            <tr>
                <td>
                    1.1</td>
                <td>
                    Updated point 1<br />
                    Added point 3</td>
                <td>
                    15-Oct-09</td>
            </tr>
        </table>
<br />
<p style="padding: 10px">
        1. When the website is initially loaded, a specific classification is shown 
        according to the system&#39;s <b>Default Classification</b> setting, and the corresponding 
        check box in the <b>Lower Classification</b> or <b>Upper Classification </b> is checked.</p>
<p style="padding: 10px">
        2. Selecting a 3ZIP area will make all 5ZIP areas covered by this 3ZIP area 
        selected; selecting a 5ZIP area will make all TRKs covered by this 5ZIP area 
        selected; selecting a TRK area will make all BGs covered by this TRK area 
        selected.</p>
<p style="border: thin solid #33CCFF; padding: 10px; background-color: #FFFF99;">
        The following is an example for point 2 above.</p>
<p style="padding: 10px">
        Suppose we have the following areas.</p>
<table cellpadding="10" cellspacing="0" rules="all" 
    style="border: thin solid #33CCFF; width: 100%; background-color: #FFFFFF;">
    <tr>
        <td>
            3ZIP Areas</td>
        <td>
            5ZIP Areas</td>
        <td>
            TRKs</td>
        <td>
            BGs</td>
    </tr>
    <tr>
        <td rowspan="8">
            999</td>
        <td rowspan="4">
            99901</td>
        <td rowspan="2">
            99901-01</td>
        <td>
            99901-01-01</td>
    </tr>
    <tr>
        <td>
            99901-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            99901-02</td>
        <td>
            99901-02-01</td>
    </tr>
    <tr>
        <td>
            99901-02-02</td>
    </tr>
    <tr>
        <td rowspan="4">
            99902</td>
        <td rowspan="2">
            99902-01</td>
        <td>
            99902-01-01</td>
    </tr>
    <tr>
        <td>
            99902-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            99902-02</td>
        <td>
            99902-02-01</td>
    </tr>
    <tr>
        <td>
            99902-02-02</td>
    </tr>
    <tr>
        <td rowspan="8">
            888</td>
        <td rowspan="4">
            88801</td>
        <td rowspan="2">
            88801-01</td>
        <td>
            88801-01-01</td>
    </tr>
    <tr>
        <td>
            88801-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            88801-02</td>
        <td>
            88801-02-01</td>
    </tr>
    <tr>
        <td>
            88801-02-02</td>
    </tr>
    <tr>
        <td rowspan="4">
            88802</td>
        <td rowspan="2">
            88802-01</td>
        <td>
            88802-01-01</td>
    </tr>
    <tr>
        <td>
            88802-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            88802-02</td>
        <td>
            88802-02-01</td>
    </tr>
    <tr>
        <td>
            88802-02-02</td>
    </tr>
</table>
<p style="padding: 10px">
        When 999 is selected, the areas under 999 will also be selected automatically, as illustrated in 
        the table below, where red areas are selected.</p>
<table cellpadding="10" cellspacing="0" rules="all" 
    style="border: thin solid #33CCFF; width: 100%; background-color: #FFFFFF;">
    <tr>
        <td>
            3ZIP Areas</td>
        <td>
            5ZIP Areas</td>
        <td>
            TRKs</td>
        <td>
            BGs</td>
    </tr>
    <tr>
        <td rowspan="8" class="selected">
            999</td>
        <td rowspan="4" class="selected">
            99901</td>
        <td rowspan="2" class="selected">
            99901-01</td>
        <td class="selected">
            99901-01-01</td>
    </tr>
    <tr>
        <td class="selected">
            99901-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            99901-02</td>
        <td class="selected">
            99901-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            99901-02-02</td>
    </tr>
    <tr>
        <td rowspan="4" class="selected">
            99902</td>
        <td rowspan="2" class="selected">
            99902-01</td>
        <td class="selected">
            99902-01-01</td>
    </tr>
    <tr>
        <td class="selected">
            99902-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            99902-02</td>
        <td class="selected">
            99902-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            99902-02-02</td>
    </tr>
    <tr>
        <td rowspan="8">
            888</td>
        <td rowspan="4">
            88801</td>
        <td rowspan="2">
            88801-01</td>
        <td>
            88801-01-01</td>
    </tr>
    <tr>
        <td>
            88801-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            88801-02</td>
        <td>
            88801-02-01</td>
    </tr>
    <tr>
        <td>
            88801-02-02</td>
    </tr>
    <tr>
        <td rowspan="4">
            88802</td>
        <td rowspan="2">
            88802-01</td>
        <td>
            88802-01-01</td>
    </tr>
    <tr>
        <td>
            88802-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            88802-02</td>
        <td>
            88802-02-01</td>
    </tr>
    <tr>
        <td>
            88802-02-02</td>
    </tr>
</table>
<p style="padding: 10px">
        Now, if the user selects 88802, the areas under 88802 will also be selected 
        automatically. The current selected areas are illustrated in the table below, 
        where red areas are selected. Please note 888 is not selected, although it 
        covers 88802.</p>
<table cellpadding="10" cellspacing="0" rules="all" 
    style="border: thin solid #33CCFF; width: 100%; background-color: #FFFFFF;">
    <tr>
        <td>
            3ZIP Areas</td>
        <td>
            5ZIP Areas</td>
        <td>
            TRKs</td>
        <td>
            BGs</td>
    </tr>
    <tr>
        <td rowspan="8" class="selected">
            999</td>
        <td rowspan="4" class="selected">
            99901</td>
        <td rowspan="2" class="selected">
            99901-01</td>
        <td class="selected">
            99901-01-01</td>
    </tr>
    <tr>
        <td class="selected">
            99901-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            99901-02</td>
        <td class="selected">
            99901-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            99901-02-02</td>
    </tr>
    <tr>
        <td rowspan="4" class="selected">
            99902</td>
        <td rowspan="2" class="selected">
            99902-01</td>
        <td class="selected">
            99902-01-01</td>
    </tr>
    <tr>
        <td class="selected">
            99902-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            99902-02</td>
        <td class="selected">
            99902-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            99902-02-02</td>
    </tr>
    <tr>
        <td rowspan="8">
            888</td>
        <td rowspan="4">
            88801</td>
        <td rowspan="2">
            88801-01</td>
        <td>
            88801-01-01</td>
    </tr>
    <tr>
        <td>
            88801-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            88801-02</td>
        <td>
            88801-02-01</td>
    </tr>
    <tr>
        <td>
            88801-02-02</td>
    </tr>
    <tr>
        <td rowspan="4" class="selected">
            88802</td>
        <td rowspan="2" class="selected">
            88802-01</td>
        <td class="selected">
            88802-01-01</td>
    </tr>
    <tr>
        <td class="selected">
            88802-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            88802-02</td>
        <td class="selected">
            88802-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            88802-02-02</td>
    </tr>
</table>
<p style="padding: 10px">
        Now, if the user selects 888, all areas under 888 will be selected 
        automatically. The current selected areas are illustrated in the table below, 
        where red areas are selected.</p>
<table cellpadding="10" cellspacing="0" rules="all" 
    style="border: thin solid #33CCFF; width: 100%; background-color: #FFFFFF;">
    <tr>
        <td>
            3ZIP Areas</td>
        <td>
            5ZIP Areas</td>
        <td>
            TRKs</td>
        <td>
            BGs</td>
    </tr>
    <tr>
        <td rowspan="8" class="selected">
            999</td>
        <td rowspan="4" class="selected">
            99901</td>
        <td rowspan="2" class="selected">
            99901-01</td>
        <td class="selected">
            99901-01-01</td>
    </tr>
    <tr>
        <td class="selected">
            99901-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            99901-02</td>
        <td class="selected">
            99901-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            99901-02-02</td>
    </tr>
    <tr>
        <td rowspan="4" class="selected">
            99902</td>
        <td rowspan="2" class="selected">
            99902-01</td>
        <td class="selected">
            99902-01-01</td>
    </tr>
    <tr>
        <td class="selected">
            99902-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            99902-02</td>
        <td class="selected">
            99902-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            99902-02-02</td>
    </tr>
    <tr>
        <td rowspan="8" class="selected">
            888</td>
        <td rowspan="4" class="selected">
            88801</td>
        <td rowspan="2" class="selected">
            88801-01</td>
        <td class="selected">
            88801-01-01</td>
    </tr>
    <tr>
        <td class="selected">
            88801-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            88801-02</td>
        <td class="selected">
            88801-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            88801-02-02</td>
    </tr>
    <tr>
        <td rowspan="4" class="selected">
            88802</td>
        <td rowspan="2" class="selected">
            88802-01</td>
        <td class="selected">
            88802-01-01</td>
    </tr>
    <tr>
        <td class="selected">
            88802-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            88802-02</td>
        <td class="selected">
            88802-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            88802-02-02</td>
    </tr>
</table>
<p style="border: thin solid #33CCFF; padding: 10px; background-color: #FFFF99;">
        Below is another example for point 2 above.</p>
<p style="padding: 10px">
        Please see the table below. Suppose the red areas have been selected.</p>
<table cellpadding="10" cellspacing="0" rules="all" 
    style="border: thin solid #33CCFF; width: 100%; background-color: #FFFFFF;">
    <tr>
        <td>
            3ZIP Areas</td>
        <td>
            5ZIP Areas</td>
        <td>
            TRKs</td>
        <td>
            BGs</td>
    </tr>
    <tr>
        <td rowspan="8">
            999</td>
        <td rowspan="4">
            99901</td>
        <td rowspan="2">
            99901-01</td>
        <td>
            99901-01-01</td>
    </tr>
    <tr>
        <td>
            99901-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            99901-02</td>
        <td>
            99901-02-01</td>
    </tr>
    <tr>
        <td>
            99901-02-02</td>
    </tr>
    <tr>
        <td rowspan="4">
            99902</td>
        <td rowspan="2">
            99902-01</td>
        <td>
            99902-01-01</td>
    </tr>
    <tr>
        <td>
            99902-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            99902-02</td>
        <td>
            99902-02-01</td>
    </tr>
    <tr>
        <td>
            99902-02-02</td>
    </tr>
    <tr>
        <td rowspan="8">
            888</td>
        <td rowspan="4">
            88801</td>
        <td rowspan="2" class="selected">
            88801-01</td>
        <td class="selected">
            88801-01-01</td>
    </tr>
    <tr>
        <td>
            88801-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            88801-02</td>
        <td>
            88801-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            88801-02-02</td>
    </tr>
    <tr>
        <td rowspan="4">
            88802</td>
        <td rowspan="2">
            88802-01</td>
        <td>
            88802-01-01</td>
    </tr>
    <tr>
        <td>
            88802-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            88802-02</td>
        <td>
            88802-02-01</td>
    </tr>
    <tr>
        <td>
            88802-02-02</td>
    </tr>
</table>
<p style="padding: 10px">
        Now, if the user selects 88801, all areas under 88801 will be selected 
        automatically, as illustrated in the table below, where red areas are selected.</p>
<table cellpadding="10" cellspacing="0" rules="all" 
    style="border: thin solid #33CCFF; width: 100%; background-color: #FFFFFF;">
    <tr>
        <td>
            3ZIP Areas</td>
        <td>
            5ZIP Areas</td>
        <td>
            TRKs</td>
        <td>
            BGs</td>
    </tr>
    <tr>
        <td rowspan="8">
            999</td>
        <td rowspan="4">
            99901</td>
        <td rowspan="2">
            99901-01</td>
        <td>
            99901-01-01</td>
    </tr>
    <tr>
        <td>
            99901-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            99901-02</td>
        <td>
            99901-02-01</td>
    </tr>
    <tr>
        <td>
            99901-02-02</td>
    </tr>
    <tr>
        <td rowspan="4">
            99902</td>
        <td rowspan="2">
            99902-01</td>
        <td>
            99902-01-01</td>
    </tr>
    <tr>
        <td>
            99902-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            99902-02</td>
        <td>
            99902-02-01</td>
    </tr>
    <tr>
        <td>
            99902-02-02</td>
    </tr>
    <tr>
        <td rowspan="8">
            888</td>
        <td rowspan="4" class="selected">
            88801</td>
        <td rowspan="2" class="selected">
            88801-01</td>
        <td class="selected">
            88801-01-01</td>
    </tr>
    <tr>
        <td class="selected">
            88801-01-02</td>
    </tr>
    <tr>
        <td rowspan="2" class="selected">
            88801-02</td>
        <td class="selected">
            88801-02-01</td>
    </tr>
    <tr>
        <td class="selected">
            88801-02-02</td>
    </tr>
    <tr>
        <td rowspan="4">
            88802</td>
        <td rowspan="2">
            88802-01</td>
        <td>
            88802-01-01</td>
    </tr>
    <tr>
        <td>
            88802-01-02</td>
    </tr>
    <tr>
        <td rowspan="2">
            88802-02</td>
        <td>
            88802-02-01</td>
    </tr>
    <tr>
        <td>
            88802-02-02</td>
    </tr>
</table>
<p style="padding: 10px">
        3. Deselecting a 3ZIP area will make all 5ZIP areas covered by this 3ZIP area 
        deselected; deselecting a 5ZIP area will make all TRKs covered by this 5ZIP area 
        deselected; deselecting a TRK area will make all BGs covered by this TRK area 
        deselected.</p>
</body>
</html>
