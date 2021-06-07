

function ShowImportData() {
    GPSUploadFile("importdatafile", function(ret) {
        var service = new TIMM.Website.AreaServices.AreaWriterService();
        service.ImportData(campaign.Id, ret.Name, function(records) {
            mapPanel.PushRecords(records);
            GPS.Loading.hide();
        }, function(e) {
            if (e._exceptionType == "GPS.Website.CampaignServices.MyException") {
                alert(e._message);
                GPS.Loading.hide();           
            }
            else {
                alert("There is an error with the data you are trying to import.");
                GPS.Loading.hide();
            }
        }
        );
        return true;
    });
}

function ShowExportDialog() {
    ExportClassification();
    
//    var addresses = addressPanel.GetAddresses();
//    if (addresses.length == 0) {
//        ExportClassification();
//    } else {
//        ExportSelectAreas(addresses);
//    }
}

function Export_File(data, exportFileFormat) {
    if (data) {
        var params = [];
        var objClass = new Object();
        objClass.name = "data";
        objClass.value = data;
        params.push(objClass);
        params.push({ name: 'exportFileFormat', value: exportFileFormat });
        params.push({ name: 'campaignIdForExport', value: campaign.Id });

        var exportForm = new GPS.Form("post", "Export.aspx", params);
        exportForm.submit_from();
    }
}

function On_Export(classification) {

    var result = "";
    
    //build exprot string
    var manualSelectAreas = GetCampaignAreaRecord().GetExportString();
    var params = [];
    if (manualSelectAreas) {
        var chkList = $("div#export-options INPUT[type=checkbox]:checked");
        for (var i = 0; i < chkList.length; i++) {
            var singleRecords = [];
            var arrParam = chkList[i].id.split('_');
            if (arrParam.length > 2) {
                continue;
            }
            var parm = chkList[i].id.match(/\d/g).join("");

            singleRecords.push(parm);
            singleRecords.push(manualSelectAreas);
            if (parm == "2")
                singleRecords.push("Tract");
            else if (parm == "3")
                singleRecords.push("BlockGroup");
            else if (parm == "1")
                singleRecords.push("FiveZip");
            else if (parm == "15")
                singleRecords.push("PremiumCRoute");
            params.push(singleRecords.join('|'));
        }
        result = params.join('#');
    }

    //prepare address
    var exportAddresses = [];
    
    var addresses = addressPanel.GetAddresses();
    if (addresses && addresses.length > 0) {
        var chkList = $("div#export-options INPUT[type=checkbox]:checked");
        for (var i = 0; i < chkList.length; i++) {
            params = [];
            var singleRecords = [];
            var arrParam = chkList[i].id.split('_');
            if (arrParam.length > 2) {
                continue;
            }
            var parm = chkList[i].id.match(/\d/g).join("");

            for (var j = 0; j < addresses.length; j++) {
                var address = addresses[j];
                var len = 0;
                if(address && address.Radiuses && address.Radiuses[2]) {
                    len = address.Radiuses[2];
                }
                params.push(parm + '|' + addresses[j].Id + "|" + GetFileName(parm, addresses[j], len));
            }
            exportAddresses.push(params.join('#'));
        }
        if (exportAddresses.length > 0) {
            result += '$' + exportAddresses.join('#');
        }
    }

    if (result == '') {
        GPSAlert("No area or address is selected.");
        return;
    }

    Export_File(result, $('#export-file-format').val());
    if (result) {
        $(exportDataDialog).dialog("close");
    }
}

