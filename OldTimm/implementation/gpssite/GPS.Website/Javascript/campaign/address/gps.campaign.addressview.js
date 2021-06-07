GPS.AddressListView = GPS.EventTrigger.extend({   // initalization method - this function is invoked dynamic when new instance
    init: function(options) {
        this._super(options);
        this._Define();
        this._InitAddressListViewBase(options);
    },
    // define basic attributes
    _Define: function() {

    },
    // initalization method
    _InitAddressListViewBase: function(div) {

    },

    Append: function(address) {
        var thisObj = this;
        var htmlString = "<div id='{0}'>{1}</div>";
        htmlString = htmlString.replace("{0}", address.Id);
        htmlString = htmlString.replace("{1}", address.Street + ", " + address.ZipCode);
        var item = $(htmlString);
        $(item).click(function() {
            $('#address_list div.selected').removeClass('selected');
            $(item).addClass('selected');
            thisObj.TriggerEvent('onitemselect', $(item).attr('id'));
        });
        $('#address_list div.selected').removeClass('selected');
        $(item).addClass('selected');
        $('#address_list').append($(item));
    },

    Delete: function(addressId) {
        $('#' + addressId).remove();
    },

    Active: function(addressId) {
        $('#address_list div.selected').removeClass('selected');
        $('#' + addressId).addClass('selected');
    },

    Hide: function() { $('#address_list').hide(); },

    Show: function() { $('#address_list').show(); }

});

