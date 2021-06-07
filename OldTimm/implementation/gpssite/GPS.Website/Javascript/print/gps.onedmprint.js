/// <reference path="../jquery-1.3.2.js" />
/// <reference path="../jquery-1.3.2-vsdoc2.js" />

var ndListSTRS = "";
var addressSTRS = "";
var oTask = "";
var isOrign = false;
var addressStr = "";
var submapDiv = $("<div></div>");
function GetUrlParms() {
    var args = new Object();
    var query = location.search.substring(1);
    var pairs = query.split("&");
    for (var i = 0; i < pairs.length; i++) {
        var pos = pairs[i].indexOf('=');
        if (pos == -1) continue;
        var argname = pairs[i].substring(0, pos);
        var value = pairs[i].substring(pos + 1);
        args[argname] = unescape(value);
    }
    return args;
}

function OnPageLoad() {
    GPS.Loading.show();
    var args = GetUrlParms();
    var campaignId = args["campaign"];
    if (campaignId) { LoadCampaign(campaignId); }
}

function OnPageLoadOneDM() {
    GPS.Loading.show();
    var args = GetUrlParms();
    var tid = args["id"];
    if (tid) { LoadTask(tid); }
}

function LoadTask(tid) {
    var service = new TIMM.Website.DistributionMapServices.DMReaderService();
    service.GetPrintTask(tid, function(task) {
        oTask = task;
        var dmId = task.DmId;
        LoadCampaignDM(dmId,tid);

    });
}