function On_Export_Old(classification) {
    var manualSelectAreas = GetCampaignAreaRecord().GetExportString();
    var params = new Array();
    var result = "";
    var exportAddresses = [];
    var addresses = addressPanel.GetAddresses();
    $.each(addresses, function(n, address) {
        var chkList = $("tr#" + address.Id + " INPUT[type=checkbox]:checked");
        if (chkList.length == 0) {
            GPSAlert("Please check a radius!");
            return false;
        }
        for (var i = 0; i < chkList.length; i++) {
            var arrParams = chkList[i].id.split('_');
            if (arrParams.length == 3) {
                if (arrParams[0] == "level") {
                    exportAddresses.push({ id: address.Id, level: arrParams[2], classifiction: 1, isExport: false });
                    exportAddresses.push({ id: address.Id, level: arrParams[2], classifiction: 2, isExport: false });
                    exportAddresses.push({ id: address.Id, level: arrParams[2], classifiction: 3, isExport: false });
                    exportAddresses.push({ id: address.Id, level: arrParams[2], classifiction: 15, isExport: false });
                }
            }
        }
    });
    $.each(addresses, function(n, address) {
        var chkList = $("tr#" + address.Id + " INPUT[type=checkbox]:checked");
        if (chkList.length == 0) {
            GPSAlert("Please check a classification!");
            return false;
        }
        for (var i = 0; i < chkList.length; i++) {
            var arrParams = chkList[i].id.split('_');
            if (arrParams.length == 3) {
                if (arrParams[0] == "classifiction") {
                    for (var j = 0; j < exportAddresses.length; j++) {
                        if (exportAddresses[j].id == address.Id && exportAddresses[j].classifiction == arrParams[2]) {
                            exportAddresses[j].isExport = true;
                        }
                    }
                }
            }
        }
    });
//    for (var k = 0; k < addresses.length; k++) {
//        var address = addresses[k];
//        var chkList = $("tr#" + address.Id + " INPUT[type=checkbox]:checked");
//        if (chkList.length == 0) {
//            GPSAlert("Please check a radius!");
//        }
//        for (var i = 0; i < chkList.length; i++) {
//            var arrParams = chkList[i].id.split('_');
//            if (arrParams.length == 3) {
//                if (arrParams[0] == "level") {
//                    exportAddresses.push({ id: address.Id, level: arrParams[2], classifiction: 1, isExport: false });
//                    exportAddresses.push({ id: address.Id, level: arrParams[2], classifiction: 2, isExport: false });
//                    exportAddresses.push({ id: address.Id, level: arrParams[2], classifiction: 3, isExport: false });
//                    exportAddresses.push({ id: address.Id, level: arrParams[2], classifiction: 15, isExport: false });
//                }
//            }
//        }
//        for (var i = 0; i < chkList.length; i++) {
//            var arrParams = chkList[i].id.split('_');
//            if (arrParams.length == 3) {
//                if (arrParams[0] == "classifiction") {
//                    for (var j = 0; j < exportAddresses.length; j++) {
//                        if (exportAddresses[j].id == address.Id && exportAddresses[j].classifiction == arrParams[2]) {
//                            exportAddresses[j].isExport = true;
//                        }
//                    }
//                }
//            }
//        }
//    }

    if (exportAddresses.length) {
        for (var i = 0; i < exportAddresses.length; i++) {
            var exportAddress = exportAddresses[i];
            if (exportAddress && exportAddress.isExport) {
                var tempRecords = manualSelectAreas;
                classification = exportAddress.classifiction;
                var addressId = exportAddress.id;
                var circelLevel = exportAddress.level;
                var address = addressPanel.GetAddress(addressId);
                address.Radiuses.sort(function(a, b) { return a.Length - b.Length; });
                var radius = 0;
                var circleRecords = "";
                var fileName = "";
                var singleRecords = new Array();
                switch (circelLevel) {
                    case "largest":
                        circleRecords = addressPanel.GetExportString(address.Id, address.Radiuses[2].Id);
                        radius = address.Radiuses[2].Length;
                        break;
                    case "middle":
                        circleRecords = addressPanel.GetExportString(address.Id, address.Radiuses[1].Id);
                        radius = address.Radiuses[1].Length;
                        break;
                    case "smallest":
                        circleRecords = addressPanel.GetExportString(address.Id, address.Radiuses[0].Id);
                        radius = address.Radiuses[0].Length;
                        break;
                }
                //                tempRecords = IntegrationSelectAreas(circleRecords, tempRecords);

                if (tempRecords && circleRecords) {
                    tempRecords = [circleRecords, tempRecords].join(';');
                }
                else if (circleRecords) {
                    tempRecords = circleRecords;
                }

                fileName = GetFileName(classification, address, radius);

                singleRecords.push(classification);
                singleRecords.push(tempRecords);
                singleRecords.push(fileName);
                var singleStr = singleRecords.join('|');
                params.push(singleStr);
            }
        }
        result = params.join('#');
    }
    else {
        if (manualSelectAreas) {
            var chkList = $("div#export-options INPUT[type=checkbox]:checked");
            for (var i = 0; i < chkList.length; i++) {
                var singleRecords = [];
                var arrParam = chkList[i].id.split('_');
                if (arrParam.length > 2) {
                    continue;
                }
                var parm = chkList[i].id.match(/\d/g).join("");
                
                singleRecords.push(parm);
                singleRecords.push(manualSelectAreas);
                if (parm == "2")
                    singleRecords.push("Tract");
                else if (parm == "3")
                    singleRecords.push("BlockGroup");
                else if (parm == "1")
                    singleRecords.push("FiveZip");
                else if (parm == "15")
                    singleRecords.push("PremiumCRoute");
                params.push(singleRecords.join('|'));
            }
            result = params.join('#');
        } else {
            GPSAlert("No area is selected.");
            return false;
        }
    }
    Export_File(result, $('#export-file-format').val());
    if (result) {
        $(exportDataDialog).dialog("close");
    }
}

