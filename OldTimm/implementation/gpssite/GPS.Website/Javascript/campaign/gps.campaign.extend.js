
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

var campaign = null;

function LoadCampaign() {
    GPS.Loading.show();
    var args = GetUrlParms();
    var campaignId = args["cid"];
    if (campaignId) {
        //        var service = new TIMM.Website.CampaignServices.CampaignReaderService();
        //        service.GetCampaignById(campaignId, function(toCampaign) {
        //            BindCampaign(toCampaign);
        //            GPS.Loading.hide();
        //            //setInterval("SaveCampaignStatus()", 20000);
        //        });


        var params = [];
        params.push("cid=" + campaignId);
        $.ajax({
            type: "get",
            url: "Handler/CampaignMap.ashx",
            data: params.join('&'),
            dataType: "json",
            success: function(toCampaign, textStatus) {
                BindCampaign(toCampaign);
                GPS.Loading.hide();
                //setInterval("SaveCampaignStatus()", 20000);
            }
        });
    }
}

function BindCampaign(campainObj) {
    campaign = campainObj;
    $("#divCampaignName").text(campaign.CompositeName);
    $("#lbMasterCampaignNumber").text(campaign.CompositeName);
    GPS.PenetrationColorManager.BindCampaign(campainObj);
    if (campainObj.SubMaps) {
        campainObj.SubMaps.sort(function(a, b) { return a.OrderId - b.OrderId; });
        BindSubMaps(campainObj.SubMaps);
    }
    if (campainObj.Addresses) {
        addressPanel.AppendAddresses(campainObj.Addresses);
    }

    mapPanel.BindCampaign(campainObj);
}

function HoldCampaign(campainObj) {
    mapPanel.HoldCampaign(campainObj);
}

function SaveCampaignStatus() {
    GPS.Loading.show("Saving Data....");
    var tempCampaign = { Id: campaign.Id };
    HoldCampaign(tempCampaign);
    var service = new TIMM.Website.CampaignServices.CampaignWriterService();
    service.SaveCampaign(tempCampaign,
    function() {
        window.close();
    },
    function() {
        window.close();
    }
    );
}

function OnSaveCampaignClick() {
    SaveCampaignStatus();
}

function ShowHideMapClassification(cbxObj, classification) {
    mapPanel.SetClassificationVisible(classification, cbxObj.checked);
}




// GPS.Nd.NdAreaDialog represents a dialog used to edit the properties of
// an non-deliverable area.
GPS.CampaignDialog = GPS.EventTrigger.extend({
    // Initialize the state of this instance.
    //
    // This method is called when the object is constructed.
    //
    // The @options object should have the following properties:
    // @DialogElement - the element used as the container of the dialog.
    // @DialogTitle - the title of the dialog.
    init: function(options) {
        this._super(options);

        this._dialogElement = options.DialogElement;
        this._dialogTitle = options.DialogTitle || '';

        // The jQuery dialog object
        this._dialog = null;
    },

    // Show the dialog.
    Show: function() {
        if (!this._dialog) {
            this._dialog = this._ConstructDialog();
        }

        this._dialog.dialog("open");
        this._OnShow();
    },

    // Construct and return the dialog object.
    _ConstructDialog: function() {
        var thisObj = this;

        return $("#" + this._dialogElement).dialog({
            autoOpen: false,
            title: thisObj._dialogTitle,
            modal: true,
            width: 800,
            resizable: false,
            overlay: {
                opacity: 0.5,
                background: "black"
            },
            buttons: {
                "Cancel": function() {
                    if (thisObj._OnBeforeCancel()) {
                        thisObj._labelPerError.html('');
                        $(this).dialog("close");
                        thisObj._OnCancel();
                    }
                },
                "Ok": function() {
                    if (thisObj._OnBeforeOk()) {
                        $(this).dialog("close");
                        thisObj._OnOk();
                    }
                }
            }
        });
    },

    // Return a boolean value indicating whether the dialog Cancel action should continue.
    _OnBeforeCancel: function() {
        return true;
    },

    // Called after the Cancel button is clicked and the dialog is closed.
    _OnCancel: function() {
    },

    // Return a boolean value indicating whether the dialog Ok action should continue.
    _OnBeforeOk: function() {
        return true;
    },

    // Called after the Ok button is clicked and the dialog is closed.
    _OnOk: function() {
    },

    // Called after the dialog is shown.
    _OnShow: function() {
    }
});