function LoadAddressStars(addressList) {
//    addressStr = "<div class='spaceline'></div><div align='center' style='font-weight: bold;font-size: 15px'>ADDRESSES LIST </div><table style='border-top:3px solid black;border-bottom:3px solid black;border-left:3px solid black;border-right:3px solid black;magin-top:36px;magin-bottom:36px;height:140px;'>";
//    var addStr = "";
    for (var i = 0; i < addressList.length; i++) {
        var starPath = "";
            if (addressList[i].Color == "red")
            {
                starPath = "images/pushpins/red-star.png";
            }else{
                starPath = "images/pushpins/green-star.png";
            }
            DrawAddress(addressList[i].Latitude, addressList[i].Longitude, starPath, i, addressList[i].Color);
//            if((i==0)||(i%5==0)){
//                addStr = addStr + "<tr>";
//            }
//            if (addressList[i].Picture != "") {
//                addStr = addStr + "<td><table style='width:200px;'><tr><td align='center' style='text-font:bold;'>" + (i + 1) + "<br/><img src='Files/Images/Address/" + addressList[i].Picture + "' width='150px;' height='150px;'><br/>" + addressList[i].Street + "</td></tr></table><br/></td>";
//            } else {
//            addStr = addStr + "<td><table style='width:200px;'><tr><td align='center' style='text-font:bold;'>" + (i + 1) + "<br/><img src='Files/Images/Address/noview.JPG' width='150px;' height='150px;'><br/>" + addressList[i].Street + "</td></tr></table><br/></td>";    
//            }
//            
//            if ((i % 5 == 4) || (i == (addressList.length-1))) {
//                addStr = addStr + "</tr>";
//            }
//            addressSTRS = addressSTRS + (i + 1) + "!" + addressList[i].Picture + "!" + addressList[i].Street + "&";
        }
//        addressStr = addressStr + addStr + "</table><div class='spaceline'></div><div class='spaceline'></div>";

//        $(submapDiv).append(addressStr);
    }

    function LoadMonitorAddress(addressList) {
        addressStr = "<div class='spaceline'></div><div align='center' style='font-weight: bold;font-size: 15px'>ADDRESSES LIST </div><table style='border-top:3px solid black;border-bottom:3px solid black;border-left:3px solid black;border-right:3px solid black;magin-top:36px;magin-bottom:36px;height:140px;'>";
        var addStr = "";
        for (var i = 0; i < addressList.length; i++) {
            var starPath = "";
//            if (addressList[i].Color == "red") {
//                starPath = "images/pushpins/red-star.png";
//            } else {
//                starPath = "images/pushpins/green-star.png";
//            }
            DrawMonitorAddress(addressList[i].Latitude, addressList[i].Longitude, i);
            if ((i == 0) || (i % 5 == 0)) {
                addStr = addStr + "<tr>";
            }
            if (addressList[i].Picture != "") {
                addStr = addStr + "<td><table style='width:200px;'><tr><td align='center' style='text-font:bold;'>" + (i + 1) + "<br/><img src='Files/Images/Address/" + addressList[i].Picture + "' width='150px;' height='150px;'><br/>#" + addressList[i].ZipCode +"<br/>" + addressList[i].Address1 + "</td></tr></table><br/></td>";
            } else {
            addStr = addStr + "<td><table style='width:200px;'><tr><td align='center' style='text-font:bold;'>" + (i + 1) + "<br/><img src='Files/Images/Address/noview.JPG' width='150px;' height='150px;'><br/>#" + addressList[i].ZipCode +"<br/>" +addressList[i].Address1 + "</td></tr></table><br/></td>";
            }

            if ((i % 5 == 4) || (i == (addressList.length - 1))) {
                addStr = addStr + "</tr>";
            }
            addressSTRS = addressSTRS + (i + 1) + "!" + addressList[i].Picture + "!" + addressList[i].Address1 + "!" + addressList[i].Latitude + "!" + addressList[i].Longitude + "!" + addressList[i].ZipCode + "&";
        }
        addressStr = addressStr + addStr + "</table><div class='spaceline'></div><div class='spaceline'></div>";

        $(submapDiv).append(addressStr);
    }

    function DrawCircle(addresses) {
        var i = 0;
        var length = addresses.length;
        while (i < length) {
            this.__DrawLocation__(addresses[i]);

            var j = 0;
            var rlength = addresses[i].Radiuses.length;
            while (j < rlength) {
                if (addresses[i].Radiuses[j].IsDisplay) {
                    this.__DrawCircle__(new VELatLong(addresses[i].Latitude, addresses[i].Longitude), addresses[i].Radiuses[j].Length, addresses[i].Radiuses[j].LengthMeasuresId);
                }
                j++;
            }

            i++;
        }
    }

    function __DrawCircle__(origin, radius, unit) {
        var earthRadius = 3959;
        if (unit == 2) { earthRadius = 6371; }

        //latitude in radius
        var lat = (origin.Latitude * Math.PI) / 180;

        //longitude in radius
        var lon = (origin.Longitude * Math.PI) / 180;

        //angular distance covered on earth's surface
        var d = parseFloat(radius) / earthRadius;
        var circleMarkPoints = [];
        var points = new Array();
        for (i = 0; i <= 360; i++) {
            var point = new VELatLong(0, 0)
            var bearing = i * Math.PI / 180; //rad
            point.Latitude = Math.asin(Math.sin(lat) * Math.cos(d) + Math.cos(lat) * Math.sin(d) * Math.cos(bearing));
            point.Longitude = ((lon + Math.atan2(Math.sin(bearing) * Math.sin(d) * Math.cos(lat), Math.cos(d) - Math.sin(lat) * Math.sin(point.Latitude))) * 180) / Math.PI;
            point.Latitude = (point.Latitude * 180) / Math.PI;
            points.push(point);
            if (i % 90 == 0) {
                circleMarkPoints.push(point);
            }
        }

        var circle = new VEShape(VEShapeType.Polyline, points);
        circle.SetLineColor(new VEColor(0, 0, 139, 0.5));
        circle.HideIcon();
        this._radiiLayer.AddShape(circle);

        var dmark = "";
        if (unit == 2) {
            dmark = Number(radius).toFixed(1) + "K";
        }
        else {
            dmark = Number(radius).toFixed(1) + "M";
        }
        var markIconFormat = '<span class="radiimark" style="position: relative; color: darkblue; font-size:12px;font-weight:bold; left: {1}px; top: {2}px;">{0}</span>';
        for (i = 0; i < 4; i++) {
            var markShape = new VEShape(VEShapeType.Pushpin, circleMarkPoints[i]);
            if (i == 0) {
                markShape.SetCustomIcon(markIconFormat.replace("{0}", dmark).replace("{1}", 0).replace("{2}", -6));
            }
            else if (i == 1) {
                markShape.SetCustomIcon(markIconFormat.replace("{0}", dmark).replace("{1}", 18).replace("{2}", 0));
            }
            else if (i == 2) {
                markShape.SetCustomIcon(markIconFormat.replace("{0}", dmark).replace("{1}", 0).replace("{2}", 16));
            }
            else {
                markShape.SetCustomIcon(markIconFormat.replace("{0}", dmark).replace("{1}", -20).replace("{2}", 0));
            }
            this._radiiLayer.AddShape(markShape);
        }
    }

