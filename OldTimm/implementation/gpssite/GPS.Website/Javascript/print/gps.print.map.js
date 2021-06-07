

PrintSettings = {
    ShowLocation: true,
    ShowRadii: true,
    ShowNonDeliverables: true,
    ShowSubMaps: true,
    ShowSubMapCount: true,
    ShowClassificationOutlines: true,
    ShowPenetrationColors: false,
    ChangePenetrationColor: false
}


/*
* Class PrintSubMap.
*/
PrintSubMap = function(div, submap) {
    this._prefix = div;
}

PrintSubMap.prototype = {
    __Init__: function(div, submap) {

    }
}


/*
* Class PrintMap.
*/
PrintMap = function(div) {
    this._div = null;
    this._map = null;
    this._addresses = null;
    this._box = null;
    this._locationLayer = null;
    this._radiiLayer = null;
    this._submapLayer = null;
    this._areaLayer = null;
    this._ndLayer = null;
    this._ogdLayer = null;
    this._agdLayer = null;
    this.__Init__(div);
}

PrintMap.prototype = {
    __Init__: function(div) {
        this._div = div;
        this._box = [];
    },

    LoadMap: function() {
        this._map = new VEMap(this._div);
        var bingMapKey = $("#bingMapKey").val();
        if (!bingMapKey) {
            alert("Please add bing map key");
        }
        this._map.SetCredentials(bingMapKey);
        this._map.LoadMap();
        this._map.HideDashboard();

        this._locationLayer = new VEShapeLayer();
        this._map.AddShapeLayer(this._locationLayer);
        this._radiiLayer = new VEShapeLayer();
        this._map.AddShapeLayer(this._radiiLayer);
        this._submapLayer = new VEShapeLayer();
        //this._submapLayer.Hide();
        this._map.AddShapeLayer(this._submapLayer);
        this._areaLayer = new VEShapeLayer();
        this._map.AddShapeLayer(this._areaLayer);
        this._ndLayer = new VEShapeLayer();
        this._map.AddShapeLayer(this._ndLayer);
        this._ogdLayer = new VEShapeLayer();
        this._map.AddShapeLayer(this._ogdLayer);
        this._agdLayer = new VEShapeLayer();
        this._map.AddShapeLayer(this._agdLayer);
    },

    __DrawCircle__: function(origin, radius, unit) {
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
    },

    HideLocations: function() { this._locationLayer.Hide(); },

    ShowLocations: function() { this._locationLayer.Show(); },

    HideRadii: function() { this._radiiLayer.Hide(); },

    ShowRadii: function() { this._radiiLayer.Show(); },

    HideClassificationOutLines: function() {
        this._submapLayer.Show();
        this._areaLayer.Hide();
    },

    ShowClassificationOutLines: function() {
        this._submapLayer.Hide();
        this._areaLayer.Show();
    },

    ShowNonDeliverables: function() {
        this._ndLayer.Show();
    },

    HideNonDeliverables: function() {
        this._ndLayer.Hide();
    },

    ShowGTUDots: function() {
        this._ogdLayer.Show();
    },

    HideGTUDots: function() {
        this._ogdLayer.Hide();
    },

    ShowAddedGTUDots: function() {
        this._agdLayer.Show();
    },

    HideAddedGTUDots: function() {
        this._agdLayer.Hide();
    },

    __DrawLocation__: function(address) {
        var text = address.Address1 + ' , ' + address.ZipCode;
        var image = address.Color == "red" ? "red-star.png" : "green-star.png";
        var shape = new VEShape(VEShapeType.Pushpin, new VELatLong(address.OriginalLatitude, address.OriginalLongitude));
        shape.LocationColor = address.Color;
        shape.SetCustomIcon("<span style='font-family:Arial; font-size:x-small; color:Black;'>" +
                   "<img src='images/pushpins/" + image + "' style='position:relative;width:30px;height:30px;'/></span>");
        this._locationLayer.AddShape(shape);

    },

    SetAddresses: function(addresses) {
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

    },

    __ParseSubMapShape__: function(submap, number) {
        var points = [];
        var i = 0;
        var length = submap.SubMapCoordinates.length;
        while (i < length) {
            points.push(new VELatLong(submap.SubMapCoordinates[i].Latitude, submap.SubMapCoordinates[i].Longitude));
            i++;
        }
        var shape = new VEShape(VEShapeType.Polygon, points);
        var color = PenetrationColorSettings.GetColor(-1);
        shape.SetFillColor(new VEColor(color.R, color.G, color.B, color.A));
        if (number > 0) { shape.SetCustomIcon('<div class="areaicon">' + number + '</div>'); }
        else { shape.HideIcon(); }
        return shape;
    },

    __ParseDMShape__: function(dm, number) {
        var points = [];
        var i = 0;
        var length = dm.DistributionMapCoordinates.length;
        if (length == 0) return null;
        while (i < length) {
            points.push(new VELatLong(dm.DistributionMapCoordinates[i].Latitude, dm.DistributionMapCoordinates[i].Longitude));
            i++;
        }
        var shape = new VEShape(VEShapeType.Polygon, points);
        var color = PenetrationColorSettings.GetColor(-1, false, true);
        shape.SetFillColor(new VEColor(color.R, color.G, color.B, 0.2));
        if (number > 0) { shape.SetCustomIcon('<div class="areaicon">' + number + '</div>'); }
        else { shape.HideIcon(); }
        return shape;
    },

    __ParseAreaShape__: function(area, number) {
        var points = [];
        var i = 0;
        var length = area.Coordinates.length;
        while (i < length) {
            points.push(new VELatLong(area.Coordinates[i].Latitude, area.Coordinates[i].Longitude));
            i++;
        }
        var shape = new VEShape(VEShapeType.Polygon, points);
        var color = PenetrationColorSettings.GetColor(-1, false, !area.IsEnabled);

        shape.SetFillColor(new VEColor(color.R, color.G, color.B, color.A));
        if (area.IsEnabled) {
            shape.SetCustomIcon('<div class="areaicon">' + number + '</div>');
        }
        else {
            shape.SetLineColor(new VEColor(color.R, color.G, color.B, color.A));
            shape.HideIcon();
        }
        return shape;
    },

    AddCampaignSubMaps: function(submaps) {
        this.HideClassificationOutLines();
        var points = [];
        var i = 0;
        var length = submaps.length;
        while (i < length) {
            if (submaps[i].SubMapCoordinates.length > 0) {
                var shape = this.__ParseSubMapShape__(submaps[i], i + 1);
                submaps[i].CampaignShapeObj = shape;
                this._submapLayer.AddShape(shape);
                points = points.concat(shape.GetPoints());
            }
            i++;
        }
        if (points.length > 0) { this._map.SetMapView(points); }
    },

    AddCampaignDM: function(submaps) {
        this.HideClassificationOutLines();
        var points = [];
        var i = 0;
        var length = submaps.length;
        while (i < length) {
            if (submaps[i].SubMapCoordinates.length > 0) {
                var shape = this.__ParseSubMapShape__(submaps[i], i + 1);
                submaps[i].CampaignShapeObj = shape;
                this._submapLayer.AddShape(shape);
                points = points.concat(shape.GetPoints());

                var len = submaps[i].DistributionMaps.length;
                var j = 0;
                while (j < len) {
                    var shape = this.__ParseDMShape__(submaps[i].DistributionMaps[j], j + 1);
                    //submaps[i].DistributionMaps[j].CampaignShapeObj = shape;
                    if (shape)
                        this._submapLayer.AddShape(shape);

                    j++;
                }
            }
            i++;
        }
        if (points.length > 0) { this._map.SetMapView(points); }
    },

    AddSubMap: function(submap) {
        if (submap.SubMapCoordinates.length > 0) {
            var points = [];
            var shape = this.__ParseSubMapShape__(submap, 0);
            submap.ShapeObj = shape;
            this._submapLayer.AddShape(shape);
            points = shape.GetPoints();
            this._map.SetMapView(points);
        }
    },

    AddDM: function(dm) {
        if (dm.DistributionMapCoordinates.length > 0) {
            var points = [];
            var shape = this.__ParseDMShape__(dm, 0);
            dm.ShapeObj = shape;
            this._submapLayer.AddShape(shape);
            points = shape.GetPoints();
            this._map.SetMapView(points);
        }
    },

    AddDMAreas: function(areas) {
        var fiveZips = areas.FiveZipAreas;
        var croutes = areas.CRoutes;
        var trks = areas.Tracts;
        var bgs = areas.BlockGroups;
        //FiveZip
        var j = 1;
        var i = 0;
        var length = fiveZips.length;
        while (i < length) {
            var area = fiveZips[i];
            var shape = this.__ParseAreaShape__(area, j);
            area.ShapeObj = shape;
            if (area.IsEnabled) {
                this._areaLayer.AddShape(shape);
                j++;
            }
            else {
                this._ndLayer.AddShape(shape);
            }
            i++;
        }
        //CRoute
        i = 0;
        length = croutes.length;
        while (i < length) {
            var area = croutes[i];
            var shape = this.__ParseAreaShape__(area, j);
            area.ShapeObj = shape;
            if (area.IsEnabled) {
                this._areaLayer.AddShape(shape);
                j++;
            }
            else {
                this._ndLayer.AddShape(shape);
            }
            i++;
        }
        //Tract
        i = 0;
        length = trks.length;
        while (i < length) {
            var area = trks[i];
            var shape = this.__ParseAreaShape__(area, j);
            area.ShapeObj = shape;
            if (area.IsEnabled) {
                this._areaLayer.AddShape(shape);
                j++;
            }
            else {
                this._ndLayer.AddShape(shape);
            }
            i++;
        }
        //BG
        i = 0;
        length = bgs.length;
        while (i < length) {
            var area = bgs[i];
            var shape = this.__ParseAreaShape__(area, j);
            area.ShapeObj = shape;
            if (area.IsEnabled) {
                this._areaLayer.AddShape(shape);
                j++;
            }
            else {
                this._ndLayer.AddShape(shape);
            }
            i++;
        }
    },

    AddSubMapAreas: function(areas) {
        var fiveZips = areas.FiveZipAreas;
        var croutes = areas.CRoutes;
        var trks = areas.Tracts;
        var bgs = areas.BlockGroups;
        //FiveZip
        var j = 1;
        var i = 0;
        var length = fiveZips.length;
        while (i < length) {
            var area = fiveZips[i];
            var shape = this.__ParseAreaShape__(area, j);
            area.ShapeObj = shape;
            if (area.IsEnabled) {
                this._areaLayer.AddShape(shape);
                j++;
            }
            else {
                this._ndLayer.AddShape(shape);
            }
            i++;
        }
        //CRoute
        i = 0;
        length = croutes.length;
        while (i < length) {
            var area = croutes[i];
            var shape = this.__ParseAreaShape__(area, j);
            area.ShapeObj = shape;
            if (area.IsEnabled) {
                this._areaLayer.AddShape(shape);
                j++;
            }
            else {
                this._ndLayer.AddShape(shape);
            }
            i++;
        }
        //Tract
        i = 0;
        length = trks.length;
        while (i < length) {
            var area = trks[i];
            var shape = this.__ParseAreaShape__(area, j);
            area.ShapeObj = shape;
            if (area.IsEnabled) {
                this._areaLayer.AddShape(shape);
                j++;
            }
            else {
                this._ndLayer.AddShape(shape);
            }
            i++;
        }
        //BG
        i = 0;
        length = bgs.length;
        while (i < length) {
            var area = bgs[i];
            var shape = this.__ParseAreaShape__(area, j);
            area.ShapeObj = shape;
            if (area.IsEnabled) {
                this._areaLayer.AddShape(shape);
                j++;
            }
            else {
                this._ndLayer.AddShape(shape);
            }
            i++;
        }
    },

    GetMapImagesStr: function() {
        var mapDivId = this._div;
        var imgs = [];
        var images = $('#' + mapDivId + ' .MSVE_ImageTile');
        var left = Number($('#' + mapDivId + ' .MSVE_Map').css('left').replace('px', ''));
        if (!left) { left = 0; }
        var top = Number($('#' + mapDivId + ' .MSVE_Map').css('top').replace('px', ''));
        if (!top) { top = 0; }
        $.each(images, function(i, image) {
            var items = [];
            items.push($(image).attr('src'));
            items.push(Number($(image).css('left').replace('px', '')) + left);
            items.push(Number($(image).css('top').replace('px', '')) + top);
            imgs.push(items.join(','));

        });
        return imgs.join(';');
    },

    GetMapPushpinsStr: function() {
        var mapDivId = this._div;
        var items = [];
        var pushpins = $('#' + mapDivId + ' .VEAPI_Pushpin');
        var left = Number($('#' + mapDivId + ' .MSVE_Map').css('left').replace('px', ''));
        if (!left) { left = 0; }
        var top = Number($('#' + mapDivId + ' .MSVE_Map').css('top').replace('px', ''));
        if (!top) { top = 0; }
        $.each(pushpins, function(i, pushpin) {
            var arr = [];
            if ($(pushpin).find('.radiimark').text() != '') {
                var mark = $(pushpin).find('.radiimark');
                arr.push($(pushpin).find('.radiimark').text());
                arr.push(Number($(pushpin).css('left').replace('px', '')) + left + Number((mark).css('left').replace('px', '')));
                arr.push(Number($(pushpin).css('top').replace('px', '')) + top + Number((mark).css('top').replace('px', '')));
                arr.push("radiimark");
                items.push(arr.join(','));
            }
            else if ($(pushpin).find('.areaicon').text() != '') {
                arr.push($(pushpin).find('.areaicon').text());
                arr.push(Number($(pushpin).css('left').replace('px', '')) + left);
                arr.push(Number($(pushpin).css('top').replace('px', '')) + top);
                arr.push("areaicon");
                items.push(arr.join(','));
            }
        });
        return items.join(';');
    },

    GetScaleStr: function() {
        var mapDivId = this._div;
        var items = [];
        items.push($('#' + mapDivId + ' .MSVE_ScaleBarLabelBg').text());
        items.push($('#' + mapDivId + ' .MSVE_ScaleBarBg').css('width').replace('px', ''));
        //        items.push($(bg).css('bottom').replace('px', ''));
        return items.join(',');
    },

    __LatLongToPixel__: function(latlong) { return this._map.LatLongToPixel(latlong); },

    GetLocationsStr: function() {
        var layer = this._locationLayer;
        var items = [];
        if (layer.IsVisible()) {
            var j = 0;
            var sCount = layer.GetShapeCount();
            while (j < sCount) {
                var shape = layer.GetShapeByIndex(j);
                var point = this.__LatLongToPixel__(shape.GetPoints()[0]);
                items.push(point.x + ',' + point.y + ',' + shape.LocationColor);
                j++;
            }
        }
        return items.join(';');
    },

    GetNdStr: function() {
        var layer = this._ndLayer;
        var items = [];
        if (layer.IsVisible()) {
            var j = 0;
            var sCount = layer.GetShapeCount();
            while (j < sCount) {
                var shape = layer.GetShapeByIndex(j);
                var p = new VELatLong(shape.Latitude, shape.Longitude)
                var point = this.__LatLongToPixel__(p);

                items.push(point.x + ',' + point.y + ',' + shape.LocationColor);
                j++;
            }
        }
        return items.join(';');
    },

    GetgdStr: function() {
        var olayer = this._ogdLayer;
        var alayer = this._agdLayer;
        var items = [];
        if (olayer.IsVisible()) {
            var j = 0;
            var sCount = olayer.GetShapeCount();
            while (j < sCount) {
                var shape = olayer.GetShapeByIndex(j);
                var p = new VELatLong(shape.Latitude, shape.Longitude)
                var point = this.__LatLongToPixel__(p);

                items.push(point.x + ',' + point.y + ',' + shape.LocationColor);
                j++;
            }
        }
        if (alayer.IsVisible()) {
            var k = 0;
            var sCount = alayer.GetShapeCount();
            while (k < sCount) {
                var shape = alayer.GetShapeByIndex(k);
                var p = new VELatLong(shape.Latitude, shape.Longitude)
                var point = this.__LatLongToPixel__(p);

                items.push(point.x + ',' + point.y + ',' + shape.LocationColor);
                k++;
            }
        }
        return items.join(';');
    },



    GetMapShapesStr: function() {
        var mapObj = this._map;
        var i = 0;
        var lCount = mapObj.GetShapeLayerCount();
        var shapes = [];
        while (i < lCount) {
            var layer = mapObj.GetShapeLayerByIndex(i);
            if (layer.IsVisible()) {
                var j = 0;
                var sCount = layer.GetShapeCount();
                while (j < sCount) {
                    var pts = [];
                    var shape = layer.GetShapeByIndex(j);
                    if (shape.GetType() == VEShapeType.Polygon || shape.GetType() == VEShapeType.Polyline) {
                        shapes.push(this.GetMapShapeStr(mapObj, shape));
                    }
                    j++;
                }
            }
            i++;
        }
        return shapes.join('$');
    },

    GetMapShapeStr: function(mapObj, shape) {
        var points = shape.GetPoints();
        var lineColor = shape.GetLineColor();
        var fillColor = shape.GetFillColor();
        var lineWidth = shape.GetLineWidth() + 1;
        var fillA = fillColor.A;
        if (shape.GetType() == VEShapeType.Polyline) {
            fillA = 0;
        }
        var items = [];
        items.push(this.GetShapePointsStr(mapObj, points));
        items.push(lineColor.R + ';' + lineColor.G + ';' + lineColor.B + ';' + lineColor.A);
        items.push(fillColor.R + ';' + fillColor.G + ';' + fillColor.B + ';' + fillA);
        items.push(lineWidth);
        return items.join('|');
    },

    GetShapePointsStr: function(mapObj, points) {
        var pts = [];
        var i = 0;
        var length = points.length;
        while (i < length) {
            var p = mapObj.LatLongToPixel(points[i]);
            pts.push(p.x + ',' + p.y);
            i++;
        }
        return pts.join(';');
    }

}