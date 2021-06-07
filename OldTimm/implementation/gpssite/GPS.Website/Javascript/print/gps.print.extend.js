


function BindSubMapFiveZips(submapDiv, submap, areas, submapId) {
    submap.AreasOrderId = 1;
    var i = 0;
    var length = areas.length;
    if (length > 0) {
        var submapItem = $("<table  class=\"submapitem\" cellspacing=\"0\" cellpadding=\"4\"></table>");
        $(submapItem).append($("<caption>ZIP CODES AREAS CONTAINED IN SUB MAP {0} ({1})</caption>".replace("{0}", submapId).replace("{1}", submap.Name)));
        $(submapItem).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"toplabel\" style=\"width: 10%;\">#</td><td class=\"toplabel\" style=\"width: 30%;\">ZIP CODE</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
        while (i < length) {
            if (areas[i].IsEnabled) {
//                areas[i].Code = areas[i].Name;

                if (areas[i].Total == null) { areas[i].Total = 0; }
                if (areas[i].Count == null) { areas[i].Count = 0; }
                var pen = areas[i].Total == 0 ? 0 : areas[i].Count / areas[i].Total * 100;
                areas[i].Percentage = pen.toFixed(2) + "%";

                var row;
                if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
                else { row = $("<tr></tr>"); }

                row.append($("<td>{0}</td>".replace("{0}", submap.AreasOrderId)));
                row.append($("<td>{0}</td>".replace("{0}", areas[i].Code)));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Total))));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Count))));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", areas[i].Percentage)));

                $(submapItem).append($(row));
                areas[i].DisplayOrderId = submap.AreasOrderId;
                submap.AreasOrderId++;
            }
            i++;
        }
        $(submapDiv).append($(submapItem));
     }

//    var i = 0;
//    var length = areas.length;
//    var nareas = [];
//    if (length > 0) {
//        for (k = 0; k < length; k++) {
//            areas[k].Percentage = "";
//            if (areas[k].IsEnabled) {
//                if (areas[k].Total == null) { areas[k].Total = 0; }
//                if (areas[k].Count == null) { areas[k].Count = 0; }
//         
//            }
//        }
//        var temp1 = areas[0];
//        temp1.DisplayOrderId = submap.AreasOrderId;
//        temp1.DisplayOrderId = "1";
//        if (length == 1) 
//            nareas.push(temp1);
//        else {
//            var j = 1;
//            while (j < length) {
//                submap.AreasOrderId++;
//                temp2 = areas[j];
//                temp2.DisplayOrderId = submap.AreasOrderId;
//                if (temp1.Code == temp2.Code && temp1.PartPercentage == 1 && temp2.PartPercentage == 1) {
//                    if (j == length - 1) {
//                        temp1.DisplayOrderId = temp1.DisplayOrderId + " - " + submap.AreasOrderId;
//                        nareas.push(temp1);
//                    }
//                }
//                else {
//                    if (temp1.DisplayOrderId < submap.AreasOrderId - 1) {
//                        temp1.DisplayOrderId = temp1.DisplayOrderId + " - " + (submap.AreasOrderId - 1);
//                    }
//                    nareas.push(temp1);
//                    temp1 = temp2;
//                    if (j == length - 1) {
//                        nareas.push(temp1);
//                    }
//                }
//                
//                j++;
//            }
//        }
//    }
//    var len = nareas.length;
//    if (len > 0) {
//        var submapItem = $("<table  class=\"submapitem\" cellspacing=\"0\" cellpadding=\"4\"></table>");
//        $(submapItem).append($("<caption>ZIP CODES AREAS CONTAINED IN SUB MAP {0} ({1})</caption>".replace("{0}", submapId).replace("{1}", submap.Name)));
//        $(submapItem).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"toplabel\" style=\"width: 10%;\">#</td><td class=\"toplabel\" style=\"width: 30%;\">CARRIER ROUTE #</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
//        while (i < len) {
//            var pen = nareas[i].Total == 0 ? 0 : nareas[i].Count / nareas[i].Total * 100;
//            //if (areas[k].PartPercentage == 0) areas[k].PartPercentage = 100;
//            nareas[i].Percentage = pen.toFixed(2) + "%";
//            var row;
//            if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
//            else { row = $("<tr></tr>"); }
//            if (nareas[i].PartPercentage == 0) nareas[i].PartPercentage = 100;
//            row.append($("<td>{0}</td>".replace("{0}", nareas[i].DisplayOrderId)));
//            row.append($("<td>{0}</td>".replace("{0}", nareas[i].Code)));
//            row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(nareas[i].Total))));
//            row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(nareas[i].Count))));
//            row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", nareas[i].Percentage)));