function DrawAddress(inLat, inLon, starPath, numI, fillcol) {
    var offset = 0.00001;
    var points = [];
    dm._shape = new VEShape(VEShapeType.Pushpin, new VELatLong(inLat, inLon));
    dm._shape.LocationColor = fillcol;
    //var icon = fillcol.toString().replace('#', '');
    //var iconUrl = "<div style=\"background-image: url('Files/GtuDots/" + icon + '.png' + "');width:15px;height:15px;\"></div>";
    var iconUrl = "<div style=\"background-image: url('" + starPath + "');width:50px;height:50px;font-weight: bold;\">" + (numI+1) + "></div>";
    dm._shape.SetCustomIcon(iconUrl);
    dm.PrintMap._locationLayer.AddShape(dm._shape);
}

function DrawMonitorAddress(inLat, inLon, numI) {
    var offset = 0.00001;
    var points = [];
    dm._shape = new VEShape(VEShapeType.Pushpin, new VELatLong(inLat, inLon));
    //var icon = fillcol.toString().replace('#', '');
    //var iconUrl = "<div style=\"background-image: url('Files/GtuDots/" + icon + '.png' + "');width:15px;height:15px;\"></div>";
    var iconUrl = "<div style=\"background-image: url('Files/GtuDots/maddress.PNG');width:15px;height:15px;font-weight: bold;\"><br/>" + (numI + 1) + "</div>";
    dm._shape.SetCustomIcon(iconUrl);
    dm.PrintMap._locationLayer.AddShape(dm._shape);
}


function ValidateLoad() {
    if (campaignLoaded && submapsLoaded) {
        GPS.Loading.hide();
    }
}

var campaign = null;

var campaignLoaded = false;
var submapsLoaded = false;
var lastSubMapId = 0;


function LoadCampaignDM(dmId,tid) {
    var service = new TIMM.Website.DistributionMapServices.DMReaderService();
    service.GetPrintDM(dmId, function(toPrintDM) {
        dm = toPrintDM;
        //BindDMs();
        BindDM(dm);

        //get address info
        var addressservice = new TIMM.Website.TaskServices.TaskReaderService();
        addressservice.GetAddressByTaskId(tid, function(addressList) {
            if (addressList.length > 0) {
                LoadAddressStars(addressList);
            }

        });
        
        //get monitor address info
        var maddressservice = new TIMM.Website.TaskServices.TaskReaderService();
        maddressservice.GetMonitorAddressByTaskId(tid, function(maddresslist) {
        if (maddresslist.length>0) {
                LoadMonitorAddress(maddresslist);    
            }
        });

        GPS.Loading.hide();

    });
}

function LoadGTUInfos(task) {
    for(var i=0;i< task.Taskgtuinfomappings.length;i++){
        var col="";
        col=task.Taskgtuinfomappings[i].UserColor;
        var len = task.Taskgtuinfomappings[i].Gtuinfos.length;
        
        var oCollection = [];
        for (var j = 0; j < len; j++) {
            var aa = [];
            aa[0] =task.Taskgtuinfomappings[i].Gtuinfos[j].PowerInfo + '&' + task.Taskgtuinfomappings[i].Gtuinfos[j].dwLatitude;
            aa[1] = task.Taskgtuinfomappings[i].Gtuinfos[j].dwLongitude;
            oCollection.push(aa);
        }
        AddEllipseS(oCollection,col,col);
    }
}

function AddEllipseS(oCollection, lineCol, fillCol) {
    var al = oCollection.length;
    var i = 0;
    for (i = 0; i < al; i++) {
        var aa = [];
        //var isOrign;
        aa = oCollection[i][0].split("&");
        if (aa[0] == 9)
            isOrign = false;
        else
            isOrign = true;
        var lat = aa[1];
        var lon = oCollection[i][1];
        DrawDot(lat,lon,fillCol,isOrign);
    }
}