GPS.AddressFormView = GPS.EventTrigger.extend({   // initalization method - this function is invoked dynamic when new instance
    init: function(options) {
        this._super(options);
        this._Define();
        this._InitAddressFormViewBase(options);
    },
    // define basic attributes
    _Define: function() {
        this._address = null;
        this._addressShape = null;
        this._center = null;
        this._radius0 = null;
        this._radius1 = null;
        this._radius2 = null;
        this._radiusUnit = null;
        this._changedCenter = null;
        this._changedRadiuses = null;
    },
    _OnSave: function() {
        GPS.Loading.show();
        var radiuses = [];
        var allChanged = false;
        var rdChanged = false;
        this._address.Radiuses.sort(function(a, b) { return a.Length - b.Length });
        if (this._radiusUnit != this._address.Radiuses[0].LengthMeasuresId) {
            for (var i in this._address.Radiuses) {
                this._address.Radiuses[i].LengthMeasuresId = this._radiusUnit;
            }
            allChanged = true;
        }
        if (this._center.Latitude != this._address.Latitude || this._center.Longitude != this._address.Longitude) {
            this._address.Latitude = this._center.Latitude;
            this._address.Longitude = this._center.Longitude;
            allChanged = true;
        }

        if (this._radius0 != this._address.Radiuses[0].Length || this._radius1 != this._address.Radiuses[1].Length || this._radius2 != this._address.Radiuses[2].Length) {
            rdChanged = true;
        }

        //        if (this._radius0 != this._address.Radiuses[0].Length) {
        //            this._address.Radiuses[0].Length = this._radius0;
        //            radiuses.push(this._address.Radiuses[0]);
        //        }
        //        if (this._radius1 != this._address.Radiuses[1].Length) {
        //            this._address.Radiuses[1].Length = this._radius1;
        //            radiuses.push(this._address.Radiuses[1]);
        //        }
        //        if (this._radius2 != this._address.Radiuses[2].Length) {
        //            this._address.Radiuses[2].Length = this._radius2;
        //            radiuses.push(this._address.Radiuses[2]);
        //        }
        if (allChanged || rdChanged) {
            this._address.Radiuses[0].Length = this._radius0;
            radiuses.push(this._address.Radiuses[0]);
            this._address.Radiuses[1].Length = this._radius1;
            radiuses.push(this._address.Radiuses[1]);
            this._address.Radiuses[2].Length = this._radius2;
            radiuses.push(this._address.Radiuses[2]);
            radiuses = this._address.Radiuses;

            for (var i in radiuses) {
                radiuses[i].Relations = [];
            }
            var thisObj = this;
            //                var service = new TIMM.Website.CampaignServices.CampaignWriterService();
            //                service.ChangeAddressRadius(this._address.Id, this._center.Latitude, this._center.Longitude, radiuses, function(ret) {
            //                   for (var i in ret) {
            //                        for (j in thisObj._address.Radiuses) {
            //                            if (thisObj._address.Radiuses[j].Id == ret[i].Id) {
            //                                thisObj._address.Radiuses[j].Relations = ret[i].Relations;
            //                            }
            //                        }
            //                    }
            //                    thisObj.TriggerEvent('oneditsaved');
            //                    GPS.Loading.hide();
            //                });

            var rd = [];
            for (var i = 0; i < radiuses.length; i++) {
                rd.push(radiuses[i].Id + "|" + radiuses[i].IsDisplay + "|" + radiuses[i].Length + "|" + radiuses[i].LengthMeasuresId);
            }
            $.ajax({
                type: "get",
                url: "Handler/ChangeAddressRadius.ashx",
                data: { radiuses: rd.join("^"), addressid: this._address.Id, latitude: this._center.Latitude, longitude: this._center.Longitude },
                dataType: "json",
                success: function(ret, textStatus) {
                    for (var i in ret) {
                        for (j in thisObj._address.Radiuses) {
                            if (thisObj._address.Radiuses[j].Id == ret[i].Id) {
                                thisObj._address.Radiuses[j].Relations = ret[i].Relations;
                            }
                        }
                    }
                    thisObj.TriggerEvent('oneditsaved');
                    GPS.Loading.hide();
                }
            });
        }
        else {
            GPS.Loading.hide();
            GPSAlert("There were no changes need to be saved!");
        }


        //        this._address = null; 
    },
    // initalization method
    _InitAddressFormViewBase: function(div) {
        var thisObj = this;
        $("#baundary_create_form a").attr("onclick", "");
        $("#button-save-radius-save").click(function() {
            thisObj._OnSave();
        });
        $("#button-save-radius-reset").click(function() {
            thisObj.BindAddress(thisObj._address, thisObj._addressShape);
            thisObj._addressShape.ChangeCircles({
                RadiusId: thisObj._address.Radiuses[0].Id,
                Center: thisObj._center,
                Length: thisObj._radius0,
                LengthUnit: thisObj._radiusUnit
            });
            thisObj._addressShape.ChangeCircles({
                RadiusId: thisObj._address.Radiuses[1].Id,
                Center: thisObj._center,
                Length: thisObj._radius1,
                LengthUnit: thisObj._radiusUnit
            });
            thisObj._addressShape.ChangeCircles({
                RadiusId: thisObj._address.Radiuses[2].Id,
                Center: thisObj._center,
                Length: thisObj._radius2,
                LengthUnit: thisObj._radiusUnit
            });
        });
        $("#button-save-radius-cancel").click(function() {
            thisObj.BindAddress(thisObj._address, thisObj._addressShape);
            thisObj._addressShape.ChangeCircles({
                RadiusId: thisObj._address.Radiuses[0].Id,
                Center: thisObj._center,
                Length: thisObj._radius0,
                LengthUnit: thisObj._radiusUnit
            });
            thisObj._addressShape.ChangeCircles({
                RadiusId: thisObj._address.Radiuses[1].Id,
                Center: thisObj._center,
                Length: thisObj._radius1,
                LengthUnit: thisObj._radiusUnit
            });
            thisObj._addressShape.ChangeCircles({
                RadiusId: thisObj._address.Radiuses[2].Id,
                Center: thisObj._center,
                Length: thisObj._radius2,
                LengthUnit: thisObj._radiusUnit
            });
            thisObj.TriggerEvent('oneditcancel');
        });

        $("#button-center-lat-up").click(function() {
            thisObj._center.Latitude += 0.001;
            $('#address_lat').html(thisObj._center.Latitude.toFixed(3));
            thisObj._addressShape.ChangeCircles({ Center: thisObj._center });
        });
        $("#button-center-lat-down").click(function() {
            thisObj._center.Latitude -= 0.001;
            $('#address_lat').html(thisObj._center.Latitude.toFixed(3));
            thisObj._addressShape.ChangeCircles({ Center: thisObj._center });
        });
        $("#button-center-lon-up").click(function() {
            thisObj._center.Longitude += 0.001;
            $('#address_lon').html(thisObj._center.Longitude.toFixed(3));
            thisObj._addressShape.ChangeCircles({ Center: thisObj._center });
        });
        $("#button-center-lon-down").click(function() {
            thisObj._center.Longitude -= 0.001;
            $('#address_lon').html(thisObj._center.Longitude.toFixed(3));
            thisObj._addressShape.ChangeCircles({ Center: thisObj._center });
        });
        $("#baundary_create_form input").attr("onblur", "");

        $('#rdMile').click(function() {
            if (thisObj._radiusUnit != 1) {
                thisObj._radiusUnit = 1;
                thisObj._addressShape.ChangeCircles({ LengthUnit: thisObj._radiusUnit });
            }
        });

        $('#rdKM').click(function() {
            if (thisObj._radiusUnit != 2) {
                thisObj._radiusUnit = 2;
                thisObj._addressShape.ChangeCircles({ LengthUnit: thisObj._radiusUnit });
            }
        });

        $("#radius0").change(function() {
            var tmpVal = $("#radius0").val();
            if (tmpVal > 0 && tmpVal < thisObj._radius1) {
                thisObj._radius0 = Number(tmpVal);
                $('#slider0').slider('value', tmpVal);
                thisObj._addressShape.ChangeCircles({
                    RadiusId: thisObj._address.Radiuses[0].Id,
                    Length: thisObj._radius0
                });
            }
            else {
                $("#radius0").val(thisObj._radius0);
            }
        });
        $("#radius1").change(function() {
            var tmpVal = $("#radius1").val();
            if (thisObj._radius0 < tmpVal && tmpVal < thisObj._radius2) {
                thisObj._radius1 = Number(tmpVal);
                $('#slider1').slider('value', tmpVal);
                thisObj._addressShape.ChangeCircles({
                    RadiusId: thisObj._address.Radiuses[1].Id,
                    Length: thisObj._radius1
                });
            }
            else {
                $("#radius1").val(thisObj._radius1);
            }
        });
        $("#radius2").change(function() {
            var tmpVal = $("#radius2").val();
            if (tmpVal > thisObj._radius1) {
                thisObj._radius2 = Number(tmpVal);
                $('#slider2').slider('value', tmpVal);
                thisObj._addressShape.ChangeCircles({
                    RadiusId: thisObj._address.Radiuses[2].Id,
                    Length: thisObj._radius2
                });
            }
            else {
                $("#radius2").val(thisObj._radius2);
            }
        });

        $("#baundary_create_form input").attr("onclick", "");

        $('#slider0').slider({
            range: "min",
            value: 0,
            min: 0,
            max: 30,
            step: 0.5,
            slide: function(event, ui) {
                $('#radius0').val(Number(ui.value).toFixed(2));
                thisObj._addressShape.ChangeCircles({
                    RadiusId: thisObj._address.Radiuses[0].Id,
                    Length: Number(ui.value)
                });
            },
            change: function(event, ui) {
                var tmpVal = Number(ui.value);
                if (Number(ui.value) > 0 && tmpVal < thisObj._radius1) {
                    if (thisObj._radius0 != tmpVal) {
                        thisObj._radius0 = tmpVal;
                        $('#radius0').val(thisObj._radius0.toFixed(2));
                        thisObj._addressShape.ChangeCircles({
                            RadiusId: thisObj._address.Radiuses[0].Id,
                            Length: thisObj._radius0
                        });
                    }
                } else {
                    $('#slider0').slider('value', thisObj._radius0);
                    $('#radius0').val(thisObj._radius0.toFixed(2));
                    thisObj._addressShape.ChangeCircles({
                        RadiusId: thisObj._address.Radiuses[0].Id,
                        Length: thisObj._radius0
                    });
                }
            }
        });
        $('#slider1').slider({
            range: "min",
            value: 0,
            min: 0,
            max: 30,
            step: 0.5,
            slide: function(event, ui) {
                $('#radius1').val(Number(ui.value).toFixed(2));
                thisObj._addressShape.ChangeCircles({
                    RadiusId: thisObj._address.Radiuses[1].Id,
                    Length: Number(ui.value)
                });
            },
            change: function(event, ui) {
                var tmpVal = Number(ui.value);
                if (thisObj._radius0 < tmpVal && tmpVal < thisObj._radius2) {
                    if (thisObj._radius1 != tmpVal) {
                        thisObj._radius1 = tmpVal;
                        $('#radius1').val(thisObj._radius1.toFixed(2));
                        thisObj._addressShape.ChangeCircles({
                            RadiusId: thisObj._address.Radiuses[1].Id,
                            Length: thisObj._radius1
                        });
                    }
                } else {
                    $('#slider1').slider('value', thisObj._radius1);
                    $('#radius1').val(thisObj._radius1.toFixed(2));
                    thisObj._addressShape.ChangeCircles({
                        RadiusId: thisObj._address.Radiuses[1].Id,
                        Length: thisObj._radius1
                    });
                }
            }
        });
        $('#slider2').slider({
            range: "min",
            value: 0,
            min: 0,
            max: 30,
            step: 0.5,
            slide: function(event, ui) {
                $('#radius2').val(Number(ui.value).toFixed(2));
                thisObj._addressShape.ChangeCircles({
                    RadiusId: thisObj._address.Radiuses[2].Id,
                    Length: Number(ui.value)
                });
            },
            change: function(event, ui) {
                var tmpVal = Number(ui.value);
                if (tmpVal > thisObj._radius1) {
                    if (thisObj._radius2 != tmpVal) {
                        thisObj._radius2 = tmpVal;
                        $('#radius2').val(thisObj._radius2.toFixed(2));
                        thisObj._addressShape.ChangeCircles({
                            RadiusId: thisObj._address.Radiuses[2].Id,
                            Length: thisObj._radius2
                        });
                    }
                } else {
                    $('#slider2').slider('value', thisObj._radius2);
                    $('#radius2').val(thisObj._radius2.toFixed(2));
                    thisObj._addressShape.ChangeCircles({
                        RadiusId: thisObj._address.Radiuses[2].Id,
                        Length: thisObj._radius2
                    });
                }
            }
        });
    },



    BindCenter: function() {
        $('#address_lat').html(this._center.Latitude.toFixed(3));
        $('#address_lon').html(this._center.Longitude.toFixed(3));
    },

    BindRadiuses: function() {
        $('#radius0').val(this._radius0.toFixed(2));
        $('#radius1').val(this._radius1.toFixed(2));
        $('#radius2').val(this._radius2.toFixed(2));
        $('#slider0').slider('value', this._radius0);
        $('#slider1').slider('value', this._radius1);
        $('#slider2').slider('value', this._radius2);
        if (this._radiusUnit == 2) {
            $('#rdKM').attr("checked", true);
            $('#rdMile').attr("checked", false);
        }
        else {
            $('#rdKM').attr("checked", false);
            $('#rdMile').attr("checked", true);
        }
    },

    BindAddress: function(address, addressShape) {
        this._address = address;
        this._addressShape = addressShape;
        $('#address_title').val(address.Street + ", " + address.ZipCode);
        this._center = new VELatLong(address.Latitude, address.Longitude);
        address.Radiuses.sort(function(a, b) { return a.Length - b.Length });
        this._radius0 = address.Radiuses[0].Length;
        this._radius1 = address.Radiuses[1].Length;
        this._radius2 = address.Radiuses[2].Length; ;
        this._radiusUnit = address.Radiuses[0].LengthMeasuresId;

        this.BindCenter();
        this.BindRadiuses();
    },

    Show: function() {
        $('#baundary_create_form').show();
    },

    Hide: function() {
        $('#baundary_create_form').hide();
    }

});


