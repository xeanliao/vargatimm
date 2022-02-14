import Backbone from 'backbone'
import BaseModel from 'models/print/base'
import $ from 'jquery'
import Promise from 'bluebird'

export default BaseModel.extend({
    urlRoot: 'map',
    idAttribute: 'key',
    defaults: {
        Id: null,
        Name: null,
        Total: null,
        DisplayName: null,
        MapImage: null,
        PolygonImage: null,
        ImageStatus: 'waiting',
    },
    fetchMapImage: function (mapOption) {
        var model = this,
            params = $.extend(
                {
                    mapType: 'HYBRID',
                },
                mapOption,
                {
                    campaignId: model.get('CampaignId'),
                    submapId: model.get('SubMapId'),
                }
            ),
            options = {
                quite: true,
                url: model.urlRoot + '/submap/',
                method: 'POST',
                processData: true,
                data: $.param(params),
                success: (result) => {
                    var mapImage = null,
                        polygonImage = null
                    if (result && result.success) {
                        mapImage = result.tiles
                        polygonImage = result.geometry
                    }
                    model.set('MapImage', mapImage, {
                        silent: true,
                    })
                    model.set('PolygonImage', polygonImage, {
                        silent: true,
                    })
                },
            }
        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
})