function DrawDot(inLat, inLon, fillcol, isOrign) {
    var offset = 0.00001;
    var points = [];
    dm._shape = new VEShape(VEShapeType.Pushpin, new VELatLong(inLat, inLon));
    dm._shape.LocationColor = fillcol;
//    points.push(new VELatLong(parseFloat(inLat) + offset, parseFloat(inLon) + offset), new VELatLong(parseFloat(inLat) + offset, parseFloat(inLon) - offset), new VELatLong(parseFloat(inLat) - offset, parseFloat(inLon) - offset), new VELatLong(parseFloat(inLat) - offset, parseFloat(inLon) + offset));
//    dm._shape = new VEShape(VEShapeType.Polygon, points);
//    dm._shape.SetLineColor(new VEColor(00, 00, 00, 0));
    var icon = fillcol.toString().replace('#', '');
    var iconUrl = "<div style=\"background-image: url('Files/GtuDots/" + icon + '.png' + "');width:15px;height:15px;\"></div>";
    dm._shape.SetCustomIcon(iconUrl);
    if (isOrign == true)
        dm.PrintMap._ogdLayer.AddShape(dm._shape);
    else
        dm.PrintMap._agdLayer.AddShape(dm._shape);
}
//function DrawDot(inLat, inLon, fillcol, isOrign) {
//        var spss = [];
//        var radius = 0.025;
//        var earthRadius = 3959;
//        var lat = (inLat * Math.PI) / 180;
//        var lon = (inLon * Math.PI) / 180;
//        var d = parseFloat(radius) / earthRadius;
//        var points = new Array();
//        for (i = 0; i <= 360; i++) {
//            var point = new VELatLong(0, 0);
//            var bearing = i * Math.PI / 180;
//            point.Latitude = Math.asin(Math.sin(lat) * Math.cos(d) + Math.cos(lat) * Math.sin(d) * Math.cos(bearing));
//            point.Longitude = ((lon + Math.atan2(Math.sin(bearing) * Math.sin(d) * Math.cos(lat), Math.cos(d) - Math.sin(lat) * Math.sin(point.Latitude))) * 180) / Math.PI;
//            point.Latitude = (point.Latitude * 180) / Math.PI;
//            points.push(point);

//        }
//        dm._shape = new VEShape(VEShapeType.Polygon, points);
//        setColor(fillcol);
//        dm._shape.SetLineColor(new VEColor(00, 00, 00, 0.8));
//        dm._shape.SetLineWidth(1);
//        dm._shape.HideIcon();
//        if(isOrign==true)
//            dm.PrintMap._ogdLayer.AddShape(dm._shape);
//        else
//            dm.PrintMap._agdLayer.AddShape(dm._shape);
//    }
    
function setColor(fillcolor) {
        var r = parseInt("0x" + fillcolor.slice(1, 3));
        var g = parseInt("0x" + fillcolor.slice(3, 5));
        var b = parseInt("0x" + fillcolor.slice(5, 7));
        dm._shape.SetFillColor(new VEColor(r, g, b, 0.7));
    }

function on_change_gtuPrint(obj) {
    if (obj == "rdAll")
        dm.PrintMap.ShowAddedGTUDots();
    else
        dm.PrintMap.HideAddedGTUDots();
}

function AttachTotal() {
    var total = 0;
    var count = 0;
    var pen = 0;
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        total += submaps[i].Total;
        count += submaps[i].Penetration;
        i++;
    }
    if (total == 0) { pen = 0; }
    else { pen = count / total; }
    campaign.Total = total;
    campaign.Count = count;
    campaign.Pen = pen;
}

function checkMaxLen(obj) {
    if (obj.value.length > 100) {
        obj.value = obj.value.substr(0, 160);
    }
}

function BindDM(dm) {
    //var submapDiv = $("<div></div>");
    $('#print-submaps-content').append($(submapDiv));

    LoadDMItems(submapDiv, dm);
}

