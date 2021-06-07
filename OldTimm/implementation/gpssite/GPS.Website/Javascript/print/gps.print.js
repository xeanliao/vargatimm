/// <reference path="../jquery-1.3.2.js" />
/// <reference path="../jquery-1.3.2-vsdoc2.js" />

var ndListSTRS = "";
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

function OnPageLoadDM() {
    GPS.Loading.show();
    var args = GetUrlParms();
    var campaignId = args["campaign"];
    if (campaignId) { LoadCampaignDM(campaignId); }
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


function LoadCampaign(campaignId) {
    //    var params = [];
    //    params.push('method=load');
    //    params.push('campaignId=' + campaignId);
    //    $.ajax({
    //        type: "POST",
    //        url: "Handler/CampaignHandler.ashx",
    //        data: params.join('&'),
    //        dataType: 'json',
    //        success: function(data) {
    //            if (data) {
    //                campaign = data;
    //                BindCoverPage();
    //                BindCampaign();
    //                campaignLoaded = true;
    //                ValidateLoad();
    //            }
    //        }
    //    });
    
//    var service = new TIMM.Website.CampaignServices.CampaignReaderService();
//    service.GetPrintCampaign(campaignId, function(toPrintCampaign) {
//        campaign = toPrintCampaign;
//        if (campaign.SubMaps.length > 0) {
//            campaign.SubMaps.sort(function(a, b) { return a.Id - b.Id; });
//        }
//        BindCoverPage();
//        BindCampaignSummary();
//        BindCampaignSubMapsSummary();
//        BindCampaignMap();
//        BindSubMaps();
//        GPS.Loading.hide();
//    },
//    function(error) {
//     }
//   );

    var params = [];
    params.push("cid=" + campaignId);
    $.ajax({
        type: "get",
        url: "Handler/CampaignMapPrint.ashx",
        data: params.join('&'),
        dataType: "json",
        success: function(toPrintCampaign, textStatus) {
            campaign = toPrintCampaign;
            var len = toPrintCampaign.CampaignPercentageColors.length;
            for (var i = 0; i < len; i++) {
                var color = toPrintCampaign.CampaignPercentageColors[i];
                if (color.min != -1 && color.max != -1) {
                    PenetrationColorSettings._settings[color.ColorId - 1].Min = color.Min;
                    PenetrationColorSettings._settings[color.ColorId - 1].Max = color.Max;
                }
            }

            if (campaign.SubMaps.length > 0) {
                campaign.SubMaps.sort(function(a, b) { return a.Id - b.Id; });
            }
            BindCoverPage();
            BindCampaignSummary();
            BindCampaignSubMapsSummary();
            BindCampaignMap();
            BindSubMaps();
            GPS.Loading.hide();
        }
    });
}

function LoadCampaignDM(campaignId) {
//    var service = new TIMM.Website.DistributionMapServices.DMReaderService();
//    service.GetPrintCampaign(campaignId, function(toPrintCampaign) {
//        campaign = toPrintCampaign;
//        if (campaign.SubMaps.length > 0) {
//            campaign.SubMaps.sort(function(a, b) { return a.Id - b.Id; });
//        }
//        //BindCoverPage();
//        //BindCampaignSummary();
//        //BindCampaignSubMapsSummary();
//        //BindCampaignDMSummary();
//        //BindCampaignDMMap();
//        //BindSubMaps();
//        BindDMs();
//        GPS.Loading.hide();
    //    });

    var params = [];
    params.push("cid=" + campaignId);
    $.ajax({
        type: "get",
        url: "Handler/CampaignMapPrint.ashx",
        data: params.join('&'),
        dataType: "json",
        success: function(toPrintCampaign, textStatus) {
            campaign = toPrintCampaign;
            if (campaign.SubMaps.length > 0) {
                campaign.SubMaps.sort(function(a, b) { return a.Id - b.Id; });
            }
            BindDMs();         
            GPS.Loading.hide();
        }
    });
}

function BindCoverPage() {
    var separaterFormat = "<tr><td style=\"height: {0}px;\"></td></tr>";
    var labelFormat = "<tr><td style=\"font-weight: bold;font-size: 23px;\">{0}</td></tr>";
    var lineFormat = "<tr><td style=\"font-size: 23px;\">{0}</td></tr>";

    var logoString = separaterFormat.replace("{0}", 80);
    if (campaign.Logo && campaign.Logo.length > 0) {
        logoString += lineFormat.replace("{0}", "<img src=\"Files/Images/{1}\" />".replace("{1}", campaign.Logo));
    }
    var clientNameString = separaterFormat.replace("{0}", 20);
    clientNameString += labelFormat.replace("{0}", "Client Name:");
    clientNameString += lineFormat.replace("{0}", campaign.ClientName);

    var createdForString = separaterFormat.replace("{0}", 20);
    createdForString += labelFormat.replace("{0}", "Created for:");
    createdForString += lineFormat.replace("{0}", campaign.ContactName);

    var createdOnString = separaterFormat.replace("{0}", 20);
    createdOnString += labelFormat.replace("{0}", "Created on:");
    createdOnString += lineFormat.replace("{0}", $.datepicker.formatDate("mm-dd-yy", new Date(campaign.Date)));

    var presentedByString = separaterFormat.replace("{0}", 80);
    presentedByString += labelFormat.replace("{0}", "Presented by:");
    presentedByString += separaterFormat.replace("{0}", 20);
    presentedByString += lineFormat.replace("{0}", "<img src=\"Images/vargainc-logo.png\" height=\"90px\" />");

    var masterCampaignString = separaterFormat.replace("{0}", 20);
    masterCampaignString += labelFormat.replace("{0}", "Master Campaign #:");
    masterCampaignString += lineFormat.replace("{0}", campaign.CompositeName);

    var createdByString = separaterFormat.replace("{0}", 20);
    createdByString += labelFormat.replace("{0}", "Created by:");
    createdByString += lineFormat.replace("{0}", campaign.UserFullName);

    var coverPage = $("#cover-page");
    $(coverPage).append($(logoString));
    $(coverPage).append($(clientNameString));
    $(coverPage).append($(createdForString));
    $(coverPage).append($(createdOnString));
    $(coverPage).append($(presentedByString));
    $(coverPage).append($(masterCampaignString));
    $(coverPage).append($(createdByString));
}

//function BindCampaign() {
//    BindCampaignSummary();
//    BindCampaignSubMapsSummary();
//    BindCampaignMap();
//    BindSubMaps();
//}

function BindCampaignMap() {
    var printmap = new PrintMap('campaigninfo-map');
    printmap.LoadMap();
    printmap.SetAddresses(campaign.Addresses);
    printmap.AddCampaignSubMaps(campaign.SubMaps);
    campaign.PrintMap = printmap;
}




function BindCampaignDMMap() {
    var printmap = new PrintMap('campaigninfo-map');
    printmap.LoadMap();
    printmap.SetAddresses(campaign.Addresses);
    printmap.AddCampaignDM(campaign.SubMaps);
    campaign.PrintMap = printmap;
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
function BindCampaignSummary() {
    var campaignSummary = $('#campaign-summary');
    AttachTotal();
    var lineFormat = "<tr><td class=\"label\">{0}</td><td>{1}</td></tr>";
    var nameString = lineFormat.replace("{0}", "MASTER CAMPAIGN #:").replace("{1}", campaign.CompositeName);
    var descriptionString = lineFormat.replace("{0}", "TARGETING METHOD:").replace("{1}", "<input name='targetingmethod1' id='targetingmethod1' type='text' value='{0}' /><br /><input name='targetingmethod2' id='targetingmethod2' type='text' />".replace("{0}", campaign.Description));
    var clientString = lineFormat.replace("{0}", "CLIENT NAME:").replace("{1}", campaign.ClientName);
    var contactString = lineFormat.replace("{0}", "CONTACT NAME:").replace("{1}", campaign.ContactName);
    var totalString = lineFormat.replace("{0}", "TOTAL HOUSEHOLDS:").replace("{1}", Number.addCommas(campaign.Total));
    var countString = lineFormat.replace("{0}", "TARGET HOUSEHOLDS:").replace("{1}", Number.addCommas(campaign.Count));
    var penString = lineFormat.replace("{0}", "PENETRATION:").replace("{1}", (campaign.Pen * 100).toFixed(2) + "%");
    $(campaignSummary).append($(nameString));
    $(campaignSummary).append($(clientString));
    $(campaignSummary).append($(contactString));
    $(campaignSummary).append($(descriptionString));
    $(campaignSummary).append($(totalString));
    $(campaignSummary).append($(countString));
    $(campaignSummary).append($(penString));

}

function BindColorLegend() {
    //var item = [];
    //var colFormat = "<td id=legBlue ><div style='height: 12px; width: 25px; background-color:Blue;'></td><td class=\"label\">Blue 0-0</td>";
    var colorLegend = $('.colorlegend');
    $(colorLegend).empty();
    if (PrintSettings.ShowPenetrationColors) {
        if (PrintSettings.ChangePenetrationColor)
            $(colorLegend).append($(PenetrationColorSettings.GetCollegendStr(true)));
        else
            $(colorLegend).append($(PenetrationColorSettings.GetCollegendStr(false)));
    }

    //item=PenetrationColorSettings.Serialize(false);
}

function BindCampaignSubMapsSummary() {
    var campaignSubMapsSummary = $('#campaign-submaps-summary');
    $(campaignSubMapsSummary).find('tr').remove();
    $(campaignSubMapsSummary).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"label\" style=\"width: 10%;\">#</td><td class=\"label\" style=\"width: 30%;\">SUB MAP NAME</td><td class=\"label\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"label\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"label\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        var pen = submaps[i].Total == 0 ? 0 : submaps[i].Penetration / submaps[i].Total * 100;

        var row;
        if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
        else { row = $("<tr></tr>"); }
        //row.append($("<td>{0}</td>".replace("{0}", submaps[i].Id)));
        row.append($("<td>{0}</td>".replace("{0}", i + 1)));
        row.append($("<td>{0}</td>".replace("{0}", submaps[i].Name)));
        row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(submaps[i].Total))));
        row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(submaps[i].Penetration))));
        row.append($("<td style=\"text-align: right;\">{0}%</td>".replace("{0}", pen.toFixed(2))));

        $(campaignSubMapsSummary).append($(row));
        i++;
    }
}

