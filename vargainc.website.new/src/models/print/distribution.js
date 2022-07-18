import Backbone from 'backbone'
import BaseModel from 'models/print/base'
import $ from 'jquery'
import { extend } from 'lodash'

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
                    dmapId: model.get('DMapId'),
                }
            ),
            options = {
                quite: true,
                url: model.urlRoot + '/distribution/',
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
        if (model.get('TopRight') && model.get('BottomLeft')) {
            options.data = $.param(
                $.extend(params, {
                    topRightLat: model.get('TopRight').lat,
                    topRightLng: model.get('TopRight').lng,
                    bottomLeftLat: model.get('BottomLeft').lat,
                    bottomLeftLng: model.get('BottomLeft').lng,
                })
            )
        }
        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
    fetchBoundary: function (opts) {
        var model = this,
            options = {
                url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/boundary/',
                method: 'GET',
                success: (result) => {
                    model.set({
                        Boundary: result.boundary,
                        Color: result.color,
                    })
                },
            }
        extend(options, opts)
        return (this.sync || Backbone.sync).call(this, 'update', this, options)
    },
})
