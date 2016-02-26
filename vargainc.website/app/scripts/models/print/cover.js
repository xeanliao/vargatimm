define([
    'underscore',
    'backbone',
    'models/print/base'
], function (_, Backbone, BaseModel) {
	return BaseModel.extend({
        idAttribute: 'key',
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