function BindDMSummary(submapDiv, dm) {
    var dmSumary = $("<table  class=\"submap\" cellspacing=\"0\" cellpadding=\"4\" ></table>");
    $(dmSumary).append($("<caption>DISTRIBUTION MAP {0} ({1})</caption>".replace("{0}", dm.Id).replace("{1}", dm.Name)));
    var lineFormat = "<tr><td class=\"leftlabel\">{0}</td><td>{1}</td></tr>";
    var noString = lineFormat.replace("{0}", "DISTRIBUTION MAP #:").replace("{1}", dm.Id);
    var nameString = lineFormat.replace("{0}", "DISTRIBUTION MAP NAME:").replace("{1}", dm.Name);
    var totalString = lineFormat.replace("{0}", "TOTAL:").replace("{1}", Number.addCommas(dm.Total));
    var countString = lineFormat.replace("{0}", "COUNT:").replace("{1}", Number.addCommas(dm.Penetration));
    var penString = lineFormat.replace("{0}", "PENETRATION:").replace("{1}", (dm.Percentage*100).toFixed(2) + "%");
    $(dmSumary).append($(noString));
    $(dmSumary).append($(nameString));
    $(dmSumary).append($(totalString));
    $(dmSumary).append($(countString));
    $(dmSumary).append($(penString));
    $(submapDiv).append($(dmSumary));

}

function LoadDMItems(submapDiv, dm) {
    BindDMMap(submapDiv, dm);
}

