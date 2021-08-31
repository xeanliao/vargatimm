﻿
GPS.DMLayout = function() { };

$(document).ready(function() {
    initLayout();
    initClassificationColors();
    initSubMapDMPannel();
    initDJDMPannel();
    //initMapPanel();
});

// Initialize the layout.
function initLayout() {
    outerLayout = $('body').layout({
        center__paneSelector: ".outer-center"
		    , north__paneSelector: ".ui-layout-north"
		    , south__paneSelector: ".ui-layout-south"
		    , east__paneSelector: ".outer-east"
		    , north__size: 39
		    , south__size: 50
		    , east__size: 250
		    , spacing_open: 2 // ALL panes
		    , spacing_closed: 4 // ALL panes
		    , north__spacing_open: 2
		    , south__spacing_open: 2
		    , center__onresize: "innerLayout.resizeAll"
		    , east__onresize: "eastInnerLayout.resizeAll"
    });

    innerLayout = $('div.outer-center').layout({
        center__paneSelector: ".inner-center"
		    , north__paneSelector: ".inner-north"
		    , south__paneSelector: ".inner-south"
		    , north__size: 90
		    , south__size: 90
		    , spacing_open: 2  // ALL panes
		    , spacing_closed: 4  // ALL panes
		    , center__onresize: 'mapLayout.resizeAll'
    });

    eastInnerLayout = $('div.outer-east').layout({
        center__paneSelector: ".east-inner-center"
		    , north__paneSelector: ".east-inner-north"
		    , south__paneSelector: ".east-inner-south"
		    , north__size: 189
		    , south__size: 87
		    , spacing_open: 2  // ALL panes
		    , spacing_closed: 4  // ALL panes
		    , center__onresize: 'submapLayout.resizeAll'
    });

    mapLayout = $('#map-inner').layout({
        center__paneSelector: "#map-container"
		    , spacing_open: 0  // ALL panes
		    , spacing_closed: 0  // ALL panes
		    , center__onopen: function() { }
		    , center__onresize_end: function() {
		        if (map) {
		            map.ResizeMap(mapLayout.panes.center.width() - 2, mapLayout.panes.center.height());
		        }
		    }
    });

    submapLayout = $('#sub-map-container-inner').layout({
        center__paneSelector: "#sub-map-inner"
		    , spacing_open: 0  // ALL panes
		    , spacing_closed: 0  // ALL panes
		    , center__onopen: function() { }
		    , center__onresize_end: function() { }
    });
}

var dmPanel = null;
function initSubMapDMPannel() {
    dmPanel = new GPS.DMPanel('sub-map-panel');
    $("#sub-map-container").tabs();
}

var djPanel = null;
function initDJDMPannel() {
    djPanel = new GPS.DJPanel('distribution-job-panel');
    $("#sub-map-container").tabs();
}


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

// Init map panel
var map = null;
function initMapPanel() {
    GPS.Container.Register({
        Id: "campaignmap.arealayer",
        IsSingleton: false,
        ClassObject: GPS.CampaignMapPanel.AreaLayer,
        Instance: null
    });
    GPS.Container.Register({
        Id: "campaignarealayer.area",
        IsSingleton: false,
        ClassObject: GPS.CampaignMapPanel.Area,
        Instance: null
    });

    map = new GPS.CampaignMapPanel({
        DivId: "map-inner"
    });
}



/**
* Called when the user clicks on the 'Users' menu item.
*/
function OnUsersClick() {
    window.open('Users.aspx', '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
}

/**
* Called when the user clicks on the 'Non-Deliverables' menu item.
*/
function OnNonDeliverablesClick() {
    window.open('NonDeliverables.aspx', '_blank', 'resizable=yes,status=yes,toolbar=no,menubar=no,location=no');
}

/**
* Called when the user clicks on the 'Log Out' menu item.
*/
function OnLogOutClick() {
    var data = "logout=true";
    $.ajax({
        type: "POST",
        url: "Handler/LoginHandler.ashx",
        data: data,
        success: function(msg) {
            window.open('login.html', '_self');
        }
    });
}


function LoadSubMap() {
    GPS.Loading.show();
    var args = GetUrlParms();
    var campaignid = args["cid"];
    var dmReader = new TIMM.Website.DistributionMapServices.DMReaderService();
    var pp = 1;
    dmReader.GetSubMaps(campaignid, function(r) {
        if (r) {
            BindDMs(r.sort(function(a, b) { return a.OrderId - b.OrderId }));
            GPS.Loading.hide();
        }
    },
    function(e) {
      alert(e); 
    }
    , null); 
}

function LoadDistributionJobs() {
    GPS.Loading.show();
    var args = GetUrlParms();
    var campaignid = args["cid"];
    var djReader = new TIMM.Website.DistributionMapServices.DJReaderService();
    djReader.GetDistributionJobs(campaignid, function(r) {
        if (r) {
            BindDJs(r, campaignid);
            GPS.Loading.hide();
        }
    }, null, null);
}