//            $(submapItem).append($(row));
//            //                nareas[i].DisplayOrderId = submap.nareasOrderId;
//            //                submap.AreasOrderId++;
//            i++;
//        }
//        $(submapDiv).append($(submapItem));
//    }
}

//function BindDMFiveZips(submapDiv, dm, areas) {
//    dm.AreasOrderId = 1;
//    var i = 0;
//    var length = areas.length;
//    if (length > 0) {
//        var dmItem = $("<table  class=\"submapitem\" cellspacing=\"0\" cellpadding=\"4\"></table>");
//        $(dmItem).append($("<caption>ZIP CODES AREAS CONTAINED IN SUB MAP {0} ({1})</caption>".replace("{0}", dm.Id).replace("{1}", dm.Name)));
//        //$(dmItem).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"toplabel\" style=\"width: 10%;\">#</td><td class=\"toplabel\" style=\"width: 30%;\">ZIP CODE</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
//        while (i < length) {
//            if (areas[i].IsEnabled) {
//                //                areas[i].Code = areas[i].Name;

////                if (areas[i].Total == null) { areas[i].Total = 0; }
////                if (areas[i].Count == null) { areas[i].Count = 0; }
////                var pen = areas[i].Total == 0 ? 0 : areas[i].Count / areas[i].Total * 100;
////                areas[i].Percentage = pen.toFixed(2) + "%";

//                var row;
//                if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
//                else { row = $("<tr></tr>"); }

//                row.append($("<td>{0}</td>".replace("{0}", dm.AreasOrderId)));
//                row.append($("<td>{0}</td>".replace("{0}", areas[i].Code)));
////                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Total))));
////                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Count))));
////                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", areas[i].Percentage)));

//                $(dmItem).append($(row));
//                areas[i].DisplayOrderId = dm.AreasOrderId;
//                dm.AreasOrderId++;
//            }
//            i++;
//        }
//        $(submapDiv).append($(dmItem));
//    }
//}

//function BindSubMapPremiumCRoutes(submapDiv, submap, areas, submapId) {
//    //areas.sort(function(a, b) { return a.Code - b.Code; });
//    submap.AreasOrderId = 2;
//    var i = 0;
//    var length = areas.length;
//    var nareas = [];
//    if (length > 0) {
//        for (k = 0; k < length; k++) {
//            areas[k].Percentage = "";
//            if (areas[k].IsEnabled) {
//                if (areas[k].Total == null) { areas[k].Total = 0; }
//                if (areas[k].Count == null) { areas[k].Count = 0; }
////                submap.CRoutes[k].Total = (areas[k].Total * areas[k].PartPercentage / 100).toFixed(0);
////                submap.CRoutes[k].Count = (areas[k].Count * areas[k].PartPercentage / 100).toFixed(0);
//            }
//        }
//        var temp1 = areas[0];
//        temp1.DisplayOrderId = submap.AreasOrderId;
//        temp1.DisplayOrderId = "1";
//        if (length == 1) nareas.push(temp1);
//        else {
//            var j = 1;
//            while (j < length) {
//                
//                temp2 = areas[j];
//                temp2.DisplayOrderId = submap.AreasOrderId;
//                if (temp1.Code == temp2.Code && temp1.PartPercentage==100 && temp2.PartPercentage==100) {
//                    if (j == length - 1) {
//                        temp1.DisplayOrderId = temp1.DisplayOrderId + " - " + submap.AreasOrderId;
//                        nareas.push(temp1);
//                    }
//                }
//                else {
//                    if (temp1.DisplayOrderId < submap.AreasOrderId - 1) {
//                        temp1.DisplayOrderId = temp1.DisplayOrderId + " - " + (submap.AreasOrderId - 1);
//                    }
//                    nareas.push(temp1);
//                    temp1 = temp2;
//                    if (j == length - 1) {
//                        nareas.push(temp1);
//                    }
//                }
//                submap.AreasOrderId++;
//                j++;
//            }
//        }
//    }
//    var len = nareas.length;
//    if (len > 0) {
//        var submapItem = $("<table  class=\"submapitem\" cellspacing=\"0\" cellpadding=\"4\"></table>");
//        $(submapItem).append($("<caption>CARRIER ROUTES CONTAINED IN SUB MAP {0} ({1})</caption>".replace("{0}", submapId).replace("{1}", submap.Name)));
//        $(submapItem).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"toplabel\" style=\"width: 10%;\">#</td><td class=\"toplabel\" style=\"width: 30%;\">CARRIER ROUTE #</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
//        while (i < len) {
//                var pen = nareas[i].Total == 0 ? 0 : nareas[i].Count / nareas[i].Total * 100;
//                //if (areas[k].PartPercentage == 0) areas[k].PartPercentage = 100;
//                nareas[i].Percentage = pen.toFixed(2) + "%";
//                var row;
//                if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
//                else { row = $("<tr></tr>"); }
//                if (nareas[i].PartPercentage == 0) nareas[i].PartPercentage = 100;
//                row.append($("<td>{0}</td>".replace("{0}", nareas[i].DisplayOrderId)));
//                row.append($("<td>{0}</td>".replace("{0}", nareas[i].Code)));
//                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(nareas[i].Total))));
//                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(nareas[i].Count))));
//                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", nareas[i].Percentage)));