function BindDMMap(submapDiv, dm) {
    var id = "submapmap" + dm.Id;
    $(submapDiv).append($("<table cellspacing=\"0\" cellpadding=\"0\" class=\"maptable\" ><caption>" + dm.Name + "</caption><tbody><tr><td><div id='" + id + "' class='submapmap'></div></td></tr></tbody></table>"));
    var printmap = new PrintMap(id);
    printmap.LoadMap();
    
    dm.MapDivId = id;
    dm.MapObj = printmap._map;
    dm.PrintMap = printmap;

    var sps = [];
    var i = 0;


    var jl = dm.DistributionMapCoordinates.length;
    if (jl == 0) {
        $(submapDiv).append("<div class='spaceline'></div><div class='spaceline'></div>");
        return;
    }

    var s_minLat, s_maxLat, s_minLon, s_maxLon;
    while (i < jl) {        
        var currentLatitude = dm.DistributionMapCoordinates[i].Latitude;
        var currentLongitude = dm.DistributionMapCoordinates[i].Longitude;
        sps.push(new VELatLong(currentLatitude, currentLongitude));

        if (i == 0) {
            s_minLat = s_maxLat = currentLatitude;
            s_minLon = s_maxLon = currentLongitude;
        }
        if (currentLatitude < s_minLat)
            s_minLat = currentLatitude;
        if (currentLatitude > s_maxLat)
            s_maxLat = currentLatitude;
        if (currentLongitude < s_minLon)
            s_minLon = currentLongitude;
        if (currentLongitude > s_maxLon)
            s_maxLon = currentLongitude;
        
        i++;
    }


    dm._shape = new VEShape(VEShapeType.Polygon, sps);
    dm._shape.SetFillColor(new VEColor(0, 0, 0, 0.1));
    dm._shape.SetLineColor(new VEColor(00, 00, 00, 0.8));
    dm._shape.SetLineWidth(2);
    dm._shape.HideIcon();
    var layer = printmap._submapLayer;
    if (layer) {
        layer.AddShape(dm._shape);
        var points = [];
        points = dm._shape.GetPoints();
        dm.PrintMap._map.SetMapView(points);
    }

//    var level = printmap._map.GetZoomLevel();

//    var lat = printmap._map.GetCenter().Latitude;
//    var lon = printmap._map.GetCenter().Longitude;
//   
    var minLat = printmap._map.GetMapView().BottomRightLatLong.Latitude;
    var maxLat = printmap._map.GetMapView().TopLeftLatLong.Latitude;
    var minLon = printmap._map.GetMapView().TopLeftLatLong.Longitude;
    var maxLon = printmap._map.GetMapView().BottomRightLatLong.Longitude;

//    printmap._map.ZoomIn();
//    var current_minLat = printmap._map.GetMapView().BottomRightLatLong.Latitude;
//    var current_maxLat = printmap._map.GetMapView().TopLeftLatLong.Latitude;
//    var current_minLon = printmap._map.GetMapView().TopLeftLatLong.Longitude;
//    var current_maxLon = printmap._map.GetMapView().BottomRightLatLong.Longitude;
//        minLat = current_minLat;
//        maxLat = current_maxLat;
//        minLon = current_minLon;
//        maxLon = current_maxLon;    
        
    //get dm screen all boxes
    var boxes = [];
    boxes = _GetScreenBoxes(dm.PrintMap._map, minLat, maxLat, minLon, maxLon);

    LoadGTUInfos(oTask);
    
    //get nd custom areas and nd addresses by boxids 
    var service = new TIMM.Website.DistributionMapServices.DMReaderService();
    service.GetCustomAreaByBox(boxes, function(toAreaList) {

        var ndListStr = "<table style='border-top:3px solid black;border-bottom:3px solid black;border-left:3px solid black;border-right:3px solid black;magin-top:36px;magin-bottom:36px;height:140px;'>";

        var length = toAreaList.length;
        var k = 0;
        var count = 0;
        while (k < length) {

            var i = 0;
            var toArea = toAreaList[k];
            var jl = toArea.Locations.length;

            var isValid = false;
            for (i = 0; i < jl; i++) {
                if (toArea.Locations[i].Latitude > minLat && toArea.Locations[i].Latitude < maxLat
                  && toArea.Locations[i].Longitude > minLon && toArea.Locations[i].Longitude < maxLon) {
                    isValid = true;
                    break;
                }
            }

            if (isValid) {

                var sps = [];
                i = 0;

                while (i < jl) {
                    sps.push(new VELatLong(toArea.Locations[i].Latitude, toArea.Locations[i].Longitude));
                    i++;
                }
                dm._shape = new VEShape(VEShapeType.Polygon, sps);
                dm._shape.SetFillColor(new VEColor(238, 44, 44, 0.7));
                dm._shape.SetLineColor(new VEColor(00, 00, 00, 0.8));
                dm._shape.SetLineWidth(1);
                dm._shape.SetCustomIcon('<div style="font-size:24px;font:bold">' + (count + 1).toString() + '</div>');
                dm.PrintMap._ndLayer.AddShape(dm._shape);

                // draw nd custom
                if (toArea.Attributes.length == 1 && toArea.Attributes[0].Key == "OTotal") {

                    ndListStr = ndListStr + "<tr><td class='rowlabel'>" + (count + 1) + ". " + toArea.Name + ", " + toArea.Attributes[0].Value + (toArea.Description == "" ? "" : ", " + toArea.Description) + "</td></tr>";
                    ndListSTRS = ndListSTRS + (count + 1) + ". " + toArea.Name + ", " + toArea.Attributes[0].Value + (toArea.Description == "" ? "" : ", " + toArea.Description) + ";";
                }
                // draw nd address
                else if (toArea.Attributes.length == 1 && toArea.Attributes[0].Key == "Geofence") {

                    ndListStr = ndListStr + "<tr><td class='rowlabel'>" + (count + 1) + ". " + toArea.Name + (toArea.Description == "" ? "" : ", " + toArea.Description) + "</td></tr>";
                    ndListSTRS = ndListSTRS + (count + 1) + ". " + toArea.Name + (toArea.Description == "" ? "" : ", " + toArea.Description) + ";";
                }

                count++;
            }
            k++;
        }

        if (count > 0) {
            ndListStr = "<div class='spaceline'></div><div align='center' style='font-weight: bold;font-size: 15px'>DO NOT DISTRIBUTE LIST "
            + dm.Name
            + "</div><div class='spaceline'></div>" + ndListStr;
        }
        ndListStr += "</table><div class='spaceline'></div><div class='spaceline'></div>";
        ndListSTRS += "$" + dm.Id + "@";
        //$(submapDiv).append(ndListStr);
        //dm.PrintMap._map.SetCenter(new VELatLong(lat, lon));
        //dm.PrintMap._map.SetCenterAndZoom(new VELatLong(lat, lon), level);

    });

    //LoadGTUInfos(oTask, dm.PrintMap._map);

}

