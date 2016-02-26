define([
    'jquery',
    'underscore',
    'backbone',
    'pubsub',
    'models/print/base'
], function ($, _, Backbone, Topic, BaseModel) {
    return BaseModel.extend({
        urlRoot: 'map',
        idAttribute: 'key',
        defaults: {
            'Id': null,
            'Name': null,
            'Total': null,
            'DisplayName': null,
            'MapImage': null,
            'PolygonImage': null,
            'ImageStatus': 'waiting'
        },
        fetchMapImage: function (mapOption) {
            console.log('fetch sub map image');
            var model = this,
                params = $.extend({mapType: 'HYBRID'}, mapOption, {
                    campaignId: model.get('CampaignId'),
                    submapId: model.get('SubMapId'),
                }),
                options = {
                    quite: true,
                    url: model.urlRoot + '/submap/',
                    method: 'POST',
                    processData: true,
                    data: $.param(params),
                    success: function (result) {
                        var mapImage = null,
                            polygonImage = null;
                        if (result && result.success) {
                            mapImage = result.tiles;
                            polygonImage = result.geometry;
                        }
                        model.set('MapImage', mapImage, {
                            silent: true
                        });
                        model.set('PolygonImage', polygonImage, {
                            silent: true
                        });
                    }
                };
            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        }
    });
});