GPS.AddressMapView = GPS.EventTrigger.extend({   // initalization method - this function is invoked dynamic when new instance
    init: function(options) {
        this._super(options);
        this._Define();
        this._InitAddressMapViewBase(options);
    },
    // define basic attributes
    _Define: function() {
        this._mapObj = null;
        this._shapeLayer = null;
        this._addressShapes = null;
        this._iconSize = { width: 26, height: 26 };
    },
    // initalization method
    _InitAddressMapViewBase: function(options) {
        var thisObj = this;
        this._mapObj = options.MapObj;
        this._shapeLayer = new VEShapeLayer();
        this._mapObj.AddShapeLayer(this._shapeLayer);
        this._addressShapes = new GPS.QArray();
        this._mapObj.AttachEvent("onchangeview", function() {
            thisObj._OnChangeZoomLevel();
        });
        this._OnChangeZoomLevel();
    },

    _OnChangeZoomLevel: function() {
        var zoomLevel = this._mapObj.GetZoomLevel();
        var value = (zoomLevel - 8) * 12;
        if (value < 0) value = 1;
        this._iconSize.width = value;
        this._iconSize.height = value;
        var iconSize = this._iconSize;
        this._addressShapes.Each(function(i, shape) {
            shape.ChangeSize(iconSize);
        });
    },

    SetCenter: function(address) {
        this._mapObj.SetCenter(new VELatLong(address.OriginalLatitude, address.OriginalLongitude));
    },

    Append: function(address) {
        var shape = new GPS.AddressMapShape({
            Address: address,
            IconSize: this._iconSize
        });
        shape.AddToLayer(this._shapeLayer);
        this._addressShapes.Set(address.Id, shape);
    },
    GetAddressShape: function(addressId) {
        return this._addressShapes.Get(addressId);
    },
    DeleteAddressShape: function(addressId) {
        var shape = this._addressShapes.Get(addressId);
        if (shape) {
            shape.DeleteFromLayer(this._shapeLayer);
            this._addressShapes.Set(addressId, null);
        }
    }
});