function _GetScreenBoxes(_map,minLat,maxLat,minLon,maxLon) {
    var boxes = [];
//    var minLat = _map.GetMapView().BottomRightLatLong.Latitude;
//    var maxLat = _map.GetMapView().TopLeftLatLong.Latitude;
//    var minLon = _map.GetMapView().TopLeftLatLong.Longitude;
//    var maxLon = _map.GetMapView().BottomRightLatLong.Longitude;
    //custom area
    var mountLat = 25;
    var mountLon = 40;

    var lat = Math.floor(minLat * 100);
    var lon = Math.floor(minLon * 100);
    lat = lat - (lat % mountLat);
    if (lat < 0) { lat -= mountLat; }
    lon = lon - (lon % mountLon);
    if (lon < 0) { lon -= mountLon; }

    while (lat < maxLat * 100) {
        var tempLon = lon;
        while (tempLon < maxLon * 100) {
            var id = lat * 100000 + tempLon;
            //if (!this._areaBoxes.ContainsKey(id)) {
            //boxes.push(new GPS.Map.AreaBox(id, 13));
            boxes.push(id);
            //}
            tempLon += mountLon;
        }
        lat += mountLat;
    }
    return boxes;
}

function ParseShape(area, number) {
    var points = [];
    var i = 0;
    var length = area.Locations.length;
    while (i < length) {
        points.push(new VELatLong(area.Locations[i].Latitude, area.Locations[i].Longitude));
        i++;
    }
    var shape = new VEShape(VEShapeType.Polygon, points);
    shape.SetCustomIcon('<div class="areaicon">' + number + '</div>');
    //    shape.HideIcon();
    return shape;
}

function OnPrintClickDM() {
    var campainStr = GetCampaignStrDM();
    $('#campaign').val(campainStr);
    return true;
}

function OnPrintClickOneDM() {
    var currentDMStr = GetCampaignStrDM();
    $('#currentDM').val(currentDMStr);
    return true;
}


function GetCampaignStrDM() {
    var items = [];
    items.push(GetCampaignDMsStr(dm));
    items.push(ndListSTRS);
    items.push(addressSTRS);
    
    
    return items.join('~');
}

function GetCampaignDMsStr(dm) {
    var items = [];
    items.push(GetCampaignDMStr(dm));
    return items.join('^');
}

function GetCampaignDMStr(dm) {
    var items = [];
    var pen = dm.Total > 0 ? dm.Penetration / dm.Total * 100 : 0;
    var printmap = dm.PrintMap;
    items.push(dm.Id);
    items.push(dm.Name);
    items.push(dm.Total);
    items.push(dm.Penetration);
    items.push(pen.toFixed(2) + '%');
    items.push(printmap.GetMapImagesStr());
    items.push(printmap.GetMapShapesStr());
    items.push(printmap.GetMapPushpinsStr());
    items.push(printmap.GetLocationsStr());
    items.push(printmap.GetScaleStr());
    items.push(GetAreasStr(dm.FiveZipAreas));
    items.push(GetAreasStr(dm.CRoutes));
    items.push(GetAreasStr(dm.Tracts));
    items.push(GetAreasStr(dm.BlockGroups));
    items.push(printmap.GetNdStr());
    items.push(printmap.GetgdStr());
    return items.join('*');
}

function GetAreasStr(areas) {
    var items = [];
    if (PrintSettings.ShowSubMapCount) {
        var i = 0;
        var length = areas.length;
        while (i < length) {
            if (areas[i].IsEnabled) {
                items.push(GetAreaStr(areas[i]));
            }
            i++;
        }
    }
    return items.join(';');
}

function GetAreaStr(area) {
    var items = [];
    items.push(area.DisplayOrderId);
    items.push(area.Code);
    items.push(area.Total);
    items.push(area.Count);
    items.push(area.Percentage);
    return items.join(',');
}

function PadLeft(str, count, charStr) {
    var disstr = "";
    if (str.length >= count) {
        return str;
    }
    for (var i = 1; i <= (count - str.length); i++) {
        disstr += charStr;
    }
    return disstr + str;
}


/*
*/


function OnPrintOptionClickDM(sendor, action) {
    var visible = !sendor.checked;
    }

function ChangeShowLocationDM(isShow) {}

function ChangeShowRadiiDM(isShow) {}

