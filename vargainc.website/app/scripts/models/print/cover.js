define([
    'underscore',
    'backbone',
    'models/print/base'
], function (_, Backbone, BaseModel) {
	return BaseModel.extend({
		defaults: {
			'Id': null,
            'ClientName': null,
            'ContactName': null,
            'DisplayName': null,
            'CreatorName': null,
            'Date': null,
            'Logo': null,
        }
	});
});