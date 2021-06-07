//
GPS.Map.Methods = {
    InArray: function(objects, object) {
        var inArray = false;
        for (var i = 0; i < objects.length; i++) {
            if (objects[i] == object) {
                inArray = true;
                break;
            }
        }
        return inArray;
    },

    GetBoxes: function(boxes, boxId) {
        var box = null;
        for (var i = 0; i < boxes.length; i++) {
            if (boxes[i].GetId() == boxId) {
                box = boxes[i];
                break;
            }
        }
        return box;
    }
}


GPS.Map.ShapeMethods = {

    AreaInArea: function(mArea, iArea) {
        return this.RectangleInRectangle(mArea.GetMinLatitude(), mArea.GetMaxLatitude(), mArea.GetMinLongitude(), mArea.GetMaxLongitude(),
		iArea.GetMinLatitude(), iArea.GetMaxLatitude(), iArea.GetMinLongitude(), iArea.GetMaxLongitude());
    },

    AreaInAreas: function(mAreas, iArea) {
        var inAreas = false;
        if (mAreas) {
            for (var i = 0; i < mAreas.length; i++) {
                if (this.AreaInArea(mAreas[i], iArea)) {
                    inAreas = true;
                    break;
                }
            }
        }
        return inAreas;
    },

    RectangleInRectangle: function(mMinLat, mMaxLat, mMinLon, mMaxLon, iMinLat, iMaxLat, iMinLon, iMaxLon) {
        if ((mMaxLat > iMinLat) && (mMinLon < iMaxLon) &&
            (mMinLat < iMaxLat) && (mMaxLon > iMinLon))
        { return true; }
        else { return false; }
    },

    GetBoxIds: function(minLat, maxLat, minLon, maxLon, mountLat, mountLon) {
        var ids = new Array();
        var lat = Math.floor(minLat * 100);
        var lon = Math.floor(minLon * 100);
        lat = lat - (lat % mountLat);
        if (lat < 0) { lat -= mountLat; }
        lon = lon - (lon % mountLon);
        if (lon < 0) { lon -= mountLon; }

        while (lat < maxLat * 100) {
            var tempLon = lon;
            while (tempLon < maxLon * 100) {
                ids.push(lat * 100000 + tempLon);
                tempLon += mountLon;
            }
            lat += mountLat;
        }
        return ids;
    },

    InLine: function(p1Lat, p1Lon, p2Lat, p2Lon, qLat, qLon) {
        if ((Math.min(p1Lat, p2Lat) <= qLat && Math.max(p1Lat, p2Lat) >= qLat)
         && (Math.min(p1Lon, p2Lon) <= qLon && Math.max(p1Lon, p2Lon) >= qLon)) {

            var latDistance = (p1Lat + (qLon - p1Lon) / (p2Lon - p1Lon) * (p2Lat - p1Lat)) - qLat;
            if (Math.abs(latDistance) <= 0.001) {
                return true;
            }
            else { return false; }
        }
        else { return false; }
    },

    PointEquals: function(p1, p2) {
        return p1.Latitude == p2.Latitude && p1.Longitude == p2.Longitude;
    },

    InPolygon: function(mCoordinates, coordinate) {
        var inPolygon = false;
        var i = 0;
        var length = mCoordinates.length;
        while (i < length) {
            var j = i + 1;
            if (j == length) { j = 0; }
            if (this.PointEquals(mCoordinates[i], coordinate)) {
                inPolygon = true;
                break;
            }
            else if (this.InLine(mCoordinates[i].Latitude, mCoordinates[i].Longitude,
             mCoordinates[j].Latitude, mCoordinates[j].Longitude,
             coordinate.Latitude, coordinate.Longitude)) {
                inPolygon = true;
                break;
            }

            i++;
        }
        return inPolygon;
    },

    PushOutCoordinates: function(outCoordinates, mCoordinates, iCoordinates) {
        //        var outCoordinates = [];
        var i = iCoordinates.length - 1;
        //        var length = iCoordinates.length;
        while (i >= 0) {
            if (!this.InPolygon(mCoordinates, iCoordinates[i])) {
                outCoordinates.push(iCoordinates[i]);
            }
            i--;
        }
        //        return outCoordinates;
    },

    GetInnerSeeks: function(mCoordinates, iCoordinates) {
        var array = [];
        var i = 0;
        var length = mCoordinates.length;
        var merged = false;
        while (i < length) {
            if (this.InPolygon(iCoordinates, mCoordinates[i])) {
                var j = array.length - 1;
                if (j < 0) {
                    array.push([i]);
                }
                else {
                    var k = array[j].length - 1;
                    if (array[j][k] == i - 1) { array[j].push(i); }
                    else { array.push([i]); }
                }
            }
            i++;
        }
        return array;
    },

    AddLineCoordinates: function(mCoordinates, iCoordinates) {
        var mi = 0;
        var mlength = mCoordinates.length;
        while (mi < mlength) {
            var mj = mi + 1;
        }
    },

    GetNewCoordinates: function(mCoordinates, iCoordinates) {
        var newCoordinates = [];
        var mi = 0;
        var mj = 1;
        var mlength = mCoordinates.length;
        while (mi < mlength) {
            mj = mi + 1;

        }
        return newCoordinates;
    },

    PointInLine: function(points, p1, p2) {


    },

    MergeCoordinates: function(mCoordinates, iCoordinates) {
        var miSeeks = this.GetInnerSeeks(mCoordinates, iCoordinates);
        var iiSeeks = this.GetInnerSeeks(iCoordinates, mCoordinates);
        var mergeCoordinates = null;
        var milength = miSeeks.length;
        var iilength = iiSeeks.length;
        if (milength > 0 && milength <= 2) {

            var mstart = miSeeks[0][miSeeks[0].length - 1];
            var mend;
            if (milength > 1) { mend = miSeeks[1][0] - 1; }
            else { mend = miSeeks[0][0] - 1; }

            mergeCoordinates = this.GetCoordinates(mCoordinates, mstart, mend);

            var istart = iiSeeks[0][iiSeeks[0].length - 1];
            var iend;
            if (iilength > 1) { iend = iiSeeks[1][0]; }
            else { iend = iiSeeks[0][0]; }

            var tiCoordinates = this.GetCoordinates(iCoordinates, istart, iend);
            var ti = 0;
            var tilen = tiCoordinates.length;
            while (ti < tilen) {
                mergeCoordinates.push(tiCoordinates[ti]);
                ti++;
            }
        }
        return mergeCoordinates;
    },

    GetCoordinates: function(oriCoordinates, start, end) {
        var i = start;
        var j = 0;
        var length = oriCoordinates.length;
        var clength = end > start ? end - start + 1 : end + length - start + 1;
        var coordinates = [];
        while (j < clength) {
            if (i == length) {
                i = 0;
            }
            if (i != length - 1) {
                coordinates.push(oriCoordinates[i]);
            }
            i++;
            j++;
        }
        return coordinates;
    },

    GetMergeCoordinates: function(mCoordinates, iCoordinates) {

        this.GetOutSeeks(mCoordinates, iCoordinates);
        var mergeCoordinates = [];
        var i = 0;
        var length = mCoordinates.length;
        var merged = false;
        while (i < length) {
            if (this.InPolygon(iCoordinates, mCoordinates[i]) && (!merged)) {
                this.PushOutCoordinates(mergeCoordinates, mCoordinates, iCoordinates);
                merged = true;
            }
            else {
                mergeCoordinates.push(mCoordinates[i]);
            }
            i++;
        }
        return mergeCoordinates;
    },

    MergeCoordinates1: function(mCoordinates, iCoordinates) {
        this.GetOutSeeks(mCoordinates, iCoordinates);
        var mergeCoordinates = [];
        var i = 0;
        var length = mCoordinates.length;
        var merged = false;
        while (i < length) {
            if (this.InPolygon(iCoordinates, mCoordinates[i]) && (!merged)) {
                this.PushOutCoordinates(mergeCoordinates, mCoordinates, iCoordinates);
                merged = true;
            }
            else {
                mergeCoordinates.push(mCoordinates[i]);
            }
            i++;
        }
        return mergeCoordinates;
    },

    UnMergeCoordinates: function(mCoordinates, iCoordinates) {
    },

    MergeAreas: function(recorder, retFun) {
        if (!this._areaCalculateService) {
            this._areaCalculateService = new TIMM.Website.CampaignServices.AreaCalculateService();
        }
        var campaignId = campaign ? campaign.Id : 0;
        this._areaCalculateService.MergeAreas(campaignId, recorder, retFun, null);
    }
}