function BindCampaignDMSummary() {
    var campaignSubMapsSummary = $('#campaign-submaps-summary');
    $(campaignSubMapsSummary).find('tr').remove();
    $(campaignSubMapsSummary).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"label\" style=\"width: 10%;\">#</td><td class=\"label\" style=\"width: 30%;\">DISTRIBUTION MAP NAME</td></tr>"));
    var submaps = campaign.SubMaps;
    var j=0;
    var len = submaps.length;
    while(j<len){
        var dms = submaps[j].DistributionMaps;
        var i = 0;
        var length = dms.length;
        while (i < length) {
//            var pen = submaps[i].Total == 0 ? 0 : submaps[i].Penetration / submaps[i].Total * 100;

            var row;
            if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
            else { row = $("<tr></tr>"); }
            row.append($("<td>{0}</td>".replace("{0}", dms[i].Id)));
            row.append($("<td>{0}</td>".replace("{0}", dms[i].Name)));
    //        row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(submaps[i].Total))));
    //        row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(submaps[i].Penetration))));
    //        row.append($("<td style=\"text-align: right;\">{0}%</td>".replace("{0}", pen.toFixed(2))));

            $(campaignSubMapsSummary).append($(row));
            i++;
        }
        j++;
    }
}

function BindSubMaps() {
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        BindSubMap(submaps[i],i+1);
        i++;
    }

    //    if (length > 0) {
    //        lastSubMapId = submaps[length - 1].Id;
    //    }
    //    else {
    //        submapsLoaded = true;
    //        ValidateLoad();
    //    }
}
function BindDMs() {
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        var j = 0;
        var len = submaps[i].DistributionMaps.length;
        while(j<len){
            BindDM(submaps[i].DistributionMaps[j]);
            j++;
        }
        i++;
        
    }

    
    //    if (length > 0) {
    //        lastSubMapId = submaps[length - 1].Id;
    //    }
    //    else {
    //        submapsLoaded = true;
    //        ValidateLoad();
    //    }
}
function BindDM(dm) {
    var submapDiv = $("<div></div>");
    $('#print-submaps-content').append($(submapDiv));

    LoadDMItems(submapDiv, dm);
}
function BindSubMap(submap,smId) {
    var submapDiv = $("<div></div>");
    $('#print-submaps-content').append($(submapDiv));

    LoadSubMapItems(submapDiv, submap,smId);
}

