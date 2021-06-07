GPS.Map.SubMapAreas = function() {
    this._records = null;
    this.__Init__();
}

GPS.Map.SubMapAreas.prototype = {
    __Init__: function() {
        this._records = [];
    },

    Clear: function() {
        this._records = [];
    },
    Add: function(submapId, area) {
        var isExists = false;
        if (!this._records[submapId]) {
            this._records[submapId] = new Array();
        }
        for (var i = 0; i < this._records[submapId].length; i++) {
            if (this._records[submapId][i]._id == area._id) {
                isExists = true;
                break;
            }
        }
        if (!isExists)
            this._records[submapId].push(area);
    },
    GetSubmapAreas: function(submapId) {
        for (var submapAreas in this._records) {
            if (submapAreas == submapId)
                return this._records[submapId];
        }
        return null;
    },
    GetArea: function(submapId, areaId) {
        for (var i = 0; i < this._records[submapId].length; i++) {
            if (this._records[submapId][i]._id == areaId)
                return this._records[submapId][i];
        }
        return null;
    },
    RemoveArea: function(submapId, areaId) {
        for (var submapAreas in this._records) {
            if (submapAreas == submapId) {
                for (var i = 0; i < this._records[submapAreas].length; i++) {
                    if (areaId == this._records[submapAreas][i]._id) {
                        this._records[submapAreas].splice(i, 1);
                        return ;
                    }
                }
            }
        }
    }
}