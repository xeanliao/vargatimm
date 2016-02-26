define([
    'underscore',
    'backbone',
    'models/print/base'
], function (_, Backbone, BaseModel) {
	return BaseModel.extend({
        urlRoot: 'print',
		defaults: {
            'Name': null,
            'TotalHouseHold': null,
            'TargetHouseHold': null,
            'Penetration': null
        },
        fetchBySubMap: function(opts){
            var model = this,
                campaignId = model.get('CampaignId'),
                submapId = model.get('SubMapId'),
                url = model.urlRoot + '/campaign/' + campaignId + '/submap/' + submapId + '/record',
                options = {
                    url: url,
                    success: function (result) {
                        model.set('CRoutes', result.record);
                    }
                };
            _.extend(options, opts);

            return (this.sync || Backbone.sync).call(this, 'read', this, options);
        }
	});
});