GPS.AddressMapShape = GPS.EventTrigger.extend({   // initalization method - this function is invoked dynamic when new instance
    init: function(options) {
        this._super(options);
        this._Define();
        this._InitAddressMapShapeBase(options);
    },
    // define basic attributes
    _Define: function() {
        this._address = null;
        this._icon = null;
        this._iconProvider = null;
        this._circles = null;
    },
    // initalization method
    _InitAddressMapShapeBase: function(options) {
        this._address = options.Address;
        this._iconProvider = new GPS.AddressIconProvider();
        this._InitIcon(options.Address, options.IconSize);
        this._InitCircles(options.Address);

    },

    _InitCircles: function(address) {
        this._circles = new GPS.QArray();
        var center = new VELatLong(address.Latitude, address.Longitude);
        for (var i in address.Radiuses) {
            var circle = new GPS.AddressCircleShape({
                Center: center,
                Length: address.Radiuses[i].Length,
                LengthUnit: address.Radiuses[i].LengthMeasuresId
            });
            circle.SetVisable(address.Radiuses[i].IsDisplay);
            
            this._circles.Set(address.Radiuses[i].Id, circle);
        }

    },

    _InitIcon: function(address, iconSize) {
        var origin = new VELatLong(address.OriginalLatitude, address.OriginalLongitude);
        var iconUrl = this._iconProvider.GetIconUrl(iconSize, address.Color.toLowerCase());
        this._icon = new VEShape(VEShapeType.Pushpin, origin);
        this._icon.SetCustomIcon(iconUrl);
        this._icon.SetDescription([address.Street, address.ZipCode].join(', '));
        //        icon.TimmAddressId = address.Id;
        //        icon.TimmColor = address.Color;
        //        icon.TimmOrigin = origin;
    },
    ChangeSize: function(iconSize) {
        var iconUrl = this._iconProvider.GetIconUrl(iconSize, this._address.Color.toLowerCase());
        this._icon.SetCustomIcon(iconUrl);
    },

    AddToLayer: function(layer) {
        layer.AddShape(this._icon);
        this._circles.Each(function(i, circle) {
            circle.AddToLayer(layer);
        });
    },

    DeleteFromLayer: function(layer) {
        layer.DeleteShape(this._icon);
        this._circles.Each(function(i, circle) {
            circle.DeleteFromLayer(layer);
        });
    },

    ChangeCircles: function(options) {
        if (options.RadiusId != null) {
            var circle = this._circles.Get(options.RadiusId);
            circle.Change(options);
        }
        else {
            this._circles.Each(function(i, circle) {
                circle.Change(options);
            });
        }
    },

    SetCircleVisable: function(radiusId, isVisable) {
        var circle = this._circles.Get(radiusId);
        if (circle) {
            circle.SetVisable(isVisable);
        }
    }
});

