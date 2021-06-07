// Name space GPS.Nd, containing classes specific to non-deliverables.
GPS.Nd = function() { }

//document ready functions
$(document).ready(function() {
    GPS.Container.Register({
        Id: "ndmap.arealayer",
        IsSingleton: false,
        ClassObject: GPS.Nd.MapPanel.AreaLayer,
        Instance: null
    });
    GPS.Container.Register({
        Id: "ndarealayer.area",
        IsSingleton: false,
        ClassObject: GPS.Nd.MapPanel.Area,
        Instance: null
    });
    initLayout();
    initMenu();
    initMapPanel();
    initMapDraw();
    initClassificationColors();
});
// init
function initClassificationColors() {
    var styleFormat = "background-color:#{0};border-style:Solid;border-width:1px;border-color:#{1};display: inline-block; height: 4px;";
    var settings = GPS.ClsSettings;
    $('#lb_Z3_color').attr("style", styleFormat.replace("{0}", settings[0].FillColor.HtmlValue).replace("{1}", settings[0].LineColor.HtmlValue));
    $('#lb_Z5_color').attr("style", styleFormat.replace("{0}", settings[1].FillColor.HtmlValue).replace("{1}", settings[1].LineColor.HtmlValue));
    $('#lb_trk_color').attr("style", styleFormat.replace("{0}", settings[2].FillColor.HtmlValue).replace("{1}", settings[2].LineColor.HtmlValue));
    $('#lb_bg_color').attr("style", styleFormat.replace("{0}", settings[3].FillColor.HtmlValue).replace("{1}", settings[3].LineColor.HtmlValue));
    $('#lb_CBSA_color').attr("style", styleFormat.replace("{0}", settings[4].FillColor.HtmlValue).replace("{1}", settings[4].LineColor.HtmlValue));
    $('#lb_Urban_color').attr("style", styleFormat.replace("{0}", settings[5].FillColor.HtmlValue).replace("{1}", settings[5].LineColor.HtmlValue));
    $('#lb_County_color').attr("style", styleFormat.replace("{0}", settings[6].FillColor.HtmlValue).replace("{1}", settings[6].LineColor.HtmlValue));
    $('#lb_SLD_Senate_color').attr("style", styleFormat.replace("{0}", settings[7].FillColor.HtmlValue).replace("{1}", settings[7].LineColor.HtmlValue));
    $('#lb_SLD_House_color').attr("style", styleFormat.replace("{0}", settings[8].FillColor.HtmlValue).replace("{1}", settings[8].LineColor.HtmlValue));
    $('#lb_Voting_District_color').attr("style", styleFormat.replace("{0}", settings[9].FillColor.HtmlValue).replace("{1}", settings[9].LineColor.HtmlValue));
    $('#lb_SD_Elem_color').attr("style", styleFormat.replace("{0}", settings[10].FillColor.HtmlValue).replace("{1}", settings[10].LineColor.HtmlValue));
    $('#lb_SD_Secondary_color').attr("style", styleFormat.replace("{0}", settings[11].FillColor.HtmlValue).replace("{1}", settings[11].LineColor.HtmlValue));
    $('#lb_SD_Unified_color').attr("style", styleFormat.replace("{0}", settings[12].FillColor.HtmlValue).replace("{1}", settings[12].LineColor.HtmlValue));
//    $('#lb_Z3_color').attr("style", styleFormat.replace("{0}", settings[13].FillColor.HtmlValue).replace("{1}", settings[13].LineColor.HtmlValue));
//    $('#lb_Z3_color').attr("style", styleFormat.replace("{0}", settings[14].FillColor.HtmlValue).replace("{1}", settings[14].LineColor.HtmlValue));
    $('#lb_croute_color').attr("style", styleFormat.replace("{0}", settings[15].FillColor.HtmlValue).replace("{1}", settings[15].LineColor.HtmlValue));
}


// Prepare menu.
function initMenu() {
    $('li.head-link').hover(
			    function() { $('ul', this).css('display', 'block'); },
			    function() { $('ul', this).css('display', 'none'); }
    	    );

    $('#menu-container li.head-link ul li').hover(
			    function() { $(this).addClass('ui-state-hover'); },
			    function() { $(this).removeClass('ui-state-hover'); }
    	    );
}

var layout = null;
// Initialize the layout.
function initLayout() {
    layout = $('body').layout({
        center__paneSelector: ".non-deliverables-content",
        north__paneSelector: ".non-deliverables-north",
        east__paneSelector: ".non-deliverables-content-right",
        north__size: 36,
        east__size: 250,
        spacing_open: 2, // ALL panes
        spacing_closed: 4, // ALL panes
        north__spacing_open: 0,
        center__onresize_end: function() { if (map) { map.Resize(layout.panes.center.width() - 2, layout.panes.center.height()); } }
    });
}
//Initialize the map panel.
var mapPanel = null;
var map = null;
function initMapPanel() {
    mapPanel = new GPS.Nd.MapPanel({
        DivId: "map-panel"
    });
    map = mapPanel.GetMap();
}

