define([
    'underscore',
    'backbone',
    'models/print/base',
    'models/print/cover',
    'models/print/footer',
    'models/print/distribution'
], function (_, Backbone, BaseModel, CoverModel, FooterModel, DistributionModel) {
    var DmapModel = Backbone.Model.extend({
        defaults: {
            Id: null,
            Name: null,
            Selected: null
        },
        idAttribute: 'Id',
    });
    var DmapCollection = Backbone.Collection.extend({
        model: DmapModel
    });

    return Backbone.Collection.extend({
        model: BaseModel,
        urlRoot: 'print',
        dmaps: [],
        campaignId: null,
        fetchForDistributionMap: function (campaignId, opts) {
            var collection = this,
                options = {
                    url: collection.urlRoot + '/campaign/' + campaignId,
                    success: function (result) {
                        collection.campaignId = campaignId;
                        collection.dmaps = [];
                        var displayName = result.DisplayName;
                        _.forEach(result.SubMaps, function (submap) {
                            _.forEach(submap.DMaps, function (dmap) {
                                collection.dmaps.push({
                                    Selected: false,
                                    Id: dmap.Id,
                                    Name: dmap.Name
                                });
                                collection.add(new DistributionModel({
                                    key: campaignId + '-' + submap.Id + '-' + dmap.Id,
                                    CampaignId: campaignId,
                                    SubMapId: submap.Id,
                                    DMapId: dmap.Id,
                                    Name: dmap.Name,
                                    Total: dmap.Total,
                                    DisplayName: displayName,
                                    ImageStatus: 'waiting'
                                }));
                            });
                        });
                    }
                };
            options = _.extend(opts, options);

            return (this.sync || Backbone.sync).call(this, 'read', this, options);
        },
        getCampaignId: function () {
            return this.campaignId;
        },
        getDMaps: function () {
            return new DmapCollection(this.dmaps);
        },
        exportPdf: function (params, opts) {
            var collection = this,
                options = {
                    url: 'pdf/build/',
                    method: 'POST',
                    processData: true,
                    data: $.param({
                        'options': JSON.stringify(params)
                    }),
                    success: function (result) {
                        console.log(result, result.campaignId, result.sourceFile);
                        return result;
                    }
                };
            options = _.extend(opts, options);
            return (this.sync || Backbone.sync).call(this, 'read', this, options);
        }
    });
});