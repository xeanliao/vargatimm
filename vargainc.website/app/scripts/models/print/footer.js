define([
    'underscore',
    'backbone',
    'models/print/base'
], function (_, Backbone, BaseModel) {
	return BaseModel.extend({
		defaults: {
            'DisplayName': null,
            'Date': null,
            'ContactName': null,
            'CreatorName': null
        }
	});
});