GPS.AddressIconProvider = function() {
    this._iconUrls['red'] = 'images/pushpins/red-star.png';
    this._iconUrls['green'] = 'images/pushpins/green-star.png';
}

GPS.AddressIconProvider.prototype = {
    _iconUrls: [],

    GetIconUrl: function(iconSize, colorName) {
        var baseWidth = 26, baseHeight = 26;
        var iconUrl = '<img src="{url}" style="margin:0;border:none;position:relative;top:{top}px;left:{left}px;width:{width}px;height:{height}px;" />';
        iconUrl = iconUrl.replace('{url}', this._iconUrls[colorName]);
        iconUrl = iconUrl.replace('{width}', iconSize.width);
        iconUrl = iconUrl.replace('{height}', iconSize.height);
        iconUrl = iconUrl.replace('{top}', (baseHeight - iconSize.height) / 2);
        iconUrl = iconUrl.replace('{left}', (baseWidth - iconSize.width) / 2);
        return iconUrl;
    }
}


GPS.AddressCircleShape = GPS.EventTrigger.extend({   // initalization method - this function is invoked dynamic when new instance
    init: function(options) {
        this._super(options);
        this._Define();
        this._InitAddressCircleShapeBase(options);
    },
    // define basic attributes
    _Define: function() {
        this._circle = null;
        this._circleMarks = null;
        this._circlePoints = null;
        this._circleMarkPoints = null;
        this._center = null;
        this._length = null;
        this._unit = null;
    },
    // initalization method
    _InitAddressCircleShapeBase: function(options) {
        this._center = options.Center;
        this._length = options.Length;
        this._unit = options.LengthUnit;
        this._GetCirclePoints(this._center, this._length, this._unit);
        this._circle = new VEShape(VEShapeType.Polyline, this._circlePoints);
        this._circle.SetLineColor(new VEColor(0, 0, 139, 0.5));
        this._circle.HideIcon();
        this._circleMarks = new GPS.QArray();
        var dmark = "";
        if (this._unit == 2) {
            dmark = Number(this._length).toFixed(1) + "K";
        }
        else {
            dmark = Number(this._length).toFixed(1) + "M";
        }
        var markIconFormat = '<span class="radiimark" style="position: relative; color: darkblue; font-size:12px;font-weight:bold; left: {1}px; top: {2}px;">{0}</span>';
        for (var i in this._circleMarkPoints) {
            var markShape = new VEShape(VEShapeType.Pushpin, this._circleMarkPoints[i]);
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
            this._circleMarks.Set(i, markShape);
        }
    },

    Change: function(options) {
        if (options.Center) {
            this._center = options.Center;
        }
        if (options.Length) {
            this._length = options.Length;
        }
        if (options.LengthUnit) {
            this._unit = options.LengthUnit;
        }
        this._GetCirclePoints(this._center, this._length, this._unit);
        this._circle.SetPoints(this._circlePoints);
        var dmark = "";
        if (this._unit == 2) {
            dmark = Number(this._length).toFixed(1) + "K";
        }
        else {
            dmark = Number(this._length).toFixed(1) + "M";
        }
        var markIconFormat = '<span class="radiimark" style="position: relative; color: #f905e5; font-size:12px;font-weight:bold; left: {1}px; top: {2}px;">{0}</span>';
        for (var i in this._circleMarkPoints) {
            var markShape = this._circleMarks.Get(i);
            markShape.SetPoints(this._circleMarkPoints[i]);
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
        }
    },

    _GetCirclePoints: function(origin, radius, unit) {
        this._circlePoints = [];
        this._circleMarkPoints = [];

        var earthRadius = 3959;
        if (unit == 2) { earthRadius = 6371; }

        //latitude in radius
        var lat = (origin.Latitude * Math.PI) / 180;

        //longitude in radius
        var lon = (origin.Longitude * Math.PI) / 180;

        //angular distance covered on earth's surface
        var d = parseFloat(radius) / earthRadius;

        for (i = 0; i <= 360; i++) {
            var point = new VELatLong(0, 0)
            var bearing = i * Math.PI / 180; //rad
            point.Latitude = Math.asin(Math.sin(lat) * Math.cos(d) + Math.cos(lat) * Math.sin(d) * Math.cos(bearing));
            point.Longitude = ((lon + Math.atan2(Math.sin(bearing) * Math.sin(d) * Math.cos(lat), Math.cos(d) - Math.sin(lat) * Math.sin(point.Latitude))) * 180) / Math.PI;
            point.Latitude = (point.Latitude * 180) / Math.PI;
            this._circlePoints.push(point);
            if (i % 90 == 0 && i < 360) {
                this._circleMarkPoints.push(point);
            }            
        }
    },
    AddToLayer: function(layer) {
        layer.AddShape(this._circle);
        this._circleMarks.Each(function(i, mark) {
            layer.AddShape(mark);
        });
    },
    DeleteFromLayer: function(layer) {
        layer.DeleteShape(this._circle);
        this._circleMarks.Each(function(i, mark) {
            layer.DeleteShape(mark);
        });
    },
    SetVisable: function(isVisable) {
        if (isVisable) {
            this._circle.Show();
            this._circleMarks.Each(function(i, mark) {
                mark.Show();
            });
        } else {
            this._circle.Hide();
            this._circleMarks.Each(function(i, mark) {
                mark.Hide();
            });
        }
    }
});