function BindSubMapSummary(submapDiv, submap,smId) {
    var submapSumary = $("<table  class=\"submap\" cellspacing=\"0\" cellpadding=\"4\"></table>");
    //$(submapSumary).append($("<caption>SUB MAP {0} ({1})</caption>".replace("{0}", submap.Id).replace("{1}", submap.Name)));
    $(submapSumary).append($("<caption>SUB MAP {0} ({1})</caption>".replace("{0}", smId).replace("{1}", submap.Name)));
    
    var lineFormat = "<tr><td class=\"leftlabel\">{0}</td><td>{1}</td></tr>";
    var noString = lineFormat.replace("{0}", "SUB MAP #:").replace("{1}", smId);
    var nameString = lineFormat.replace("{0}", "SUB MAP NAME:").replace("{1}", submap.Name);
    var totalString = lineFormat.replace("{0}", "TOTAL HOUSEHOLDS:").replace("{1}", Number.addCommas(submap.Total));
    var countString = lineFormat.replace("{0}", "TARGET HOUSEHOLDS:").replace("{1}", Number.addCommas(submap.Penetration));
    var penString = lineFormat.replace("{0}", "PENETRATION:").replace("{1}", (submap.Percentage*100).toFixed(2) + "%");
    $(submapSumary).append($(noString));
    $(submapSumary).append($(nameString));
    $(submapSumary).append($(totalString));
    $(submapSumary).append($(countString));
    $(submapSumary).append($(penString));
    $(submapDiv).append($(submapSumary));

}