//                $(submapItem).append($(row));
////                nareas[i].DisplayOrderId = submap.nareasOrderId;
////                submap.AreasOrderId++;
//            i++;
//        }
//        $(submapDiv).append($(submapItem));
//    }
//}


function BindSubMapPremiumCRoutes(submapDiv, submap, areas, submapId) {
    submap.AreasOrderId = 1;
    var i = 0;
    var length = areas.length;
    if (length > 0) {
        var submapItem = $("<table  class=\"submapitem\" cellspacing=\"0\" cellpadding=\"4\"></table>");
        $(submapItem).append($("<caption>CARRIER ROUTES CONTAINED IN SUB MAP {0} ({1})</caption>".replace("{0}", submapId).replace("{1}", submap.Name)));
        $(submapItem).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"toplabel\" style=\"width: 10%;\">#</td><td class=\"toplabel\" style=\"width: 30%;\">CARRIER ROUTE #</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
        while (i < length) {
            if (areas[i].IsEnabled) {
                //                areas[i].Code = areas[i].Name;

                if (areas[i].Total == null) { areas[i].Total = 0; }
                if (areas[i].Count == null) { areas[i].Count = 0; }
                var pen = areas[i].Total == 0 ? 0 : areas[i].Count / areas[i].Total * 100;
                areas[i].Percentage = pen.toFixed(2) + "%";

                var row;
                if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
                else { row = $("<tr></tr>"); }

                row.append($("<td>{0}</td>".replace("{0}", submap.AreasOrderId)));
                row.append($("<td>{0}</td>".replace("{0}", areas[i].Code)));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Total))));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Count))));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", areas[i].Percentage)));

                $(submapItem).append($(row));
                areas[i].DisplayOrderId = submap.AreasOrderId;
                submap.AreasOrderId++;
            }
            i++;
        }
        $(submapDiv).append($(submapItem));
    }
}


function BindTracts(submapDiv, submap, areas, submapId) {
    var i = 0;
    var length = areas.length;
    if (length > 0) {
        var submapItem = $("<table  class=\"submapitem\" cellspacing=\"0\" cellpadding=\"4\"></table>");
        $(submapItem).append($("<caption>CENSUS TRACTS CONTAINED IN SUB MAP {0} ({1})</caption>".replace("{0}", submapId).replace("{1}", submap.Name)));
        $(submapItem).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"toplabel\" style=\"width: 10%;\">#</td><td class=\"toplabel\" style=\"width: 30%;\">CENSUS TRACT #</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
        while (i < length) {
            if (areas[i].IsEnabled) {
//                areas[i].Code = PadLeft(areas[i].Attributes.State, 2, '0')
//            + PadLeft(areas[i].County, 3, '0')
//            + PadLeft(areas[i].Attributes.Tract, 6, '0');

                if (areas[i].Total == null) { areas[i].Total = 0; }
                if (areas[i].Count == null) { areas[i].Count = 0; }
                var pen = areas[i].Total == 0 ? 0 : areas[i].Count / areas[i].Total * 100;
                areas[i].Percentage = pen.toFixed(2) + "%";

                var row;
                if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
                else { row = $("<tr></tr>"); }
                row.append($("<td>{0}</td>".replace("{0}", submap.AreasOrderId)));
                row.append($("<td>{0}</td>".replace("{0}", areas[i].Code)));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Total))));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Count))));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", areas[i].Percentage)));

                $(submapItem).append($(row));
                areas[i].DisplayOrderId = submap.AreasOrderId;
                submap.AreasOrderId++;
            }
            i++;
        }
        $(submapDiv).append($(submapItem));
    }
}