function GetFileName(classification, address, radius) {
    var fileName = [];
    var addressLength = 12;
    if (classification == 2) fileName.push("trk");
    else if (classification == 3) fileName.push("bg");
    else if (classification == 1) fileName.push("z5");
    else if (classification == 15) fileName.push("cr");
    
    var measureId = 1;
    if(address.Radiuses[0]) {
        measureId = address.Radiuses[0].LengthMeasuresId;
    }
    if (measureId == 1) {
        fileName.push(radius + "Mile");
    } else {
        fileName.push(radius + "KM");
        addressLength = 14;
    }
    fileName.push(address.Street.substr(0, addressLength) + "_" + address.ZipCode);

    return fileName.join('_');
}

function On_Cancel_Export() {
    $(exportDataDialog).dialog("close");
}
var exportDataDialog;


function CreateTH() { return $('<th></th>'); }
function CreateTR() { return $('<tr></tr>'); }
function CreateTD() { return $('<td></td>'); }
function ExportSelectAreas(addresses) {
    var arrClass = [[1, '5 Zip'],
                    [2, 'Tract'],
                    [3, 'BG'],
                    [15, 'CRoute']];
    var arrLevel = [["all", "_largest", "Outer"],
                    ["all", "_middle", "Middle"],
                    ["all", "_smallest", "Inner"]];
    var divExportClass = $("#export_classifications");
    var divExportOptions = $("#export-options");

    ExportClassification();

    var tableAddress = $('<table></table>');

    var thead = $('<thead></thead>');
    var trHead = CreateTR();
    var thAddress = CreateTH().html('Addresses'); // address title
    $(trHead).append($(thAddress));

    $.each(arrLevel, function(n, value) {
        var th = CreateTH();
        var chk = CreateCheckBox(value[1], "");
        $(chk).click(function() { SelectHeader(value[1]); });
        var lb = CreateLabel(value[1], value[2]);
        $(th).append(chk);
        $(th).append(lb);
        $(trHead).append($(th));
    });

//    $.each(arrClass, function(n, value) {
//        var th = CreateTH();
//        var chk = CreateCheckBox("_" + value[0], "");
//        $(chk).click(function() { SelectHeader("_" + value[0]); });
//        var lb = CreateLabel("_" + value[0], value[1]);
//        $(th).append(chk);
//        $(th).append(lb);
//        $(trHead).append($(th));
//    });
    $(thead).append(trHead);
    $(tableAddress).append($(thead));

    var tbody = $('<tbody></tbody>');
    $.each(addresses, function(n, address) {
        var tr = CreateTR();
        $(tr).attr("id", address.Id);
        var tdAddress = CreateTD();
        $(tdAddress).html(address.Street + ", " + address.ZipCode);
        $(tr).append(tdAddress);

        $.each(arrLevel, function(n, value) {
            var tdLevel = CreateTD();
            var chkLevel = CreateCheckBox("level_" + address.Id, value[1]);
            $(chkLevel).click(function() { SelectBody("level_" + address.Id + value[1], value[1]); });
            $(tdLevel).append($(chkLevel));
            $(tr).append(tdLevel);
        });

//        $.each(arrClass, function(n, value) {
//            var tdClass = CreateTD();
//            var chkClass = CreateCheckBox("classifiction_" + address.Id, "_" + value[0]);
//            $(chkClass).click(function() { SelectBody("classifiction_" + address.Id + "_" + value[0], "_" + value[0]); });
//            $(tdClass).append($(chkClass));
//            $(tr).append(tdClass);
//        });

        $(tbody).append($(tr));
    });

    $(tableAddress).append($(tbody));
    
    $("#export-address").append($(tableAddress)).attr("style", "height:240px;");
    $("#export-address-lable").show();
    
    $("#export-options li").css("float", "left");
    $("#export_button_panle").show();
    ShowExportData(divExportClass);
}