function BindDMSummary(submapDiv, dm) {
    var dmSumary = $("<table  class=\"dmap\" cellspacing=\"0\" cellpadding=\"4\" style=\"width:2300px\"></table>");
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

function LoadSubMapItems(submapDiv, submap,smId) {
    //    var params = [];
    //    params.push('action=getsubmapareas');
    //    params.push('campaign=' + campaign.Id);
    //    params.push('submap=' + submap.Id);
    //    $.ajax({
    //        type: "GET",
    //        url: "Handler/submap.ashx",
    //        data: params.join('&'),
    //        dataType: 'json',
    //        success: function(data) {
    //            if (data) {
    //                BindSubMapItems(submapDiv, submap, data);
    //            }
    //        }
    //    });

    $(submapDiv).append('<p></p>');
    BindSubMapSummary(submapDiv, submap,smId);
    $(submapDiv).append('<div class="spaceline"></div>');
    BindSubMapMap(submapDiv, submap, submap);
    //BindSubMapColorLegend(submapDiv);
    $(submapDiv).append($("<div class=\"colorlegend\"></div>"));
    $(submapDiv).append('<div class="spaceline"></div>');
    BindSubMapFiveZips(submapDiv, submap, submap.FiveZipAreas,smId);
    $(submapDiv).append('<div class="spaceline"></div>');
    BindSubMapPremiumCRoutes(submapDiv, submap, submap.CRoutes,smId);
    $(submapDiv).append('<div class="spaceline"></div>');
    BindTracts(submapDiv, submap, submap.Tracts, smId);
    $(submapDiv).append('<div class="spaceline"></div>');
    BindBlockGroups(submapDiv, submap, submap.BlockGroups, smId);

}
function LoadDMItems(submapDiv, dm) {
    //    var params = [];
    //    params.push('action=getsubmapareas');
    //    params.push('campaign=' + campaign.Id);
    //    params.push('submap=' + submap.Id);
    //    $.ajax({
    //        type: "GET",
    //        url: "Handler/submap.ashx",
    //        data: params.join('&'),
    //        dataType: 'json',
    //        success: function(data) {
    //            if (data) {
    //                BindSubMapItems(submapDiv, submap, data);
    //            }
    //        }
    //    });

    //$(submapDiv).append('<p></p>');
    BindDMSummary(submapDiv, dm);
    //$(submapDiv).append('<div class="spaceline"></div>');
    BindDMMap(submapDiv, dm);
    //$(submapDiv).append('<div class="spaceline"></div>');
//    BindDMFiveZips(submapDiv, dm, dm.FiveZipAreas);
//    $(submapDiv).append('<div class="spaceline"></div>');
//    BindDMPremiumCRoutes(submapDiv, dm, dm.CRoutes);
//    $(submapDiv).append('<div class="spaceline"></div>');
//    BindTractsDM(submapDiv, dm, dm.Tracts);
//    $(submapDiv).append('<div class="spaceline"></div>');
//    BindBlockGroupsDM(submapDiv, dm, dm.BlockGroups);

}

//function BindSubMapItems(submapDiv, submap, areas) {
//    submap.Areas = areas;
//    $(submapDiv).append('<p></p>');
//    BindSubMapSummary(submapDiv, submap);
//    $(submapDiv).append('<div class="spaceline"></div>');
//    BindSubMapMap(submapDiv, submap, areas);
//    $(submapDiv).append('<div class="spaceline"></div>');
//    BindSubMapFiveZips(submapDiv, submap, areas.FiveZipAreas);
//    $(submapDiv).append('<div class="spaceline"></div>');
//    BindTracts(submapDiv, submap, areas.Tracts);
//    $(submapDiv).append('<div class="spaceline"></div>');
//    BindBlockGroups(submapDiv, submap, areas.BlockGroups);
//    if (lastSubMapId == submap.Id) {
//        submapsLoaded = true;
//        ValidateLoad();
//    }
//}

function BindSubMapMap(submapDiv, submap) {
    var id = "submapmap" + submap.Id;
    $(submapDiv).append($("<table cellspacing=\"0\" cellpadding=\"0\" class=\"maptable\"><caption>Map</caption><tbody><td><div id='" + id + "' class='submapmap'></div></td></tbody></table>"));
    var printmap = new PrintMap(id);
    printmap.LoadMap();
    printmap.SetAddresses(campaign.Addresses);

    submap.MapDivId = id;
    submap.MapObj = printmap._map;
    submap.PrintMap = printmap;
    printmap.AddSubMap(submap);
    printmap.AddSubMapAreas(submap);

    //    BindSubMapMapShapes(map, areas);
}

var currentPrintmap = null;
var currentDm = null;

function EndZoomHandler(e) {
    var z = currentPrintmap._map.GetZoomLevel();
    var divId = "ndArea_" + currentDm.Id;
    $(divId).html("");
}




function BindDMMap(submapDiv, dm) {


    var id = "submapmap" + dm.Id;
    $(submapDiv).append($("<table cellspacing=\"0\" cellpadding=\"0\" class=\"maptable\"><caption>" + campaign.CompositeName + " - " + dm.Name + "</caption><tbody><tr><td><div id='" + id + "' class='dmmap'></div></td></tr></tbody></table>"));
    var printmap = new PrintMap(id);
    printmap.LoadMap();
   
    printmap.SetAddresses(campaign.Addresses);
    dm.MapDivId = id;
    dm.MapObj = printmap._map;
    dm.PrintMap = printmap;
    //    printmap.AddDM(dm);
    //    printmap.AddDMAreas(dm);
    //    BindSubMapMapShapes(map, areas);

    //draw dm
    var sps = [];
    var i = 0;


    var jl = dm.DistributionMapCoordinates.length;
    if (jl == 0) {
        $(submapDiv).append("<div class='spaceline'></div><div class='spaceline'></div>");
        //ndListSTRS += "$" + dm.Id + "@";
        return;
    }

    //var s_minLat, s_maxLat, s_minLon, s_maxLon;
    while (i < jl) {        
        var currentLatitude = dm.DistributionMapCoordinates[i].Latitude;
        var currentLongitude = dm.DistributionMapCoordinates[i].Longitude;
        sps.push(new VELatLong(currentLatitude, currentLongitude));

//        if (i == 0) {
//            s_minLat = s_maxLat = currentLatitude;
//            s_minLon = s_maxLon = currentLongitude;
//        }
//        if (currentLatitude < s_minLat)
//            s_minLat = currentLatitude;
//        if (currentLatitude > s_maxLat)
//            s_maxLat = currentLatitude;
//        if (currentLongitude < s_minLon)
//            s_minLon = currentLongitude;
//        if (currentLongitude > s_maxLon)
//            s_maxLon = currentLongitude;
        
        i++;
    }


    dm._shape = new VEShape(VEShapeType.Polygon, sps);
    //dm._shape.SetFillColor(new VEColor(0, 0, 0, 0.1));
    dm._shape.SetFillColor(new VEColor(0, 255, 0, 0.2));
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

//    dm.PrintMap._map.ZoomIn();
    
//  var level = printmap._map.GetZoomLevel();
//  var maxLevel = dm._shape.GetMaxZoomLevel();

//    var lat = printmap._map.GetCenter().Latitude;
//    var lon = printmap._map.GetCenter().Longitude;


//    while (level < maxLevel) {
//        var current_minLat = printmap._map.GetMapView().BottomRightLatLong.Latitude;
//        var current_maxLat = printmap._map.GetMapView().TopLeftLatLong.Latitude;
//        var current_minLon = printmap._map.GetMapView().TopLeftLatLong.Longitude;
//        var current_maxLon = printmap._map.GetMapView().BottomRightLatLong.Longitude;
//        if (s_minLat > current_minLat && s_maxLat < current_maxLat && s_minLon > current_minLon && s_maxLon < current_maxLon) {
//            printmap._map.SetCenterAndZoom(new VELatLong(lat, lon), level);
//            level++;
//        }
//        else{
//            level--;
//            break;
//        }
//
//    }

    //dm center postition      
    var minLat = printmap._map.GetMapView().BottomRightLatLong.Latitude;
    var maxLat = printmap._map.GetMapView().TopLeftLatLong.Latitude;
    var minLon = printmap._map.GetMapView().TopLeftLatLong.Longitude;
    var maxLon = printmap._map.GetMapView().BottomRightLatLong.Longitude;

//    printmap._map.ZoomIn();
//    var current_minLat = printmap._map.GetMapView().BottomRightLatLong.Latitude;
//    var current_maxLat = printmap._map.GetMapView().TopLeftLatLong.Latitude;
//    var current_minLon = printmap._map.GetMapView().TopLeftLatLong.Longitude;
//    var current_maxLon = printmap._map.GetMapView().BottomRightLatLong.Longitude;
//    //if (s_minLat > current_minLat && s_maxLat < current_maxLat && s_minLon > current_minLon && s_maxLon < current_maxLon) {
//        minLat = current_minLat;
//        maxLat = current_maxLat;
//        minLon = current_minLon;
//        maxLon = current_maxLon;       
////    }
////    else {
////        printmap._map.ZoomOut();        
////    }    
        
    //get dm screen all boxes
    var boxes = [];
    boxes = _GetScreenBoxes(dm.PrintMap._map,minLat,maxLat,minLon,maxLon);
    
    //get nd custom areas and nd addresses by boxids 
    var service = new TIMM.Website.DistributionMapServices.DMReaderService();
    service.GetCustomAreaByBox(boxes, function(toAreaList) {

        var ndListStr = "<div id='ndArea_" + dm.Id  +"'><table style='width:2300px;border-top:3px solid black;border-bottom:3px solid black;border-left:3px solid black;border-right:3px solid black;magin-top:36px;magin-bottom:36px;height:140px;'>";

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
                //dm._shape.HideIcon();
                //dm._shape.SetCustomIcon('<div class="addressicon"></div>');
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
            ndListStr = "<div class='spaceline'></div><div align='center' style='width:2300px;font-weight: bold;font-size: 15px'>DO NOT DISTRIBUTE LIST"
            //+ dm.Name + "(MC#:" + campaign.CompositeName + ")" 
            + "</div><div class='spaceline'></div>" + ndListStr;
        }
        ndListStr += "</table><div class='spaceline'></div><div class='spaceline'></div></div>";
        ndListSTRS += "$" + dm.Id + "@";
        $(submapDiv).append(ndListStr);
        //dm.PrintMap._map.SetCenter(new VELatLong(lat, lon));
        //dm.PrintMap._map.SetCenterAndZoom(new VELatLong(lat, lon), level);

        //printmap._map.AttachEvent("onchangeview", EndZoomHandler);
        currentPrintmap = printmap;
        currentDm = dm;
        

    });

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


function OnPrintClick() {
//    setTimeout('GPS.Loading.show("Printing...");', 100);
//    GPS.Loading.show("Printing...");
    var campainStr = GetCampaignStr();
    $('#campaign').val(campainStr);
//    GPS.Loading.hide();
    return true;
}

function OnPrintClickDM() {
    //    setTimeout('GPS.Loading.show("Printing...");', 100);
    //    GPS.Loading.show("Printing...");
    var campainStr = GetCampaignStrDM();
    $('#campaign').val(campainStr);
    //$('#campaign').val(campaign.CompositeName);
    //    GPS.Loading.hide();
    return true;
}

function HoldCampaign() {
    if (campaign) {
        campaign.Description = $('#campaign-description').val();
    }
}

function GetCampaignStr() {
    var printmap = campaign.PrintMap;
    var items = [];
    items.push(campaign.CompositeName);
    items.push(campaign.ContactName);
    items.push(campaign.ClientName);
    items.push(campaign.UserFullName);
    items.push(campaign.Total);
    items.push(campaign.Count);
    items.push((campaign.Pen * 100).toFixed(2) + "%");
    items.push($.datepicker.formatDate("mm-dd-yy", new Date(campaign.Date)));
    items.push(printmap.GetMapImagesStr());
    items.push(printmap.GetMapShapesStr());
    items.push(printmap.GetMapPushpinsStr());
    items.push(printmap.GetLocationsStr());
    items.push(printmap.GetScaleStr());
    items.push(GetCampaignSubMapsStr(campaign.SubMaps));
    items.push(GetCampaignColorStr());
    items.push(campaign.Logo);
    return items.join('~');
}

function GetCampaignStrDM() {
    //var printmap = dm.PrintMap;
    var items = [];
    items.push(campaign.CompositeName);
    items.push(campaign.ContactName);
    items.push(campaign.ClientName);
    items.push(campaign.UserFullName);
    items.push(campaign.Total);
    items.push(campaign.Count);
    items.push((campaign.Pen * 100).toFixed(2) + "%");
    items.push($.datepicker.formatDate("mm-dd-yy", new Date(campaign.Date)));

//    var x = 0;
//    var y = 0;
//    var sublen = campaign.SubMaps.length;
//    var dislen = 0;
//    var printmap = null;
//    while (x < sublen) {
//        y = 0;
//        dislen = campaign.SubMaps[x].DistributionMaps.length;
//        while(y<dislen){
//            printmap = campaign.SubMaps[x].DistributionMaps[y];
//            items.push(printmap.GetMapImagesStr());
//            items.push(printmap.GetMapShapesStr());
//            items.push(printmap.GetMapPushpinsStr());
//            items.push(printmap.GetLocationsStr());
//            items.push(printmap.GetScaleStr());
//            
//            y++;
//        }
//        x++;
//    }
    items.push(GetCampaignDMsStr(campaign.SubMaps));
    items.push(GetCampaignColorStr());
    items.push(campaign.Logo);
    items.push(ndListSTRS);
    
    
    return items.join('~');
}

function GetCampaignColorStr() {
    if (PrintSettings.ShowPenetrationColors) {
        return PenetrationColorSettings.Serialize(PrintSettings.ChangePenetrationColor);
    }
    else { return ""; }

}

function GetCampaignSubMapsStr(submaps) {
    var items = [];
    if (PrintSettings.ShowSubMaps) {
        var i = 0;
        var length = submaps.length;
        while (i < length) {
            items.push(GetCampaignSubMapStr(submaps[i]));
            i++;
        }
    }
    return items.join('^');
}

function GetCampaignSubMapStr(submap) {
    var items = [];
    var pen = submap.Total > 0 ? submap.Penetration / submap.Total * 100 : 0;
    var printmap = submap.PrintMap;
    items.push(submap.Id);
    items.push(submap.Name);
    items.push(submap.Total);
    items.push(submap.Penetration);
    items.push(pen.toFixed(2) + '%');
    items.push(printmap.GetMapImagesStr());
    items.push(printmap.GetMapShapesStr());
    items.push(printmap.GetMapPushpinsStr());
    items.push(printmap.GetLocationsStr());
    items.push(printmap.GetScaleStr());
    items.push(GetAreasStr(submap.FiveZipAreas));
    items.push(GetAreasStr(submap.CRoutes));
    items.push(GetAreasStr(submap.Tracts));
    items.push(GetAreasStr(submap.BlockGroups));
    return items.join('*');
}

function GetCampaignDMsStr(submaps) {
    var items = [];
    if (PrintSettings.ShowSubMaps) {
        var i = 0;
        var length = submaps.length;
        while (i < length) {
            var dms = submaps[i].DistributionMaps;
            var j = 0;
            var len = dms.length;
            while (j < len) {
                items.push(GetCampaignDMStr(dms[j]));
                j++;
            }
            i++;
        }
    }
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

function OnPrintOptionClick(sendor, action) {
    var visible = !sendor.checked;
    if (action == 'coverpage') { ChangeShowCoverPage(!sendor.checked); }
    else if (action == 'location') { ChangeShowLocation(!sendor.checked); }
    else if (action == 'radii') { ChangeShowRadii(!sendor.checked); }
    else if (action == 'nondeliverables') { ChangeShowNonDeliverables(!sendor.checked); }
    else if (action == 'submaps') { ChangeShowSubMaps(!sendor.checked); }
    else if (action == 'submapcount') { ChangeShowSubMapCount(!sendor.checked); }
    else if (action == 'classificationoutlines') { ChangeShowClassificationOutLines(!sendor.checked); }
    else if (action == 'penetrationcolor') { ChangeShowPenetrationColor(sendor.checked); }
    else if (action == 'changepenetrationcolor') { ChangePenetrationColor(sendor.checked); }
}

function OnPrintOptionClickDM(sendor, action) {
    var visible = !sendor.checked;
//    if (action == 'coverpage') { ChangeShowCoverPage(!sendor.checked); }
//    else if (action == 'location') { ChangeShowLocationDM(!sendor.checked); }
    if (action == 'location') { ChangeShowLocationDM(!sendor.checked); }
    else if (action == 'radii') { ChangeShowRadiiDM(!sendor.checked); }
    else if (action == 'nondeliverables') { ChangeShowNonDeliverablesDM(!sendor.checked); }
    else if (action == 'dms') { ChangeShowSubMaps(!sendor.checked); }
    else if (action == 'dmcount') { ChangeShowDMapCount(!sendor.checked); }
//    else if (action == 'classificationoutlines') { ChangeShowClassificationOutLinesDM(!sendor.checked); }
//    else if (action == 'penetrationcolor') { ChangeShowPenetrationColorDM(sendor.checked); }
    else if (action == 'changepenetrationcolor') { ChangePenetrationColorDM(sendor.checked); }
}

function ChangeShowCoverPage(isShow) {
    if (isShow) {
        $("#cover-page").show();
    }
    else {
        $("#cover-page").hide();
    }
    $('#showcoverpage').val(isShow);
}

function ChangeShowLocation(isShow) {
    if (isShow) { campaign.PrintMap.ShowLocations(); }
    else { campaign.PrintMap.HideLocations(); }
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        if (isShow) { submaps[i].PrintMap.ShowLocations(); }
        else { submaps[i].PrintMap.HideLocations(); }
        i++;
    }
    PrintSettings.ShowLocation = isShow;
}

function ChangeShowLocationDM(isShow) {
//    if (isShow) { campaign.PrintMap.ShowLocations(); }
//    else { campaign.PrintMap.HideLocations(); }
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        var dms = submaps[i].DistributionMaps;
        var j = 0;
        var len = dms.length;
        while (j < len) {
            if (isShow) { dms[j].PrintMap.ShowLocations(); }
            else { dms[j].PrintMap.HideLocations(); }
            j++;
        }
//        if (isShow) { submaps[i].PrintMap.ShowLocations(); }
//        else { submaps[i].PrintMap.HideLocations(); }
        i++;
    }
    PrintSettings.ShowLocation = isShow;
}

