/*
*   Class GPS.Map.AreaBase
*   @options - class { Classification: - area classification
*                      Id: - area id
*                      Attributes: - object
*                    }
*/
GPS.Map.AreaBase = GPS.EventTrigger.extend({
    // initalization method - this function is invoked dynamic when new instance
    init: function (options) {
        this._super(options);
        this._Define();
        this._InitAreaBase(options);
    },
    // define basic attributes
    _Define: function () {
        this._symbol = "areabase";
        this._classification = null;
        this._id = null;
        this._name = null;
        this._attributes = null;
        this._shapes = null;
        this._referenceBoxes = null;
        this._isEnabled = null;
        this._relations = null;
        this._description = null;

        this._latitude = null;
        this._longitude = null;
    },
    // initalization method
    _InitAreaBase: function (options) {
        if (options.Symbol) { this._symbol = options.Symbol; }
        this._classification = options.Classification;
        this._id = options.Id;
        this._name = options.Name;
        this._attributes = options.Attributes;
        this._referenceBoxes = 0;
        this._isEnabled = options.IsEnabled;
        this._relations = options.Relations;
        this._description = options.Description;

        this._latitude = options.Latitude;
        this._longitude = options.Longitude;

        this._InitShapes(options);
    },
    // initalization method
    _InitShapes: function (options) {
        this._shapes = [];
        var shapeId = 0;
        var points = [];
        if (options.Locations.length > 0) {
            shapeId = options.Locations[0].ShapeId;
        }

        for (var i = 0; i < options.Locations.length; i++) {
            if (shapeId != options.Locations[i].ShapeId) {
                var shape = new VEShape(VEShapeType.Polygon, points);
                shape.GPSAreaId = this._id;
                shape.GPSShapeId = shapeId;
                this._shapes.push(shape);
                shapeId = options.Locations[i].ShapeId;
                points = [];
            }
            points.push(new VELatLong(options.Locations[i].Latitude, options.Locations[i].Longitude));
        }
        var lShape = new VEShape(VEShapeType.Polygon, points);
        lShape.GPSAreaId = this._id;
        lShape.GPSShapeId = shapeId;
        this._shapes.push(lShape);
        this._InitShapesStyle(options);
    },
    // initalization shape style
    _InitShapesStyle: function (options) {
        var styleOptions = GPS.Map.AreaStyleManager.Get(options);
        this.SetShapesStyle(styleOptions);
    },
    // set shap style
    SetShapesStyle: function (options) {
        var i = 0;
        var slen = this._shapes.length;
        while (i < slen) {
            var shape = this._shapes[i];

            if (options.Title) {
                shape.SetTitle(options.Title);
            }
            if (options.Description) {
                shape.SetDescription(options.Description);
            }
            if (options.CustomIcon) {
                shape.SetCustomIcon(options.CustomIcon);
            }
            if (typeof (options.IconVisible) != 'undefined') {
                if (options.IconVisible) {
                    shape.ShowIcon();
                }
                else {
                    shape.HideIcon();
                }
            }
            if (options.LineColor) {
                shape.SetLineColor(options.LineColor);
            }
            if (options.LineWidth) {
                shape.SetLineWidth(options.LineWidth);
            }
            if (options.FillColor) {
                shape.SetFillColor(options.FillColor);
            }
            i++;
        }
    },
    // get area classification
    GetClassification: function () {
        return this._classification;
    },
    // get area id
    GetId: function () {
        return this._id;
    },
    // get area name
    GetName: function () {
        return this._name;
    },
    // get attributes
    GetAttributes: function () {
        return this._attributes;
    },
    // get VEShape object
    GetShape: function () {
        return this._shapes[0];
    },
    // add shapes to shape layer
    AddToShapeLayer: function (shapeLayer) {
        var i = 0;
        var slen = this._shapes.length;
        while (i < slen) {
            shapeLayer.AddShape(this._shapes[i]);
            i++;
        }
    },
    //delete shapes from shape layer
    DeleteFromShapeLayer: function (shapeLayer) {
        var i = 0;
        var slen = this._shapes.length;
        while (i < slen) {
            shapeLayer.DeleteShape(this._shapes[i]);
            i++;
        }
    },
    // add reference box
    AddReferenceBox: function () {
        this._referenceBoxes++;
    },
    // remove reference box
    RemoveReferenceBox: function () {
        this._referenceBoxes--;
    },
    // get enabled
    GetEnabled: function () {
        return this._isEnabled;
    },
    // get reference box
    GetReferenceBox: function () {
        return this._referenceBoxes;
    },
    //Get Relations
    GetRelations: function () {
        return this._relations;
    }
});