function SelectHeader(id) {
    if ($("#" + id)[0].checked)
        $("input[name=" + id + "]").attr("checked", true);
    else
        $("input[name=" + id + "]").attr("checked", false);
}
function SelectBody(objId, headerId) {
    if ($("#" + headerId)[0].checked) {
        if (!$("#" + objId)[0].checked) {
            $("#" + headerId)[0].checked = false;
        }
    }
}
function SelectAddress(id) {
    if ($("#" + id)[0].checked)
        $("input[name=" + id + "]").attr("checked", true);
    else
        $("input[name=" + id + "]").attr("checked", false);
}
function SelectRadius(radiusId, addressId) {
    if ($("#" + addressId)[0].checked) {
        if ($("#" + radiusId)[0].checked == false) {
            $("#" + addressId)[0].checked = false;
        }
    }
}


function ExportClassification() {
    var arrClass = [[1, '5 Zips'],
                    [2, 'Tracts'],
                    [3, 'BGs'],
                    [15, 'CRoute']];
    var divExportClass = $("#export_classifications");
    var divExportOptions = $("#export-options");
    divExportOptions.find("ul").remove();
    divExportOptions.find("table").remove();
    $("export-options").find("table").remove();
    $("export-options-lable").hide();

    var ulClassification = $('<ul></ul>');

    $.each(arrClass, function(n, value) {
        if (EnableExport(value[0])) {
            // Create container for checkbox and label which specifies this export option
            var liClass = $('<li></li>');

            // Create checkbox and display it
            var chk = CreateCheckBox(value[0], "_class");
            $(liClass).append($(chk));

            // Create checkbox label and display it
            var lb = CreateLabel(value[0] + "_class", value[1]);
            $(liClass).append($(lb));

            $(ulClassification).append($(liClass));
        }
    });

    $(divExportOptions).append($(ulClassification));
    ShowExportData(divExportClass);
}

function EnableExport(classification) {
    return true;
    //    return map.EnableExport(classification);
}

function CreateLabel(forId, text) {
    var label = document.createElement("label");
    label.setAttribute("for", forId);
    var labelText = document.createTextNode(text);
    label.appendChild(labelText);
    return label;
}

function CreateCheckBox(id, level) {
    var chk = document.createElement("input"); // create input node
    chk.type = "checkbox"; // set type
    chk.id = id + level;
    chk.name = level; // set name if necessary
    return chk
}

function ShowExportData(objDiv) {
    if (!exportDataDialog) {
        exportDataDialog = $(objDiv).dialog({
            width: 650, modal: true, overlay: { opacity: 0.5 }
        });
    }
    else { $(exportDataDialog).dialog("open"); }
}




var tempPenetrationColors = null;

