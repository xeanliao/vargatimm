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
            suppressCover: true,

            suppressCampaign: true,
            suppressCampaignSummary: true,
            suppressNDAInCampaign: true,

            suppressSubMap: true,
            suppressSubMapCountDetail: true,
            suppressNDAInSubMap: true,

            suppressDMap: false,
            suppressGTU: true,
            suppressNDAInDMap: true,

            customSubMapPenetrationColors: false,
            suppressLocations: true,
            suppressRadii: true,
            /**
             * ROADMAP, SATELLITE, HYBRID, TERRAIN
             */
            mapType: 'ROADMAP'
        }
    });
});