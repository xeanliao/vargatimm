define([
	'underscore',
	'views/base'
], function (_, BaseView) {

});
import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import {
	extend
} from 'lodash';

import BaseView from 'views/base';

export default extend({}, BaseView, {
	onClose: function () {
		this.publish("showDialog");
		if (this.props.needTrigger) {
			this.publish('print.map.options.changed', this.getModel());
		}
	},
	onApply: function (e) {
		e.preventDefault();
		e.stopPropagation();
		this.publish('print.map.options.changed', this.getModel());
	},
	OnValueChanged: function (e) {
		var model = this.getModel(),
			ele = $(e.currentTarget),
			name = ele.attr('name'),
			value = ele.is('input:checkbox') ? ele.prop('checked') : ele.val();
		model.set(name, value);
	}
});