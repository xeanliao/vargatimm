

function OnLoadAddresses(colorStr) {
    //    alert("OnLoadFiveZipAreaClick('" + colorStr + "')");
    if (addressPanel) {
        GPSUploadFile("addressfile", function(ret) {
            //            GPS.Loading.show();
            var service = new TIMM.Website.CampaignServices.CampaignWriterService();
            service.UploadAddress(campaign.Id, ret.Name, colorStr, function(addresses) {
                addressPanel.AppendAddresses2(addresses);
                GPS.Loading.hide();
            }, function(e) {
                if (e._exceptionType == "MyException") {
                    alert(e._message);
                }
                else {
                    alert("An error occurs when importing data.");
                }
                GPS.Loading.hide();
            }
            );
            return true;
        });
    }
}

function OnLoadFiveZipAreaClick() {
    //    alert("OnLoadFiveZipAreaClick");
    ShowLoadFiveZipAreaDialog();
}

function OnLoadBlockGroupAreaClick() {
    // alert("OnLoadBlockGroupAreaClick");
    ShowLoadBlockGroupAreaDialog();
}


function OnLoadCrouteAreaClick() {
    //    alert("OnLoadFiveZipAreaClick");
    ShowLoadCrouteAreaDialog();
}


function LoadFiveZipArea(areas) {
    if (mapPanel) {
        for (var i in areas) {
            var relations = DictionaryConvertFrom(areas[i].Relations);
            mapPanel.SignArea({
                Classification: areas[i].Classification,
                AreaId: areas[i].Id,
                ShapeId: 0,
                Relations: relations,
                Value: true,
                Center: new VELatLong(areas[i].Latitude, areas[i].Longitude)
            });
        }
    }
}


function LoadBlockGroupArea(areas) {
    if (mapPanel) {
        for (var i in areas) {
            var relations = DictionaryConvertFrom(areas[i].Relations);
            mapPanel.SignArea({
                Classification: areas[i].Classification,
                AreaId: areas[i].Id,
                ShapeId: 0,
                Relations: relations,
                Value: true,
                Center: new VELatLong(areas[i].Latitude, areas[i].Longitude)
            });
        }
    }
}

function LoadCrouteArea(areas) {
    if (mapPanel) {
        for (var i in areas) {
            var relations = DictionaryConvertFrom(areas[i].Relations);
            mapPanel.SignArea({
                Classification: areas[i].Classification,
                AreaId: areas[i].Id,
                ShapeId: 0,
                Relations: relations,
                Value: true,
                Center: new VELatLong(areas[i].Latitude, areas[i].Longitude)
            });
        }
    }
}

function DictionaryConvertFrom(items) {
    var dItems = new Object();
    for (var i in items) {
        if (typeof (items[i].Key) != 'undefined') {
            var key = items[i].Key;
            var value = items[i].Value;
            if (typeof (value[0]) != 'undefined') {
                dItems[key] = DictionaryConvertFrom(value);
            }
            else {
                dItems[key] = value;
            }
        }
    }
    return dItems;
}
