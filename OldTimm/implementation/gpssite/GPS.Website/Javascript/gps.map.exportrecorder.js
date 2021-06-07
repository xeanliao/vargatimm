//
GPS.Map.ExportRecorder = function(classification) {
    this._classification = null;
    this._defaultExport = null;
    this._records = null;
    this.__Init__(classification);
}

GPS.Map.ExportRecorder.prototype = {
    __Init__: function(classification) {
        this._defaultExport = false;
        this._records = new Array();
    },

    __GetRecord__: function(areaId) {
        var record = null;
        var i = 0;
        var length = this._records.length;
        while (i < length) {
            if (this._records[i][0] == areaId) {
                record = this._records[i];
                break;
            }
            i++;
        }
        return record;
    },

    __NewRecord__: function(areaId, exportArea) {
        record = [areaId, exportArea];
        this._records.push(record);
    },

    __RemoveRecord__: function(areaId) {
        var i = 0;
        var length = this._records.length;
        while (i < length) {
            if (this._records[i][0] == areaId) {
                this._records.splice(i, 1);
                break;
            }
            i++;
        }
    },

    Record: function(areaId, exportArea) {
        if (this._defaultExport != exportArea) {
            var record = this.__GetRecord__(areaId);
            if (record) { record[1] = exportArea; }
            else { this.__NewRecord__(areaId, exportArea); }
        } else { this.__RemoveRecord__(areaId); }
    },

    IsExportArea: function(areaId) {
        var record = this.__GetRecord__(areaId);
        if (record) { return record[1]; }
        else { return this._defaultExport; }
    },

    IsExport: function(areaObj) {
        
    },

    GetString: function() {
        var array = [];
        var i = 0;
        var length = this._records.length;
        while (i < length) {
            if (this._records[i][1]) {
                array.push(this._records[i][0]);
            }
            i++;
        }
        return array.join(',');
    },

    GetLength: function() {
        return this._records.length;
    }
}
