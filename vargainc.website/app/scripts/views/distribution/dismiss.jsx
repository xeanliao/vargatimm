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
			Topic.publish('distribution/dismiss', user);
		},
		onClose: function(){
			Topic.publish("showDialog", null);
		},
		onProcess: function(){
			console.log('dmap publish topic');
			if(this.state && this.state.selectedUser){
				Topic.publish('distribution/dismiss', this.state.selectedUser);
			}
		},
		render: function(){
			
			return (
				<div>
					<h5>Campaign Publish</h5>
					<span>Assign to</span>
					<AdminUserList onSelect={this.onUserSelected} onDbSelect={this.onDbUserSelected} group="campaign" />
					<div className="float-right">
						<button className="success button" onClick={this.onProcess}>Okay</button>
						<a href="javascript:;" className="button" onClick={this.onClose}>Cancel</a>
					</div>
				</div>
			);
		}
	});
});