function BindTractsDM(submapDiv, dm, areas, submapId) {
    var i = 0;
    var length = areas.length;
    if (length > 0) {
        var dmItem = $("<table  class=\"submapitem\" cellspacing=\"0\" cellpadding=\"4\"></table>");
        $(dmItem).append($("<caption>CENSUS TRACTS CONTAINED IN SUB MAP {0} ({1})</caption>".replace("{0}", dm.Id).replace("{1}", dm.Name)));
        //$(dmItem).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"toplabel\" style=\"width: 10%;\">#</td><td class=\"toplabel\" style=\"width: 30%;\">TRACT #</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
        while (i < length) {
            if (areas[i].IsEnabled) {
                //                areas[i].Code = PadLeft(areas[i].Attributes.State, 2, '0')
                //            + PadLeft(areas[i].County, 3, '0')
                //            + PadLeft(areas[i].Attributes.Tract, 6, '0');

//                if (areas[i].Total == null) { areas[i].Total = 0; }
//                if (areas[i].Count == null) { areas[i].Count = 0; }
//                var pen = areas[i].Total == 0 ? 0 : areas[i].Count / areas[i].Total * 100;
//                areas[i].Percentage = pen.toFixed(2) + "%";

                var row;
                if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
                else { row = $("<tr></tr>"); }
                row.append($("<td>{0}</td>".replace("{0}", dm.AreasOrderId)));
                row.append($("<td>{0}</td>".replace("{0}", areas[i].Code)));
//                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Total))));
//                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Count))));
//                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", areas[i].Percentage)));

                $(dmItem).append($(row));
                areas[i].DisplayOrderId = dm.AreasOrderId;
                dm.AreasOrderId++;
            }
            i++;
        }
        $(submapDiv).append($(dmItem));
    }
}

//function BindBlockGroups(submapDiv, dm, areas) {
//    var i = 0;
//    var length = areas.length;
//    if (length > 0) {
//        var dmItem = $("<table  class=\"submapitem\" cellspacing=\"0\" cellpadding=\"4\"></table>");
//        $(dmItem).append($("<caption>BLOCK GROUPS CONTAINED IN SUB MAP {0} ({1})</caption>".replace("{0}", dm.Id).replace("{1}", dm.Name)));
//        //$(submapItem).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"toplabel\" style=\"width: 10%;\">#</td><td class=\"toplabel\" style=\"width: 30%;\">BLOCK GROUP #</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
//        while (i < length) {
//            if (areas[i].IsEnabled) {
////                areas[i].Code = PadLeft(areas[i].Attributes.State, 2, '0')
////            + PadLeft(areas[i].County, 3, '0')
////            + PadLeft(areas[i].Attributes.Tract, 6, '0')
////            + areas[i].Attributes.BlockGroup;

////                if (areas[i].Total == null) { areas[i].Total = 0; }
////                if (areas[i].Count == null) { areas[i].Count = 0; }
////                var pen = areas[i].Total == 0 ? 0 : areas[i].Count / areas[i].Total * 100;
////                areas[i].Percentage = pen.toFixed(2) + "%";

//                var row;
//                if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
//                else { row = $("<tr></tr>"); }
//                row.append($("<td>{0}</td>".replace("{0}", dm.AreasOrderId)));
//                row.append($("<td>{0}</td>".replace("{0}", areas[i].Code)));
////                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Total))));
////                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Count))));
////                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", areas[i].Percentage)));

