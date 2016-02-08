define([
	'underscore',
	'pubsub'
], function(_, Topic){
	return{
		registerTopic: [],
		subscribe: function(name, callback){
			console.log('register topic');
			var signal = Topic.subscribe(name, callback);
			this.registerTopic.push(signal);
		},
		publish: function(){
			Topic.publish.apply(this, arguments);
		},
		componentWillUnmount: function(){
			console.log('componentDidUpdate', this.registerTopic.length);
			_.forEach(this.registerTopic, function(i){
				Topic.unsubscribe(i);
			});
		}
	};
});