function ChangeShowNonDeliverables(isShow) {}

function ChangeShowNonDeliverablesDM(isShow) {}

function ChangeShowSubMaps(isShow) {
    if (isShow) {
        $('#print-submaps-content').show();
        $('#campaign-submaps-summary').show();
    }
    else {
        $('#print-submaps-content').hide();
        $('#campaign-submaps-summary').hide();
    }
    PrintSettings.ShowSubMaps = isShow;
}

function ChangeShowSubMapCount(isShow) {
    if (isShow) { $('#print-submaps-content').find('.submapitem').show(); }
    else { $('#print-submaps-content').find('.submapitem').hide(); }
    PrintSettings.ShowSubMapCount = isShow;
}

function ChangeShowClassificationOutLines(isShow) {}

function ChangeShowClassificationOutLinesDM(isShow) {}

function ChangeShowPenetrationColor(isShow) {}

function ChangeShowPenetrationColorDM(isShow) {}

function ChangePenetrationColor(isChange) {}

function ChangePenetrationColorDM(isChange) {}

function GetNumber(number) {
    number = Math.round(number * 100);
    return number > 100 ? 100 : (number < 0 ? 0 : number);
}

function ColorPointChanged(obj) {

    if (ValidateColorPoint(obj)) {
        var points = PenetrationColorSettings.GetColorPoints(true);
        var p1 = Number(GetNumber(points[0]));
        var p2 = Number(GetNumber(points[1]));
        var p3 = Number(GetNumber(points[2]));
        var p4 = Number(GetNumber(points[3]));

        $('#print-color-point1').val(p1);
        $('#print-color-point2').val(p2);
        $('#print-color-point3').val(p3);
        $('#print-color-point4').val(p4);

        $('#print-color-td1').css('width', p1 * 3 + 'px');
        $('#print-color-td2').css('width', (p2 - p1) * 3 + 'px');
        $('#print-color-td3').css('width', (p3 - p2) * 3 + 'px');
        $('#print-color-td4').css('width', (p4 - p3) * 3 + 'px');
        $('#print-color-td5').css('width', (100 - p4) * 3 + 'px');
        if (PrintSettings.ShowPenetrationColors) {
            ChangeShowPenetrationColor(true);
        }
    }
}

function ColorPointChangedDM(obj) {

    if (ValidateColorPoint(obj)) {
        var points = PenetrationColorSettings.GetColorPoints(true);
        var p1 = Number(GetNumber(points[0]));
        var p2 = Number(GetNumber(points[1]));
        var p3 = Number(GetNumber(points[2]));
        var p4 = Number(GetNumber(points[3]));

        $('#print-color-point1').val(p1);
        $('#print-color-point2').val(p2);
        $('#print-color-point3').val(p3);
        $('#print-color-point4').val(p4);

        $('#print-color-td1').css('width', p1 * 3 + 'px');
        $('#print-color-td2').css('width', (p2 - p1) * 3 + 'px');
        $('#print-color-td3').css('width', (p3 - p2) * 3 + 'px');
        $('#print-color-td4').css('width', (p4 - p3) * 3 + 'px');
        $('#print-color-td5').css('width', (100 - p4) * 3 + 'px');
        if (PrintSettings.ShowPenetrationColors) {
            ChangeShowPenetrationColorDM(true);
        }
    }
}

function ValidateColorPoint(obj) {
    var valid = false;
    var strP = /^\d+(\.\d+)?$/;
    var index = Number(obj.id.substring(obj.id.length - 1));
    if (strP.test(obj.value)) {
        var point = Number(obj.value);
        var prevPoint = index < 2 ? 0 : PenetrationColorSettings.GetColorPoints(true)[index - 2]
        var nextPoint = index > 3 ? 1 : PenetrationColorSettings.GetColorPoints(true)[index];
        if ((point > prevPoint * 100 || (prevPoint * 100 <= 0 && point == 0)) && nextPoint > 0) {
            PenetrationColorSettings.SetColorPoint(index, point / 100, true);
            valid = true;
        }
    }
    if (!valid) { $(obj).val(GetNumber(PenetrationColorSettings.GetColorPoints(true)[index - 1])); }
    return valid;
}