/*
*   Class GPS.Map.AreaStyleManager
*   @options - class { Classification: - area classification
*                      Attributes: - object
*                    }
*/
GPS.Map.AreaStyleManager = {
    // get style by options
    Get: function(options) {
        var style = null;
        switch (options.Classification) {
            case 0:
                style = this.GetThreeZipStyle(options);
                break;
            case 1:
                style = this.GetFiveZipStyle(options);
                break;
            case 2:
                style = this.GetTractStyle(options);
                break;
            case 3:
                style = this.GetBlockGroupStyle(options);
                break;
            case 13:
                style = this.GetCustomAreaStyle(options);
                break;
            case 14:
                style = this.GetAddressStyle(options);
                break;
            case 15:
                style = this.GetCrouteStyle(options);
                break;
            default:
                style = this.GetDefaultStyle(options);
                break;
        }
        return style;
    },
    // get three zip style
    GetThreeZipStyle: function(options) {
        var styleOptions = this.GetDefaultStyle(options);
        styleOptions.CustomIcon = '<div class="z3icon"></div>';

        styleOptions.Description = "<br />" + ["SFDU:" + options.Attributes.Home + " MFDU:" + options.Attributes.Apt,
            "Total:" + options.Attributes.All,
            ].join("<br />");

        styleOptions.IconVisible = true;
        return styleOptions;
    },
    // get five zip style
    GetFiveZipStyle: function(options) {
        var styleOptions = this.GetDefaultStyle(options);
        if (options.Attributes.IsInnerRing == "0") {
            styleOptions.CustomIcon = '<div id="zip' + options.Id + '" class="z5icon"></div>';

            // highlight searched area
            if(window["searchedAreas"] != undefined) {
                for (var i = 0; i < searchedAreas.length; i++) {
                    if (searchedAreas[i] == "zip" + options.Id)
                        styleOptions.CustomIcon = '<div id="zip' + options.Id + '" class="z5iconA"></div>';
                }
            }

            styleOptions.Title = options.Name;
            styleOptions.Description = "Zip Code: " + options.Name;
            if (options.Attributes.Total) {
                styleOptions.Description += "<br />" + ["Total:" + options.Attributes.Total,
            "Count:" + options.Attributes.Count,
            "Penetration:" + (Number(options.Attributes.Penetration) * 100).toFixed(2) + '%'].join("<br />");
            }
            else {
                styleOptions.Description += "<br />" + ["SFDU:" + options.Attributes.Home + " MFDU:" + options.Attributes.Apt,
            "Total:" + options.Attributes.All,
            ].join("<br />");
            }

            if (!options.IsEnabled) {
                if (options.Attributes.OTotal && Number(options.Attributes.OTotal) > 0) {
                    styleOptions.Description += "<br />H/H:" + options.Attributes.OTotal + "<br />";
                }
                if (options.Description) {
                    styleOptions.Description += "Description:" + options.Description;
                }
                styleOptions.CustomIcon = '<div class="addressicon"></div>';
            }
            styleOptions.IconVisible = true;
        }
        else {
            styleOptions.IconVisible = false;
        }
        return styleOptions;
    },
    // get tract style
    GetTractStyle: function(options) {
        var styleOptions = this.GetDefaultStyle(options);
        styleOptions.CustomIcon = '<div class="tracticon"></div>';
        styleOptions.Title = 'TRK:' + options.Name;
        styleOptions.Description = ["State:" + options.Attributes.State,
            "County:" + options.Attributes.County,
            "Tract:" + options.Attributes.Tract].join("<br />");
        if (options.Attributes.Total) {
            styleOptions.Description += "<br />" + ["Total:" + options.Attributes.Total,
            "Count:" + options.Attributes.Count,
            "Penetration:" + (Number(options.Attributes.Penetration) * 100).toFixed(2) + '%'].join("<br />");
        }
        if (!options.IsEnabled) {
            if (options.Attributes.OTotal && Number(options.Attributes.OTotal) > 0) {
                styleOptions.Description += "<br />H/H:" + options.Attributes.OTotal;
            }
            if (options.Description) {
                styleOptions.Description += "<br />Description:" + options.Description;
            }
            styleOptions.CustomIcon = '<div class="addressicon"></div>';
        }
        styleOptions.IconVisible = true;
        styleOptions.IconVisible = true;
        return styleOptions;
    },
    // get block group style
    GetBlockGroupStyle: function(options) {
        var styleOptions = this.GetDefaultStyle(options);
        styleOptions.CustomIcon = '<div class="bgicon"></div>';
        styleOptions.Title = 'BG:' + options.Name;
        styleOptions.Description = ["State:" + options.Attributes.State,
            "County:" + options.Attributes.County,
            "Tract:" + options.Attributes.Tract,
            "BlockGroup:" + options.Attributes.BlockGroup].join("<br />");
        if (options.Attributes.Total) {
            styleOptions.Description += "<br />" + ["Total:" + options.Attributes.Total,
            "Count:" + options.Attributes.Count,
            "Penetration:" + (Number(options.Attributes.Penetration) * 100).toFixed(2) + '%'].join("<br />");
        }
        if (!options.IsEnabled) {
            if (options.Attributes.OTotal && Number(options.Attributes.OTotal) > 0) {
                styleOptions.Description += "<br />H/H:" + options.Attributes.OTotal;
            }
            if (options.Description) {
                styleOptions.Description += "<br />Description:" + options.Description;
            }
            styleOptions.CustomIcon = '<div class="addressicon"></div>';
        }
        styleOptions.IconVisible = true;
        return styleOptions;
    },
    // get custom area style
    GetCustomAreaStyle: function(options) {
        var styleOptions = this.GetDefaultStyle(options);
        styleOptions.CustomIcon = '<div class="addressicon"></div>';
        styleOptions.IconVisible = true;
        styleOptions.Title = options.Name;
        styleOptions.Description = "H/H:" + options.Attributes.OTotal + "<br />" + "Description:" + options.Description;
        return styleOptions;
    },
    // get address style
    GetAddressStyle: function(options) {
        var styleOptions = this.GetDefaultStyle(options);
        styleOptions.CustomIcon = '<div class="addressicon"></div>';
        styleOptions.IconVisible = true;
        styleOptions.Title = options.Name;
        styleOptions.Description = "Geofence Scope:" + options.Attributes.Geofence + "<br />";
        styleOptions.Description += "Description:" + options.Description;
        return styleOptions;
    },
    // get address style
    GetCrouteStyle: function(options) {
        var styleOptions = this.GetDefaultStyle(options);
        if (options.Attributes.IsInnerRing == "0") {
            styleOptions.CustomIcon = '<div class="bgicon"></div>';
            styleOptions.IconVisible = true;
            styleOptions.Title = options.Name;
            styleOptions.Description = "Zip Code:" + options.Attributes.Zip;
            if (options.Attributes.Total) {
                styleOptions.Description += "<br />" + ["Total:" + options.Attributes.Total,
            "Count:" + options.Attributes.Count,
            "Penetration:" + (Number(options.Attributes.Penetration) * 100).toFixed(2) + '%'].join("<br />");
            }
            else {
                styleOptions.Description += "<br />" + ["SFDU:" + options.Attributes.Home + " MFDU:" + options.Attributes.Apt,
            "Total:" + options.Attributes.All,
            ].join("<br />");
            }


            if (!options.IsEnabled) {
                if (options.Attributes.OTotal && Number(options.Attributes.OTotal) > 0) {
                    styleOptions.Description += "<br />H/H:" + options.Attributes.OTotal;
                }
                if (options.Description) {
                    styleOptions.Description += "<br />Description:" + options.Description;
                }



                styleOptions.CustomIcon = '<div class="addressicon"></div>';
            }
            //        styleOptions.Description += "Description:" + options.Description;
        }
        else {
            styleOptions.IconVisible = false;
        }
        return styleOptions;
    },
    // get style function
    GetDefaultStyle: function(options) {
        var styleOptions = null;
        if (options.IsEnabled) {
            var highLignt = options.IsHignLight ? 0.2 : 0;
            var fColor = GPS.ClsSettings[Number(options.Classification)].FillColor;
            var lColor = GPS.ClsSettings[Number(options.Classification)].LineColor;
            styleOptions = {
                LineColor: new VEColor(lColor.R, lColor.G, lColor.B, lColor.A + highLignt),
                FillColor: new VEColor(fColor.R, fColor.G, fColor.B, fColor.A + highLignt),
                IconVisible: false
            };
        }
        else {
            styleOptions = {
                LineColor: new VEColor(0, 0, 0, 0.6),
                FillColor: new VEColor(0, 0, 0, 0.6),
                CustomIcon: '<div class="addressicon"></div>'
            };
        }
        if (options.Classification < 4 || options.Classification == 15) {
            styleOptions.IconVisible = true;
            styleOptions.Title = options.Name;
            if (!options.IsEnabled) {
                if (options.Attributes.OTotal && Number(options.Attributes.OTotal) > 0) {
                    styleOptions.Description = "H/H:" + options.Attributes.OTotal + "<br />";
                }
                if (options.Description) {
                    styleOptions.Description += "Description:" + options.Description;
                }
            }
        }
        return styleOptions;
    }
}