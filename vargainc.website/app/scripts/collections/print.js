import Backbone from 'backbone';
import $ from 'jquery';
import Promise from 'bluebird';
import {
    extend,
    each
} from 'lodash';
import BaseModel from 'models/print/base';
import FooterModel from 'models/print/footer';
import CoverModel from 'models/print/cover';
import CampaignModel from 'models/print/campaign';
import SubmapModel from 'models/print/submap';
import CRouteModel from 'models/print/croute';
import DmapModel from 'models/print/dmap';
import DistributionModel from 'models/print/distribution';

var SubmapCollection = Backbone.Collection.extend({
    model: SubmapModel
});
var DmapCollection = Backbone.Collection.extend({
    model: DmapModel
});

export default Backbone.Collection.extend({
    model: BaseModel,
    urlRoot: 'print',
    dmaps: [],
    campaignId: null,
    fetchForCampaignMap: function (campaignId, opts) {
        var collection = this,
            options = {
                url: collection.urlRoot + '/campaign/' + campaignId,
                success: (result) => {
                    var displayName = result.DisplayName,
                        footerModel = new FooterModel({
                            DisplayName: result.DisplayName,
                            Date: result.Date,
                            ContactName: result.ContactName,
                            CreatorName: result.CreatorName
                        });

                    collection.campaignId = campaignId;
                    collection.submaps = [];
                    collection.dmaps = [];

                    collection.add(new CoverModel({
                        type: 'Cover',
                        key: 'cover',
                        ClientName: result.ClientName,
                        ContactName: result.ContactName,
                        DisplayName: result.DisplayName,
                        CreatorName: result.CreatorName,
                        Date: result.Date,
                        Logo: result.Logo,
                        Footer: footerModel
                    }));

                    collection.add(new CampaignModel({
                        type: 'Campaign',
                        key: 'campaign',
                        CampaignId: campaignId,
                        ClientName: result.ClientName,
                        ContactName: result.ContactName,
                        DisplayName: result.DisplayName,
                        CreatorName: result.CreatorName,
                        TotalHouseHold: result.TotalHouseHold,
                        TargetHouseHold: result.TargetHouseHold,
                        Penetration: result.Penetration,
                        Date: result.Date,
                        Logo: result.Logo,
                        Footer: footerModel
                    }));
                    collection.add(new CampaignModel({
                        type: 'CampaignSummary',
                        key: 'campaign-summary',
                        CampaignId: campaignId,
                        SubMaps: collection.submaps,
                        Footer: footerModel
                    }));

                    each(result.SubMaps, (submap) => {
                        collection.submaps.push({
                            Name: submap.Name,
                            OrderId: submap.OrderId,
                            TotalHouseHold: submap.TotalHouseHold,
                            TargetHouseHold: submap.TargetHouseHold,
                            Penetration: submap.Penetration
                        });
                        collection.add(new SubmapModel({
                            type: 'SubMap',
                            key: campaignId + '-' + submap.Id,
                            CampaignId: campaignId,
                            SubMapId: submap.Id,
                            Name: submap.Name,
                            OrderId: submap.OrderId,
                            TotalHouseHold: submap.TotalHouseHold,
                            TargetHouseHold: submap.TargetHouseHold,
                            Penetration: submap.Penetration,
                            Footer: footerModel
                        }));
                        collection.add(new CRouteModel({
                            type: 'SubMapDetail',
                            key: 'detail-' + campaignId + '-' + submap.Id,
                            CampaignId: campaignId,
                            SubMapId: submap.Id,
                            Name: submap.Name,
                            OrderId: submap.OrderId,
                            Footer: footerModel
                        }));
                    });
                }
            };
        options = extend(opts, options);

        return (this.sync || Backbone.sync).call(this, 'read', this, options);
    },
    fetchForDistributionMap: function (campaignId, opts) {
        var collection = this,
            options = {
                url: collection.urlRoot + '/campaign/' + campaignId,
                success: (result) => {
                    collection.campaignId = campaignId;
                    collection.dmaps = [];
                    var displayName = result.DisplayName;
                    each(result.SubMaps, function (submap) {
                        each(submap.DMaps, function (dmap) {
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
        options = extend(opts, options);

        return (this.sync || Backbone.sync).call(this, 'read', this, options);
    },
    fetchForReportMap: function (campaignId, opts) {
        var collection = this,
            options = {
                url: collection.urlRoot + '/campaign/' + campaignId,
                success: (result) => {
                    var displayName = result.DisplayName,
                        footerModel = new FooterModel({
                            DisplayName: result.DisplayName,
                            Date: result.Date,
                            ContactName: result.ContactName,
                            CreatorName: result.CreatorName
                        });

                    collection.campaignId = campaignId;
                    collection.submaps = [];
                    collection.dmaps = [];

                    collection.add(new CoverModel({
                        type: 'Cover',
                        key: 'cover',
                        ClientName: result.ClientName,
                        ContactName: result.ContactName,
                        DisplayName: result.DisplayName,
                        CreatorName: result.CreatorName,
                        Date: result.Date,
                        Logo: result.Logo,
                        Footer: footerModel
                    }));

                    collection.add(new CampaignModel({
                        type: 'Campaign',
                        key: 'campaign',
                        CampaignId: campaignId,
                        ClientName: result.ClientName,
                        ContactName: result.ContactName,
                        DisplayName: result.DisplayName,
                        CreatorName: result.CreatorName,
                        TotalHouseHold: result.TotalHouseHold,
                        TargetHouseHold: result.TargetHouseHold,
                        Penetration: result.Penetration,
                        Date: result.Date,
                        Logo: result.Logo,
                        Footer: footerModel
                    }));
                    collection.add(new CampaignModel({
                        type: 'CampaignSummary',
                        key: 'campaign-summary',
                        CampaignId: campaignId,
                        SubMaps: collection.submaps,
                        Footer: footerModel
                    }));

                    each(result.SubMaps, function (submap) {
                        collection.submaps.push({
                            Name: submap.Name,
                            OrderId: submap.OrderId,
                            TotalHouseHold: submap.TotalHouseHold,
                            TargetHouseHold: submap.TargetHouseHold,
                            Penetration: submap.Penetration
                        });
                        collection.add(new SubmapModel({
                            type: 'SubMap',
                            key: campaignId + '-' + submap.Id,
                            CampaignId: campaignId,
                            SubMapId: submap.Id,
                            Name: submap.Name,
                            OrderId: submap.OrderId,
                            TotalHouseHold: submap.TotalHouseHold,
                            TargetHouseHold: submap.TargetHouseHold,
                            Penetration: submap.Penetration,
                            Footer: footerModel
                        }));
                        collection.add(new CRouteModel({
                            type: 'SubMapDetail',
                            key: 'detail-' + campaignId + '-' + submap.Id,
                            CampaignId: campaignId,
                            SubMapId: submap.Id,
                            Name: submap.Name,
                            OrderId: submap.OrderId,
                            Footer: footerModel
                        }));
                    });

                    each(result.SubMaps, function (submap) {
                        each(submap.DMaps, function (dmap) {
                            collection.dmaps.push({
                                Selected: false,
                                Id: dmap.Id,
                                Name: dmap.Name
                            });
                            collection.add(new DmapModel({
                                type: 'DMap',
                                key: campaignId + '-' + submap.Id + '-' + dmap.Id,
                                CampaignId: campaignId,
                                SubMapId: submap.Id,
                                DMapId: dmap.Id,
                                Name: dmap.Name,
                                Total: dmap.Total,
                                DisplayName: displayName,
                                Footer: footerModel
                            }));
                        });
                    });
                }
            };
        options = extend(opts, options);

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
                    //console.log(result, result.campaignId, result.sourceFile);
                    return result;
                }
            };
        options = extend(opts, options);
        return (this.sync || Backbone.sync).call(this, 'read', this, options);
    }
});