// GPS.Nd.Nd5ZipAreaDialog represents a dialog used to edit the properties of
// an non-deliverable 5 digit zip area.
GPS.CampaignAdjustCountDialog = GPS.CampaignDialog.extend({
    // Initialize the state of this instance.
    //
    // This method is called when the object is constructed.
    //
    // The @options object should have the properties specified in the super class.
    init: function (options) {
        this._super(options);
    },

    // Override the method in the super class.
    //
    // Get input controls.
    _ConstructDialog: function () {
        // Construct the dialog
        var dialog = this._super();

        // Get input controls for later use
        this._txtTotal = $('#campaign-txt-adjust-total');
        this._txtCount = $('#campaign-txt-adjust-count');
        this._txtPer = $('#campaign-txt-adjust-per');
        this._labelTotalError = $('#campaign-error-adjust-total');
        this._labelCountError = $('#campaign-error-adjust-count');
        this._labelPerError = $('#campaign-error-adjust-per');

        return dialog;
    },

    // Called after the dialog is shown.
    _OnShow: function () {
        if (!this._map) {
            this._map = new VEMap('adjust-count-dialog-map');
            var bingMapKey = $("#bingMapKey").val();
            if (!bingMapKey) {
                alert("Please add bing map key");
            }
            this._map.SetCredentials(bingMapKey);
            this._map.LoadMap();
            var thisObj = this;
            this._map.AttachEvent("onclick", function (e) {
                thisObj._OnClick(e);
            });
            this._labelPerError.html('');
        }
    },

    _OnClick: function (e) {
        var thisObj = this;
        if (e.leftMouseButton && e.elementID) {
            var shape = this._map.GetShapeByID(e.elementID);
            if (shape) {
                if (thisObj._ValidatePerValue()) {
                    thisObj._labelPerError.html('');
                    thisObj._HoldActiveShape();
                    thisObj._activeShape = shape;
                    thisObj._RefreshActive();
                }
                else {
                    thisObj._labelPerError.html('Your input: ' + thisObj._txtPer.val() + ' is invalid! Please enter only whole or decimal numbers.');
                }
            }
        }
    },

    _HoldActiveShape: function () {
        //if (!this._validatePer(this._txtPer.val())) { alert('should be integer!') }
        var total = Number(this._txtTotal.val());
        var count = Number(this._txtCount.val());
        var per = Number(this._txtPer.val()) / 100;
        var modifyData = this._activeShape.GPSAreaData;
        if (modifyData) {
            if (modifyData.Total != total || modifyData.Count != count || modifyData.PartPercentage != per) {
                var isPartModified = false;
                if (this._area.GetAttributes() && this._area.GetAttributes().PartCount > 1
               && (this._area.GetClassification() == 1 || this._area.GetClassification() == 15)) {
                    total = this._area.GetAttributes().Total ? Number(this._area.GetAttributes().Total) : Number(this._area.GetAttributes().All);
                    count = this._area.GetAttributes().Count ? Number(this._area.GetAttributes().Count) : 0;
                    total = Math.round(total * per);
                    count = Math.round(count * per);
                    isPartModified = true;
                }
                this._activeShape.GPSAreaIsModified = true;
                this._activeShape.GPSAreaData = {
                    Id: this._activeShape.GPSAreaId,
                    Total: total,
                    Count: count,
                    PartPercentage: per,
                    IsPartModified: isPartModified
                };
            }
        }
        else {
            this._activeShape.GPSAreaIsModified = true;
            this._activeShape.GPSAreaData = {
                Id: this._activeShape.GPSAreaId,
                Total: total,
                Count: count,
                PartPercentage: per
            };
        }
        this._SetCustomIcon(this._activeShape);
    },

    _SetCustomIcon: function (shape) {
        var iconStr = "<div class='importareaicon'>{3}{0}<br />{1}</div>"
            .replace("{0}", shape.GPSAreaData.Total).replace("{1}", shape.GPSAreaData.Count);
        if (this._area.GetAttributes() && this._area.GetAttributes().PartCount > 1
               && (this._area.GetClassification() == 1 || this._area.GetClassification() == 15)) {
            iconStr = iconStr.replace("{3}", shape.GPSAreaData.PartPercentage * 100 + "%<br />");
        }
        else {
            if (shape.GPSAreaData && shape.GPSAreaData.Total != 0) {

                iconStr = iconStr.replace("{3}", "");
            }
            else {
                iconStr = "<div></div>";
                shape.SetCustomIcon(iconStr);
            }
        }
        shape.SetCustomIcon(iconStr);

    },

    _AddShapes: function (toAreas) {
        var points = [];
        for (var t in toAreas) {
            var toArea = toAreas[t];
            var locations = [];
            for (var i in toArea.Locations) {
                locations.push(new VELatLong(toArea.Locations[i].Latitude, toArea.Locations[i].Longitude));
                points.push(new VELatLong(toArea.Locations[i].Latitude, toArea.Locations[i].Longitude));
            }
            if (toArea.Id != this._activeShape.GPSAreaId) {
                //var attributes = DictionaryConvertToClient(toArea.Attributes);
                var shape = new VEShape(VEShapeType.Polygon, locations);
                shape.GPSAreaId = toArea.Id;
                if (toArea.Classification == 15) {
                    if (toArea.Attributes[12]) {
                        var total = toArea.Attributes[12] ? Number(toArea.Attributes[12].Value) : 0;
                        var count = toArea.Attributes[13] ? toArea.Attributes[13].Value : 0;
                        var per = toArea.Attributes[14] ? Number(toArea.Attributes[14].Value) : 1;
                        var isPartModified = toArea.Attributes[15] ? (toArea.Attributes[15].Value == "True" ? true : false) : false;
                    } else {
                        var total = Number(toArea.Attributes[7].Value);
                        var count = 0;
                        var per = 0;
                        var isPartModified = 0;
                    }
                }
                else if (toArea.Classification == 1) {
                    if (toArea.Attributes[11]) {
                        var total = toArea.Attributes[11] ? Number(toArea.Attributes[11].Value) : 0;
                        var count = toArea.Attributes[12] ? toArea.Attributes[12].Value : 0;
                        var per = toArea.Attributes[13] ? Number(toArea.Attributes[13].Value) : 1;
                        var isPartModified = toArea.Attributes[14] ? (toArea.Attributes[14].Value == "True" ? true : false) : false;
                    } else {
                        var total = Number(toArea.Attributes[6].Value);
                        var count = 0;
                        var per = 0;
                        var isPartModified = 0;
                    }
                }
                shape.GPSAreaData = {
                    Id: toArea.Id,
                    Total: total,
                    Count: count,
                    PartPercentage: per,
                    IsPartModified: isPartModified
                };
                this._SetCustomIcon(shape);
                this._map.AddShape(shape);
                this._shapes.Set(toArea.Id, shape);
            }
        }
        if (points.length > 0) {
            this._map.SetMapView(points);
        }
        this._RefreshActive();
    },

    _RefreshActive: function () {
        this._shapes.Each(function (id, shp) {
            shp.SetLineColor(new VEColor(0, 0, 255, 0.5));
            shp.SetLineWidth(2);
            //            shp.HideIcon();

        });

        if (this._activeShape.GPSAreaData) {
            this._txtTotal.val(this._activeShape.GPSAreaData.Total);
            this._txtCount.val(this._activeShape.GPSAreaData.Count);
            this._txtPer.val(this._activeShape.GPSAreaData.PartPercentage * 100);
        }
        else {
            this._txtTotal.val(0);
            this._txtCount.val(0);
            this._txtPer.val(100);
        }
        //        this._activeShape.ShowIcon();
        this._activeShape.SetLineColor(new VEColor(255, 0, 0, 0.5));
        this._activeShape.SetLineWidth(5);
    },

    _ValidatePerValue: function () {

        var per = this._txtPer.val();
        var strP = /^\d+(\.\d+)?$/;
        return strP.test(per);
    },

    _AddShape: function (area) {
        //        alert(campaign.AreaDescription);

        //        for (var p in area.GetAttributes()) {
        //            if (typeof (p) != "function") {
        //                alert(p);
        //            }
        //        }

        var total = area.GetAttributes().Total ? Number(area.GetAttributes().Total) : Number(area.GetAttributes().All);

//        switch (campaign.AreaDescription) {
//            case "APT ONLY":
//                total = area._attributes.Apt;
//                break;
//            case "HOME ONLY":
//                total = area._attributes.Home;
//                break;
//            case "APT + HOME":
//            default:
//                total = area._attributes.Home + area._attributes.Apt;
//                //result.Count += 0;
//                break;
//        }

        var count = area.GetAttributes().Count ? Number(area.GetAttributes().Count) : 0;
        var per = area.GetAttributes().PartPercentage ? Number(area.GetAttributes().PartPercentage) : 0;
        var isPartModified = area.GetAttributes().IsPartModified ? (area.GetAttributes().IsPartModified == "True" ? true : false) : false;
        var oshape = area.GetShape();
        var nshape = new VEShape(VEShapeType.Polygon, oshape.GetPoints());
        nshape.SetTitle(oshape.GetTitle());
        nshape.SetDescription(oshape.GetDescription());
        nshape.GPSAreaId = area.GetId();
        nshape.GPSAreaData = {
            Id: area.GetId(),
            Total: total,
            Count: count,
            PartPercentage: per,
            IsPartModified: isPartModified
        };
        this._SetCustomIcon(nshape);
        this._map.AddShape(nshape);
        this._shapes.Set(area.GetId(), nshape);
        this._activeShape = nshape;
        this._RefreshActive();
    },

    Bind: function (area, fnback) {
        if (fnback) { this._fnBack = fnback; }
        else { this._fnBack = null; }
        this._area = area;
        this._map.DeleteAllShapes();
        this._shapes = new GPS.QArray();
        this._AddShape(area);
        if (area.GetAttributes() && area.GetAttributes().PartCount > 1
               && (area.GetClassification() == 1 || area.GetClassification() == 15)) {
            $('#campaign-row-adjust-total').hide();
            $('#campaign-row-adjust-count').hide();
            $('#campaign-row-adjust-per').show();
            var service = new TIMM.Website.AreaServices.AreaReaderService();
            var thisObj = this;
            if (area.GetClassification() == 1) {
                service.GetCampaignFiveZipAreas(campaign.Id, area.GetName(), function (toAreas) {
                    thisObj._AddShapes(toAreas);
                });
            }
            else {
                service.GetCampaignCRouteAreas(campaign.Id, area.GetName(), function (toAreas) {
                    thisObj._AddShapes(toAreas);
                });
            }
        }
        else {
            $('#campaign-row-adjust-total').show();
            $('#campaign-row-adjust-count').show();
            $('#campaign-row-adjust-per').hide();
            this._map.SetMapView(area.GetShape().GetPoints());
        }
    },

    // Return a boolean value indicating whether the dialog Ok action should continue.
    _OnBeforeOk: function () {
        var result = true;
        var thisObj = this;
        if (thisObj._ValidatePerValue()) {
            this._HoldActiveShape();
            var total = 0;
            if (this._area.GetAttributes() && this._area.GetAttributes().PartCount > 1
               && (this._area.GetClassification() == 1 || this._area.GetClassification() == 15)) {

                if (this._area.GetClassification() == 15) { // croute
                    this._shapes.Each(function (id, shp) {
                        //if (!shp.GPSAreaData.IsPartModified) {
                        if (shp.GPSAreaData.PartPercentage <= 0 || shp.GPSAreaData.PartPercentage > 1) {
                            result = false;
                            thisObj._labelPerError.html('You should adjust all parts.The percentage of any part should large than 0% and less than or equal 100%.');
                        }
                    });
                }
                else { // non-croute
                    this._shapes.Each(function (id, shp) {
                        //if (!shp.GPSAreaData.IsPartModified) {
                        if (shp.GPSAreaData.PartPercentage == 0) {
                            result = false;
                            thisObj._labelPerError.html('You should adjust all parts.The percentage of any part should not be 0%.');
                        }
                        total += Math.round(shp.GPSAreaData.PartPercentage * 10000);
                    });
                    if (result && total != 10000) {
                        result = false;
                        this._labelPerError.html('The sum of all parts must be 100%.');
                    }
                }
            }
        }
        else {
            thisObj._labelPerError.html('Your input: ' + thisObj._txtPer.val() + ' is invalid! Please enter only whole or decimal numbers.');
            result = false;
        }
        if (result) thisObj._labelPerError.html('');
        return result;
    },

    // Override the method in the super class.
    //
    // Check if the specified 5 digit zip exists. If yes, fire an 'onendsave' event.
    _OnOk: function () {
        // Get input values
        if (this._area) {
            this._HoldActiveShape();
            var datas = [];
            this._shapes.Each(function (id, shp) {
                if (shp.GPSAreaIsModified) {
                    datas.push(shp.GPSAreaData);
                }
            });
            var service = new TIMM.Website.AreaServices.AreaWriterService();
            var thisObj = this;
            service.AdjustData(campaign.Id, this._area.GetClassification(), datas, function () {
                var shape = thisObj._shapes.Get(thisObj._area.GetId())
                if (shape) {
                    thisObj._area.AdjustCount(shape.GPSAreaData.Total, shape.GPSAreaData.Count);
                }
                if (thisObj._fnBack) {
                    thisObj._fnBack();
                }
            });
        }
    }
});