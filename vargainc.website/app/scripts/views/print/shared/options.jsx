define([
	'underscore',
	'views/base'
], function (_, BaseView) {
	return _.extend({}, BaseView, {
		onClose: function(){
			this.publish("showDialog", null);
			if(this.props.needTrigger){
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
				name = e.currentTarget.name,
				value = e.currentTarget.checked;
			model.set(name, value);
		}
	});
});