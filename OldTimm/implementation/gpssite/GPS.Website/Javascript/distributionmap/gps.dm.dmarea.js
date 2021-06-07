GPS.Map.DMAreas = function() {
    this._records = null;
    this.__Init__();
}

GPS.Map.DMAreas.prototype = {
    __Init__: function() {
        this._records = [];
    },

    Clear: function() {
        this._records = [];
    },
    Add: function(dmId, area) {
        var isExists = false;
        if (!this._records[dmId]) {
            this._records[dmId] = new Array();
        }
        for (var i = 0; i < this._records[dmId].length; i++) {
            if (this._records[dmId][i]._id == area._id) {
                isExists = true;
                break;
            }
        }
        if (!isExists)
            this._records[dmId].push(area);
    },
    GetSubmapAreas: function(dmId) {
        for (var dmAreas in this._records) {
            if (dmAreas == dmId)
                return this._records[dmId];
        }
        return null;
    },
    GetArea: function(dmId, areaId) {
        for (var i = 0; i < this._records[dmId].length; i++) {
            if (this._records[dmId][i]._id == areaId)
                return this._records[dmId][i];
        }
        return null;
    },
    RemoveArea: function(dmId, areaId) {
        for (var dmAreas in this._records) {
            if (dmAreas == dmId) {
                for (var i = 0; i < this._records[dmAreas].length; i++) {
                    if (areaId == this._records[dmAreas][i]._id) {
                        this._records[dmAreas].splice(i, 1);
                        return ;
                    }
                }
            }
        }
    }
}