function ShowPenetrationColorDialog() {
    if ($("#dialog-penetration-color").length == 0) {
        var dialogString = '<div id="dialog-penetration-color"><table style="width: 320px;"><tbody><tr><td style="width: 20px;"><input id="cbxColorEnabled1" type="checkbox" checked="checked" onclick="javascript:OnPenetrationColorEnabledChange(this);" /></td><td style="background-color: Blue; width: 50px;"></td><td style="width: 50px;">Blue</td><td style="width: 150px;"><input id="txtColorMin1" type="text" onchange="javascript:OnPenetrationColorMin(this);" style="width: 30px;"value="0" />% -<input id="txtColorMax1" type="text" onchange="javascript:OnPenetrationColorMax(this);" style="width: 30px;"value="20" />%</td></tr><tr><td><input id="cbxColorEnabled2" type="checkbox" checked="checked" onclick="javascript:OnPenetrationColorEnabledChange(this);" /></td><td style="background-color: Green; width: 50px;"></td><td>Green</td><td><input id="txtColorMin2" type="text" onchange="javascript:OnPenetrationColorMin(this);" style="width: 30px;"value="20" />% -<input id="txtColorMax2" type="text" onchange="javascript:OnPenetrationColorMax(this);" style="width: 30px;"value="40" />%</td></tr><tr><td><input id="cbxColorEnabled3" type="checkbox" checked="checked" onclick="javascript:OnPenetrationColorEnabledChange(this);" /></td><td style="background-color: Yellow; width: 50px;"></td><td>Yellow</td><td><input id="txtColorMin3" type="text" onchange="javascript:OnPenetrationColorMin(this);" style="width: 30px;"value="40" />% -<input id="txtColorMax3" type="text" onchange="javascript:OnPenetrationColorMax(this);" style="width: 30px;"value="60" />%</td></tr><tr><td><input id="cbxColorEnabled4" type="checkbox" checked="checked" onclick="javascript:OnPenetrationColorEnabledChange(this);" /></td><td style="background-color: #f75600; width: 50px;"></td><td>Orange</td><td><input id="txtColorMin4" type="text" onchange="javascript:OnPenetrationColorMin(this);" style="width: 30px;"value="60" />% -<input id="txtColorMax4" type="text" onchange="javascript:OnPenetrationColorMax(this);" style="width: 30px;"value="80" />%</td></tr><tr><td><input id="cbxColorEnabled5" type="checkbox" checked="checked" onclick="javascript:OnPenetrationColorEnabledChange(this);" /></td><td style="background-color: #bb0000; width: 50px;"></td><td>Red</td><td><input id="txtColorMin5" type="text" onchange="javascript:OnPenetrationColorMin(this);" style="width: 30px;"value="80" />% -<input id="txtColorMax5" type="text" onchange="javascript:OnPenetrationColorMax(this);" style="width: 30px;"value="100" />%</td></tr></tbody></table><div style="color: Red;">The minimum value is included in the range, but the maximum value is excluded except 100%.</div></div>';
        $("body").append($(dialogString));
        $("#dialog-penetration-color").dialog({
            autoOpen: false,
            title: 'Penetration Colors',
            modal: true,
            width: 450,
            height: 250,
            //            resizable: false,
            overlay: {
                opacity: 0.5,
                background: "black"
            },
            buttons: {
                "Cancel": function() {
                    //                    alert(ValidatePenetrationColorGaps(tempPenetrationColors));
                    $(this).dialog("close");
                },
                "Change Colors": function() {
                    if (ValidatePenetrationColorGaps(tempPenetrationColors)) {
                        GPS.PenetrationColorManager.SetColors(tempPenetrationColors);
                        $(this).dialog("close");
                    }
                    else {
                        GPSConfirm("There are gaps in the values you specified. Are you sure to continue?", window.ConfirmPenetrationColorGapsCallBack);
                    }
                }
            }
        });
    }
    tempPenetrationColors = GPS.PenetrationColorManager.CopyColors();
    BindPenetrationColorDialog(tempPenetrationColors);
    $("#dialog-penetration-color").dialog("open");
}

function ConfirmPenetrationColorGapsCallBack(value) {
    if (value == 'yes') {
        GPS.PenetrationColorManager.SetColors(tempPenetrationColors);
        $("#dialog-penetration-color").dialog("close");
    }
    else {
        $("#dialog-penetration-color").dialog("close");
        $("#dialog-penetration-color").dialog("open");
    }
}

function ValidatePenetrationColorGaps(colors) {
    var validate = false;
    var i = 0;
    var clen = colors.length;
    var min = 0;
    while (i < clen) {
        if (min == colors[i].Min && colors[i].Max > colors[i].Min) {
            min = colors[i].Max;
            i = 0;
            if (min == 1) {
                validate = true;
                break;
            }
        }
        i++;
    }
    return validate;
}

function BindPenetrationColorDialog(colors) {
    var i = 0;
    var length = colors.length;
    while (i < length) {
        var id = i + 1;
        if (colors[i].Max > colors[i].Min) {
            $('#cbxColorEnabled' + id).attr('checked', true);
            $('#txtColorMin' + id).val(GetPenetrationNumber(colors[i].Min));
            $('#txtColorMax' + id).val(GetPenetrationNumber(colors[i].Max));
            $('#txtColorMin' + id).attr('disabled', false);
            $('#txtColorMax' + id).attr('disabled', false);
        }
        else {
            colors[i].Min = -1;
            colors[i].Max = -1;
            $('#cbxColorEnabled' + id).attr('checked', false);
            $('#txtColorMin' + id).val('');
            $('#txtColorMax' + id).val('');
            $('#txtColorMin' + id).attr('disabled', true);
            $('#txtColorMax' + id).attr('disabled', true);
        }

        i++;
    }
}

