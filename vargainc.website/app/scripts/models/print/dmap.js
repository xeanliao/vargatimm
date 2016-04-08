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
            //console.log('fetch d map image');
            var model = this,
                params = $.extend({
                    mapType: 'HYBRID'
                }, mapOption, {
                    campaignId: model.get('CampaignId'),
                    submapId: model.get('SubMapId'),
                    dmapId: model.get('DMapId')
                }),
                options = {
                    quite: true,
                    url: model.urlRoot + '/dmap/',
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
            return (this.sync || Backbone.sync).call(this, 'read', this, options);
        },
        fetchGtu: function (opts) {
            var model = this,
                options = {
                    url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/gtu/',
                    method: 'GET',
                    success: function (result) {
                        var gtus = [];
                        for (var i = 0; i < result.pointsColors.length; i++) {
                            if (result.points[i] && result.points[i].length > 0) {
                                gtus.push({
                                    gtuId: result.points[i][0].Id,
                                    color: result.pointsColors[i],
                                    points: result.points[i]
                                });
                            }
                        }
                        model.set({
                            'Gtu': gtus,
                            lastUpdateTime: result.lastUpdateTime
                        });
                    }
                };
            _.extend(options, opts);
            return (this.sync || Backbone.sync).call(this, 'read', this, options);
        },
        fetchAllGtu: function (opts) {
            var model = this,
                options = {
                    url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/gtu/all/',
                    method: 'GET',
                    success: function (result) {
                        var gtus = [];
                        for (var i = 0; i < result.pointsColors.length; i++) {
                            if (result.points[i] && result.points[i].length > 0) {
                                gtus.push({
                                    gtuId: result.points[i][0].Id,
                                    color: result.pointsColors[i],
                                    points: result.points[i]
                                });
                            }
                        }
                        model.set({
                            'Gtu': gtus,
                            lastUpdateTime: result.lastUpdateTime
                        });
                    }
                };
            _.extend(options, opts);
            return (this.sync || Backbone.sync).call(this, 'read', this, options);
        },
        updateGtuAfterTime: function (time, opts) {
            var model = this,
                lastTime = time ? time.utc().format('YYYYMMDDTHHmmss[Z]') : model.get('lastUpdateTime'),
                options = {
                    url: 'print/campaign/' + model.get('CampaignId') + '/submap/' + model.get('SubMapId') + '/dmap/' + model.get('DMapId') + '/gtu/all/' + lastTime,
                    method: 'GET',
                    success: function (result) {
                        var gtus = model.get('Gtu') || [];
                        for (var i = 0; i < result.pointsColors.length; i++) {
                            var colorItem = _.find(gtus, {
                                color: result.pointsColors[i]
                            });
                            if (!colorItem) {
                                gtus.push({
                                    color: result.pointsColors[i],
                                    points: result.points[i]
                                })
                            } else {
                                colorItem.points = _.concat(colorItem.points, result.points[i]);
                            }
                        }
                        model.set({
                            lastUpdateTime: result.lastUpdateTime
                        });
                    }
                };
            _.extend(options, opts);
            return (this.sync || Backbone.sync).call(this, 'read', this, options);
        }
    });
});