function ChangeShowRadii(isShow) {
    if (isShow) { campaign.PrintMap.ShowRadii(); }
    else { campaign.PrintMap.HideRadii(); }
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        if (isShow) { submaps[i].PrintMap.ShowRadii(); }
        else { submaps[i].PrintMap.HideRadii(); }
        i++;
    }
    PrintSettings.ShowRadii = isShow;
}

function ChangeShowRadiiDM(isShow) {
    //    if (isShow) { campaign.PrintMap.ShowRadii(); }
    //    else { campaign.PrintMap.HideRadii(); }
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
//    while (i < length) {
//        if (isShow) { submaps[i].PrintMap.ShowRadii(); }
//        else { submaps[i].PrintMap.HideRadii(); }
//        i++;
//    }
    while (i < length) {
        var dms = submaps[i].DistributionMaps;
        var j = 0;
        var len = dms.length;
        while (j < len) {
            if (isShow) { dms[j].PrintMap.ShowRadii(); }
            else { dms[j].PrintMap.HideRadii(); }
            j++;
        }
        i++;
    }
    PrintSettings.ShowRadii = isShow;
}

function ChangeShowNonDeliverables(isShow) {
    if (isShow) { campaign.PrintMap.ShowRadii(); }
    else { campaign.PrintMap.HideRadii(); }
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        if (isShow) { submaps[i].PrintMap.ShowNonDeliverables(); }
        else { submaps[i].PrintMap.HideNonDeliverables(); }
        i++;
    }
    PrintSettings.ShowNonDeliverables = isShow;
}