//                $(dmItem).append($(row));
//                areas[i].DisplayOrderId = dm.AreasOrderId;
//                dm.AreasOrderId++;
//            }
//            i++;
//        }
//        $(submapDiv).append($(dmItem));
//    }
//}
function BindBlockGroups(submapDiv, submap, areas, submapId) {
    var i = 0;
    var length = areas.length;
    if (length > 0) {
        var submapItem = $("<table  class=\"submapitem\" cellspacing=\"0\" cellpadding=\"4\"></table>");
        $(submapItem).append($("<caption>BLOCK GROUPS CONTAINED IN SUB MAP {0} ({1})</caption>".replace("{0}", submapId).replace("{1}", submap.Name)));
        $(submapItem).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"toplabel\" style=\"width: 10%;\">#</td><td class=\"toplabel\" style=\"width: 30%;\">BLOCK GROUP #</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
        while (i < length) {
            if (areas[i].IsEnabled) {
                //                areas[i].Code = PadLeft(areas[i].Attributes.State, 2, '0')
                //            + PadLeft(areas[i].County, 3, '0')
                //            + PadLeft(areas[i].Attributes.Tract, 6, '0')
                //            + areas[i].Attributes.BlockGroup;

                if (areas[i].Total == null) { areas[i].Total = 0; }
                if (areas[i].Count == null) { areas[i].Count = 0; }
                var pen = areas[i].Total == 0 ? 0 : areas[i].Count / areas[i].Total * 100;
                areas[i].Percentage = pen.toFixed(2) + "%";

                var row;
                if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
                else { row = $("<tr></tr>"); }
                row.append($("<td>{0}</td>".replace("{0}", submap.AreasOrderId)));
                row.append($("<td>{0}</td>".replace("{0}", areas[i].Code)));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Total))));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Count))));
                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", areas[i].Percentage)));

                $(submapItem).append($(row));
                areas[i].DisplayOrderId = submap.AreasOrderId;
                submap.AreasOrderId++;
            }
            i++;
        }
        $(submapDiv).append($(submapItem));
    }
}

function BindBlockGroupsDM(submapDiv, dm, areas) {
    var i = 0;
    var length = areas.length;
    if (length > 0) {
        var dmItem = $("<table  class=\"submapitem\" cellspacing=\"0\" cellpadding=\"4\"></table>");
        $(dmItem).append($("<caption>BLOCK GROUPS CONTAINED IN SUB MAP {0} ({1})</caption>".replace("{0}", dm.Id).replace("{1}", dm.Name)));
        //$(dmItem).append($("<tr style=\"background-color:#eeeeee;\"><td class=\"toplabel\" style=\"width: 10%;\">#</td><td class=\"toplabel\" style=\"width: 30%;\">BLOCK GROUP #</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TOTAL H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">TARGET H/H</td><td class=\"toplabel\" style=\"width: 20%; text-align: right;\">PENETRATION</td></tr>"));
        while (i < length) {
            if (areas[i].IsEnabled) {
                //                areas[i].Code = PadLeft(areas[i].Attributes.State, 2, '0')
                //            + PadLeft(areas[i].County, 3, '0')
                //            + PadLeft(areas[i].Attributes.Tract, 6, '0')
                //            + areas[i].Attributes.BlockGroup;

//                if (areas[i].Total == null) { areas[i].Total = 0; }
//                if (areas[i].Count == null) { areas[i].Count = 0; }
//                var pen = areas[i].Total == 0 ? 0 : areas[i].Count / areas[i].Total * 100;
//                areas[i].Percentage = pen.toFixed(2) + "%";

                var row;
                if (i % 2) { row = $("<tr style=\"background-color:#eeeeee;\"></tr>"); }
                else { row = $("<tr></tr>"); }
                row.append($("<td>{0}</td>".replace("{0}", dm.AreasOrderId)));
                row.append($("<td>{0}</td>".replace("{0}", areas[i].Code)));
//                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Total))));
//                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", Number.addCommas(areas[i].Count))));
//                row.append($("<td style=\"text-align: right;\">{0}</td>".replace("{0}", areas[i].Percentage)));

                $(dmItem).append($(row));
                areas[i].DisplayOrderId = dm.AreasOrderId;
                dm.AreasOrderId++;
            }
            i++;
        }
        $(submapDiv).append($(dmItem));
    }
}