function OnPenetrationColorEnabledChange(obj) {
    var index = Number(obj.id.substring(obj.id.length - 1)) - 1;
    var id = index + 1;
    if (obj.checked) {
        $('#txtColorMin' + id).attr('disabled', false);
        $('#txtColorMax' + id).attr('disabled', false);
    }
    else {
        $('#txtColorMin' + id).val('');
        $('#txtColorMax' + id).val('');
        $('#txtColorMin' + id).attr('disabled', true);
        $('#txtColorMax' + id).attr('disabled', true);
        tempPenetrationColors[index].Min = -1;
        tempPenetrationColors[index].Max = -1;
    }
}

function OnPenetrationColorMin(obj) {
    ValidatePenetrationColorMin(obj);
}

function ValidatePenetrationColorMin(obj) {
    var valid = false;
    var strP = /^\d+(\.\d+)?$/;
    var index = Number(obj.id.substring(obj.id.length - 1)) - 1;
    var omax = Math.round(tempPenetrationColors[index].Max * 100);
    var omin = Math.round(tempPenetrationColors[index].Min * 100);

    if (strP.test(obj.value) && Number(obj.value) >= 0 && Number(obj.value) <= 100) {
        var nmin = Number(obj.value);
        var colors = tempPenetrationColors;
        var i = 0;
        var clen = colors.length;
        var innerValid = true;
        while (i < clen) {
            if (i == index) { i++; continue; }
            if (omax >= 0) {
                if ((Math.round(colors[i].Min * 100) <= nmin && nmin < Math.round(colors[i].Max * 100))
                || (Math.round(colors[i].Min * 100) < omax && nmin < Math.round(colors[i].Max * 100))) {
                    innerValid = false;
                    break;
                }

            }
            else {
                if (Math.round(colors[i].Min * 100) <= nmin && nmin < Math.round(colors[i].Max * 100)) {
                    innerValid = false;
                    break;
                }
            }
            i++;
        }
        if (innerValid) {
            if (omax >= 0 && omin >= 0) {
                if (nmin < omax) { valid = true; }
            }
            else {
                valid = true;
            }
        }
    }

    if (valid) { tempPenetrationColors[index].Min = Number(obj.value) / 100; }
    else if (omin >= 0) { $(obj).val(GetPenetrationNumber(omin)); }
    else { $(obj).val(''); }
    return valid;
}

function OnPenetrationColorMax(obj) {
    ValidatePenetrationColorMax(obj);
}

function ValidatePenetrationColorMax(obj) {
    var valid = false;
    var strP = /^\d+(\.\d+)?$/;
    var index = Number(obj.id.substring(obj.id.length - 1)) - 1;
    var omax = Math.round(tempPenetrationColors[index].Max * 100);
    var omin = Math.round(tempPenetrationColors[index].Min * 100);

    if (strP.test(obj.value) && Number(obj.value) >= 0 && Number(obj.value) <= 100) {
        var nmax = Number(obj.value);
        var colors = tempPenetrationColors;
        var i = 0;
        var clen = colors.length;
        var innerValid = true;
        while (i < clen) {
            if (i == index) { i++; continue; }
            if (omin >= 0) {
                if ((Math.round(colors[i].Min * 100) < nmax && nmax <= Math.round(colors[i].Max * 100))
                    || (Math.round(colors[i].Min * 100) < nmax && omin < Math.round(colors[i].Max * 100))) {
                    innerValid = false;
                    break;
                }
            }
            else {
                if (Math.round(colors[i].Min * 100) < nmax && nmax <= Math.round(colors[i].Max * 100)) {
                    innerValid = false;
                    break;
                }
            }

            i++;
        }
        if (innerValid) {
            if (nmax >= 0 && omin >= 0) {
                if (nmax > omin) { valid = true; }
            }
            else {
                valid = true;
            }
        }
    }

    if (valid) { tempPenetrationColors[index].Max = Number(obj.value) / 100; }
    else if (omax >= 0) { $(obj).val(GetPenetrationNumber(omax)); }
    else { $(obj).val(''); }
    return valid;
}


function GetPenetrationNumber(number) {
    number = Math.round(number * 100);
    return number > 100 ? 100 : (number < 0 ? 0 : number);
}