function ChangeShowNonDeliverablesDM(isShow) {
    //    if (isShow) { campaign.PrintMap.ShowRadii(); }
    //    else { campaign.PrintMap.HideRadii(); }
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        var dms = submaps[i].DistributionMaps;
        var j = 0;
        var len = dms.length;
        while (j < len) {
            if (isShow) { dms[j].PrintMap.ShowNonDeliverables(); }
            else { dms[j].PrintMap.HideNonDeliverables(); }
            j++;
        }
        i++;
    }
//    while (i < length) {
//        if (isShow) { submaps[i].PrintMap.ShowNonDeliverables(); }
//        else { submaps[i].PrintMap.HideNonDeliverables(); }
//        i++;
//    }
    PrintSettings.ShowNonDeliverables = isShow;
}

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

function ChangeShowDMapCount(isShow) {
    if (isShow) { $('#print-submaps-content').find('.dmap').show(); }
    else { $('#print-submaps-content').find('.dmap').hide(); }
    PrintSettings.ShowSubMapCount = isShow;
}

function ChangeShowClassificationOutLines(isShow) {
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        if (isShow) { submaps[i].PrintMap.ShowClassificationOutLines(); }
        else { submaps[i].PrintMap.HideClassificationOutLines(); }
        i++;
    }
    PrintSettings.ShowClassificationOutlines = isShow;
}