//Initialize the mapDrawing instance.
var mapDrawing = null;

function initMapDraw() {
    mapDrawing = new GPS.Map.MapDrawing(map);
    mapDrawing.SetEndDrawingFunction(function() {
        // Show a dialog for the user to specify the properties of this custom area
        var dialog = new GPS.Nd.NdCustomAreaDialog({
            DialogElement: 'dialog-custom-area',
            DialogTitle: 'Non-Deliverable Custom Area',
            CustomPoints: mapDrawing.GetPointsArray()
        });

        // After the custom are is save to server successfully, show the custom area
        // on the map panel
        dialog.AttachEvent("onendsave", function(options) {
            options.Locations = mapDrawing.GetPoints();
            mapPanel.AddCustomArea(options);
        });

        dialog.Show();
    });
}

//Remove Custom Area
function RemoveCustomArea(id) {
    var shape = map.GetShapeByID(id);
    if (shape) { map.DeleteShape(shape); }
}

///
function OnMenuClick(action) {
    if (action == "custom") {
        mapDrawing.StartDrawing();
    } else if (action == "zipcode") {
        var dialog = new GPS.Nd.Nd5ZipAreaDialog({
            DialogElement: 'nd-5zip-dialog',
            DialogTitle: 'Non-Deliverable 5 Digit Zip'
        });

        dialog.AttachEvent("onendsave", function(options) {
            // Turn the 5 digit zip to non-deliverable or deliverable
            // according to the options specified
            mapPanel.SetFiveZipAreaEnabled(options);
        });

        dialog.Show();
    } else if (action == "tract") {
        var dialog = new GPS.Nd.NdTractAreaDialog({
            DialogElement: 'nd-tract-dialog',
            DialogTitle: 'Non-Deliverable Census Tract'
        });

        dialog.AttachEvent("onendsave", function(options) {
            // Turn the Census Tract to non-deliverable or deliverable
            // according to the options specified
            mapPanel.SetTractAreaEnabled(options);
        });

        dialog.Show();
    } else if (action == "blockgroup") {
        var dialog = new GPS.Nd.NdBgAreaDialog({
            DialogElement: 'nd-bg-dialog',
            DialogTitle: 'Non-Deliverable Census Block Group'
        });

        dialog.AttachEvent("onendsave", function(options) {
            // Turn the Census Block Group to non-deliverable or deliverable
            // according to the options specified
            mapPanel.SetBlockGroupAreaEnabled(options);
        });

        dialog.Show();
    } else if (action == "address") {
        var dialog = new GPS.Nd.NdAddressDialog({
            DialogElement: 'nd-address-dalog',
            DialogTitle: 'Non-Deliverable Address'
        });

        dialog.AttachEvent("onendsave", function(options) {
            // Turn the Address to non-deliverable or deliverable
            // according to the options specified
            mapPanel.AddNonDeliverableAddress(options);
        });

        dialog.Show();
    } else if (action == "addresses") {
    GPSUploadFile("ndaddressfile", function (ret) {
        if (ret.IsSuccess) {
            var service = new TIMM.Website.AreaServices.AreaWriterService();
            service.AddNonDeliverableAddresses(ret.Name, function (aRet) {
                //upgrade the file parse to a background job.
                GPS.Loading.hide();
                GPSAlert("<br />Please wait while TIMM uploads these address. You'll be notified when by email when upload is completed. Refresh this browser windows with F5 after the confirmation mail to see the result.<br />");
                return;
                //old code
                var i = 0;
                var alen = aRet.length;
                var successed = [];
                var failed = [];
                while (i < alen) {
                    if (aRet[i].IsSuccess) {
                        aRet[i].Name = aRet[i].Street + ", " + aRet[i].ZipCode;
                        aRet[i].Location = new VELatLong(aRet[i].Latitude, aRet[i].Longitude);
                        var locations = [];
                        var j = 0;
                        var llen = aRet[i].Locations.length;
                        while (j < llen) {
                            locations.push(new VELatLong(aRet[i].Locations[i][0], aRet[i].Locations[i][1]));
                            j++;
                        }
                        aRet[i].Locations = locations;
                        successed.push(aRet[i]);
                    }
                    else {
                        failed.push("&nbsp;&nbsp;" + aRet[i].Street + ", " + aRet[i].ZipCode);
                    }
                    i++;
                }
                mapPanel.AddNonDeliverableAddresses(successed);
                GPS.Loading.hide();
                if (failed.length > 0) {
                    GPSAlert("<p style=\"color:red;\">The following addresses can't be uploaded successfully, either because they are incorrect, or becauese they already exist in the system.</p>" + failed.join("<br />"));
                }
            });
        }
        return true;
    });
    }
}

///
function ShowHideMapClassification(cbxObj, classification) {
    mapPanel.SetClassificationVisible(classification, cbxObj.checked);
}
