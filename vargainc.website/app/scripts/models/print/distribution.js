define([
    'jquery',
    'underscore',
    'backbone',
    'pubsub',
    'models/print/base'
], function ($, _, Backbone, Topic, BaseModel) {
    return BaseModel.extend({
        urlRoot: 'map',
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
            var model = this,
                params = $.extend(mapOption, {
                    campaignId: model.get('CampaignId'),
                    submapId: model.get('SubMapId'),
                    dmapId: model.get('DMapId')
                }),
                options = {
                    quite: true,
                    url: model.urlRoot + '/distribution/',
                    method: 'POST',
                    processData: true,
                    data: $.param(params),
                    success: function (result) {
                        if (result && result.success) {
                            var mapImageLoaded = false,
                                mapImageAddress = result.tiles,
                                polygonImageLoaded = false,
                                polygonImageAddress = result.geometry,
                                imageLoaded = function () {
                                    model.set({
                                        'MapImage': mapImageAddress,
                                        'PolygonImage': polygonImageAddress
                                    });
                                }

                            var mapImage = $(new Image()).one('load', function () {
                                mapImageLoaded = true;
                                polygonImageLoaded && imageLoaded();
                            }).attr('src', mapImageAddress);

                            var polygonImage = $(new Image()).one('load', function () {
                                polygonImageLoaded = true;
                                mapImageLoaded && imageLoaded();
                            }).attr('src', polygonImageAddress);
                        } else {
                            model.set({
                                'MapImage': null,
                                'PolygonImage': null
                            });
                        }
                    },
                    error: function () {
                        model.set({
                            'MapImage': null,
                            'PolygonImage': null
                        });
                    }
                };
            if (model.get('TopRight') && model.get('BottomLeft')) {
                options.data = $.param($.extend(params, {
                    topRightLat: model.get('TopRight').lat,
                    topRightLng: model.get('TopRight').lng,
                    bottomLeftLat: model.get('BottomLeft').lat,
                    bottomLeftLng: model.get('BottomLeft').lng
                }));
            }
            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        },
        fetchBoundary: function (opts) {
            var model = this,
                options = {
                    url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/boundary/',
                    method: 'GET',
                    success: function (result) {
                        model.set({
                            'Boundary': result.boundary,
                            'Color': result.color
                        });
                    }
                };
            _.extend(options, opts);
            return (this.sync || Backbone.sync).call(this, 'update', this, options);
        }
    });
});