function ChangeShowClassificationOutLinesDM(isShow) {
    var submaps = campaign.SubMaps;
    var i = 0;
    var length = submaps.length;
    while (i < length) {
        var len = submaps[i].DistributionMaps.length;
        var j = 0;
        while (j < len) {
            if (isShow) { submaps[i].DistributionMaps[j].PrintMap.ShowClassificationOutLines(); }
            else { submaps[i].DistributionMaps[j].PrintMap.HideClassificationOutLines(); }
            j++;
        }
        i++;
    }
    PrintSettings.ShowClassificationOutlines = isShow;
}

function ChangeShowPenetrationColor(isShow) {
    var submaps = campaign.SubMaps;
    var j = 0;
    var jlength = submaps.length;
    while (j < jlength) {
        var fiveZips = submaps[j].FiveZipAreas;
        var croutes = submaps[j].CRoutes;
        var trks = submaps[j].Tracts;
        var bgs = submaps[j].BlockGroups;
        var i = 0;
        var length = fiveZips.length;
        while (i < length) {
            if (fiveZips[i].IsEnabled) {
                var shape = fiveZips[i].ShapeObj;
                var color;
                if (isShow) {
                    color = PenetrationColorSettings.GetColor(fiveZips[i].Count / fiveZips[i].Total, PrintSettings.ChangePenetrationColor); 
                 }
                else { color = PenetrationColorSettings.GetColor(-1); }
                shape.SetFillColor(new VEColor(color.R, color.G, color.B, color.A));
            }
            i++;
        }

        //Croutes
        i = 0;
        length = croutes.length;
        while (i < length) {
            if (croutes[i].IsEnabled) {
                var shape = croutes[i].ShapeObj;
                var color;
                if (isShow) {                   
                    color = PenetrationColorSettings.GetColor(croutes[i].Count / croutes[i].Total, PrintSettings.ChangePenetrationColor); 
                }
                else { color = PenetrationColorSettings.GetColor(-1); }
                shape.SetFillColor(new VEColor(color.R, color.G, color.B, color.A));
            }
            i++;
        }

        //Tract
        i = 0;
        length = trks.length;
        while (i < length) {
            if (trks[i].IsEnabled) {
                var shape = trks[i].ShapeObj;
                var color;
                if (isShow) { color = PenetrationColorSettings.GetColor(trks[i].Count / trks[i].Total, PrintSettings.ChangePenetrationColor); }
                else { color = PenetrationColorSettings.GetColor(-1); }
                shape.SetFillColor(new VEColor(color.R, color.G, color.B, color.A));
            }
            i++;
        }

        //BG
        i = 0;
        length = bgs.length;
        while (i < length) {
            if (bgs[i].IsEnabled) {
                var shape = bgs[i].ShapeObj;
                var color;
                if (isShow) { color = PenetrationColorSettings.GetColor(bgs[i].Count / bgs[i].Total, PrintSettings.ChangePenetrationColor); }
                else { color = PenetrationColorSettings.GetColor(-1); }
                shape.SetFillColor(new VEColor(color.R, color.G, color.B, color.A));
            }
            i++;
        }

        var submapColor;
        if (isShow) { submapColor = PenetrationColorSettings.GetColor(submaps[j].Penetration / submaps[j].Total, PrintSettings.ChangePenetrationColor); }
        else { submapColor = PenetrationColorSettings.GetColor(-1); }
        if (submaps[j].CampaignShapeObj) {
            submaps[j].CampaignShapeObj.SetFillColor(new VEColor(submapColor.R, submapColor.G, submapColor.B, submapColor.A));
        }
//        submaps[j].ShapeObj.SetFillColor(new VEColor(submapColor.R, submapColor.G, submapColor.B, submapColor.A));
        j++;
    }
    PrintSettings.ShowPenetrationColors = isShow;
}

