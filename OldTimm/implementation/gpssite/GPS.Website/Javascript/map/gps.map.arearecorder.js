
GPS.Map.AreaRecorder = Class.extend({
    // initalization method - this function is invoked dynamic when new instance
    init: function() {
        this._Define();
        this._records = new GPS.QArray();
    },
    // define basic attributes
    _Define: function() {
        this._records = null;
    },
    //
    Clear: function() {
        this._records = new GPS.QArray();
    },
    // Record
    _Record: function(classification, areaId, shapeId, value, relations) {
        var record = this._GetRecord(classification, areaId, shapeId, relations);
        if ((record != null && (record.Value != value)) || ((record == null) && value == true)) {
            this._HardRecord(classification, areaId, shapeId, value, relations);
        }
        else {
            this._ClearSubRecords(classification, areaId, shapeId);
        }
    },
    // Clear Sub Records
    _ClearSubRecords: function(classification, areaId, shapeId) {
        var cls = [];
        if (classification < 3) {
            cls.push(3);
            if (classification < 2) {
                cls.push(2);
                cls.push(15);
                if (classification < 1) {
                    cls.push(1);
                }
            }
        }
        for (var t in cls) {
            var cId = cls[t];
            var cRecords = this._records.Get(cId);
            if (cRecords != null) {
                cRecords.Each(function(i, aRecords) {
                    aRecords.Each(function(j, sRecord) {
                        var sRelations = sRecord.Relations;
                        if ((typeof (sRelations[classification]) != "undefined") &&
                        (typeof (sRelations[classification][areaId]) != "undefined") &&
                        (typeof (sRelations[classification][areaId][shapeId]) != "undefined")) {
                            aRecords.Set(j, null);
                        }
                    });
                });
            }
        }
    },
    // new record
    _HardRecord: function(classification, areaId, shapeId, value, relations) {
        this._ClearSubRecords(classification, areaId, shapeId);
        var clsRecords = this._records.Get(classification);
        if (clsRecords == null) {
            clsRecords = new GPS.QArray();
            this._records.Set(classification, clsRecords);
        }
        var areaRecords = clsRecords.Get(areaId);
        if (areaRecords == null) {
            areaRecords = new GPS.QArray();
            clsRecords.Set(areaId, areaRecords);
        }
        areaRecords.Set(shapeId, { Id: shapeId, Value: value, Relations: relations });
    },
    //Get relations record
    _GetRelationsRecord: function(classification, areaId, shapeId, relations) {
        var record = null;
        for (var cId in relations) {
            var cRelations = relations[cId];
            for (var aId in cRelations) {
                var aRelations = cRelations[aId];
                for (var sId in aRelations) {
                    var sRelations = aRelations[sId];
                    if (typeof (sRelations[shapeId]) != "undefined") {
                        record = this._GetAreaShapeSingleRecord(cId, aId, sId);
                    }
                    if (record != null) {
                        break;
                    }
                }
                if (record != null) {
                    break;
                }
            }
            if (record != null) {
                break;
            }
        }
        return record;
    },
    // get area shape record
    _GetAreaShapeSingleRecord: function(classification, areaId, shapeId) {
        var record = null;
        var clsRecords = this._records.Get(classification);
        if (clsRecords != null) {
            var areaRecords = clsRecords.Get(areaId);
            if (areaRecords != null) {
                record = areaRecords.Get(shapeId);
            }
        }
        return record;
    },
    // get record
    _GetRecord: function(classification, areaId, shapeId, relations) {
        var record = this._GetAreaShapeSingleRecord(classification, areaId, shapeId);
        if (record == null) {
            record = this._GetRelationsRecord(classification, areaId, shapeId, relations);
        }
        return record;
    },
    // Get Area Record
    _GetSingleAreaRecords: function(classifation, areaId) {
        var clsRecords = this._GetClsRecords(classification);
        var areaRecords = clsRecords.Get(areaId);
        if (areaRecords == null) {
            areaRecords = new GPS.QArray();
            clsRecords.Set(areaId, areaRecords);
        }
        return areaRecords;
    },
    // Get Classification Records
    _GetClsRecords: function(classification) {
        var clsRecords = this._records.Get(classification);
        if (clsRecords == null) {
            clsRecords = new GPS.QArray();
            this._records.Set(classification, clsRecords);
        }
        return clsRecords;
    },
    // Set Area Record
    SetAreaRecord: function(options) {
        return this._Record(options.Classification, options.AreaId, options.ShapeId, options.Value, options.Relations);
    },
    // Get Area Record
    GetAreaRecord: function(options) {
        return this._GetRecord(options.Classification, options.AreaId, options.ShapeId, options.Relations);
    }
});

