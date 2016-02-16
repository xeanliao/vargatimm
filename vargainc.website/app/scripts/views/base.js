define([
	'underscore',
	'pubsub'
], function (_, Topic) {
	return {
		getDefaultProps: function(){
			return {
				registerTopic: {}
			}
		},
		subscribe: function (name, callback) {
			var signal = Topic.subscribe(name, callback);
			this.props.registerTopic[name] = signal;
			return signal;
		},
		publish: function () {
			Topic.publish.apply(this, arguments);
		},
		unsubscribe: function (name) {
			var signal = this.props.registerTopic[name];
			_.unset(this.props.registerTopic, name);
			signal && Topic.unsubscribe(signal);
		},
		componentWillUnmount: function () {
			_.forEach(this.props.registerTopic, function (i) {
				Topic.unsubscribe(i);
			});
		},
	};

});