function ChangeShowPenetrationColorDM(isShow) {
    var submaps = campaign.SubMaps;
    var k = 0;
    var klen = submaps.length;
    while(k<klen){
        var j = 0;
        var jlength = submaps[k].DistributionMaps.length;
        
        while (j < jlength) {
            var fiveZips = submaps[k].DistributionMaps[j].FiveZipAreas;
            var croutes = submaps[k].DistributionMaps[j].CRoutes;
            var trks = submaps[k].DistributionMaps[j].Tracts;
            var bgs = submaps[k].DistributionMaps[j].BlockGroups;
            var i = 0;
            var length = fiveZips.length;
            while (i < length) {
                if (fiveZips[i].IsEnabled) {
                    var shape = fiveZips[i].ShapeObj;
                    if(shape!=undefined){
                        var color;
                        if (isShow) { color = PenetrationColorSettings.GetColor(fiveZips[i].Count / fiveZips[i].Total, PrintSettings.ChangePenetrationColor); }
                        else { color = PenetrationColorSettings.GetColor(-1); }
                        shape.SetFillColor(new VEColor(color.R, color.G, color.B, color.A));
                    }
                }
                i++;
            }

            //Croutes
            i = 0;
            length = croutes.length;
            while (i < length) {
                if (croutes[i].IsEnabled) {
                    var shape = croutes[i].ShapeObj;
                    if (shape != undefined) {
                        var color;
                        if (isShow) { color = PenetrationColorSettings.GetColor(croutes[i].Count / croutes[i].Total, PrintSettings.ChangePenetrationColor); }
                        else { color = PenetrationColorSettings.GetColor(-1); }
                        shape.SetFillColor(new VEColor(color.R, color.G, color.B, color.A));
                    }
                }
                i++;
            }

            //Tract
            i = 0;
            length = trks.length;
            while (i < length) {
                if (trks[i].IsEnabled) {
                    var shape = trks[i].ShapeObj;
                    if (shape != undefined) {
                        var color;
                        if (isShow) { color = PenetrationColorSettings.GetColor(trks[i].Count / trks[i].Total, PrintSettings.ChangePenetrationColor); }
                        else { color = PenetrationColorSettings.GetColor(-1); }
                        shape.SetFillColor(new VEColor(color.R, color.G, color.B, color.A));
                    }
                }
                i++;
            }

            //BG
            i = 0;
            length = bgs.length;
            while (i < length) {
                if (bgs[i].IsEnabled) {
                    var shape = bgs[i].ShapeObj;
                    if (shape != undefined) {
                        var color;
                        if (isShow) { color = PenetrationColorSettings.GetColor(bgs[i].Count / bgs[i].Total, PrintSettings.ChangePenetrationColor); }
                        else { color = PenetrationColorSettings.GetColor(-1); }
                        shape.SetFillColor(new VEColor(color.R, color.G, color.B, color.A));
                    }
                }
                i++;
            }

            var submapColor;
            if (isShow) { submapColor = PenetrationColorSettings.GetColor(submaps[k].DistributionMaps[j].Penetration / submaps[k].DistributionMaps[j].Total, PrintSettings.ChangePenetrationColor); }
            else { submapColor = PenetrationColorSettings.GetColor(-1); }
            submaps[k].DistributionMaps[j]._shape.SetFillColor(new VEColor(submapColor.R, submapColor.G, submapColor.B, submapColor.A));
            submaps[k].DistributionMaps[j].ShapeObj.SetFillColor(new VEColor(submapColor.R, submapColor.G, submapColor.B, submapColor.A));
            j++;
        }
        k++;
    }
    PrintSettings.ShowPenetrationColors = isShow;
}

function ChangePenetrationColor(isChange) {
    //    $('#print-color-td2').css('');
    PrintSettings.ChangePenetrationColor = isChange;
    if (isChange) {
        $('#print-new-minimum-criteria').show();
    }
    else {
        $('#print-new-minimum-criteria').hide();

    }

    if (PrintSettings.ShowPenetrationColors) {
        ChangeShowPenetrationColor(true);
    }
}

function ChangePenetrationColorDM(isChange) {
    //    $('#print-color-td2').css('');
    PrintSettings.ChangePenetrationColor = isChange;
    if (isChange) {
        $('#print-new-minimum-criteria').show();
    }
    else {
        $('#print-new-minimum-criteria').hide();

    }

    if (PrintSettings.ShowPenetrationColors) {
        ChangeShowPenetrationColorDM(true);
    }
}

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
        //        if (point > prevPoint * 100 && point < nextPoint * 100) {
        if ((point > prevPoint * 100 || (prevPoint * 100 <= 0 && point == 0)) && nextPoint > 0) {
            PenetrationColorSettings.SetColorPoint(index, point / 100, true);
            valid = true;
        }
        //        }
        //        else { GPSAlert('Require > ' + prevPoint * 100 + ' and < ' + nextPoint * 100); }
    }
    //    else { GPSAlert('Require muber'); }
    if (!valid) { $(obj).val(GetNumber(PenetrationColorSettings.GetColorPoints(true)[index - 1])); }
    return valid;
}

function updateColorLegend() {
    BindColorLegend();
}


