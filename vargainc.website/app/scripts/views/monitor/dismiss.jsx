define([
	'moment',
	'backbone',
	'react',
	'pubsub',
	'views/user/adminList',
	'react.backbone'
], function (moment, Backbone, React, Topic, AdminUserList) {
	return React.createBackboneClass({
		componentWillMount: function(){
		},
		onUserSelected: function(user){
			this.setState({selectedUser: user});
		},
		onDbUserSelected: function(user){
			this.setState({selectedUser: user});
			Topic.publish('monitor/dismiss', user);
		},
		onClose: function(){
			Topic.publish("showDialog");
		},
		onProcess: function(){
			console.log('dmap publish topic');
			if(this.state && this.state.selectedUser){
				Topic.publish('monitor/dismiss', this.state.selectedUser);
			}
		},
		render: function(){
			return (
				<div>
					<h5>Dismiss back to Distribution Map</h5>
					<span>Assign to</span>
					<AdminUserList onSelect={this.onUserSelected} onDbSelect={this.onDbUserSelected} group="distribution" />
					<div className="float-right">
						<button className="success button" onClick={this.onProcess}>Okay</button>
						<a href="javascript:;" className="button" onClick={this.onClose}>Cancel</a>
					</div>
				</div>
			);
		}
	});
});