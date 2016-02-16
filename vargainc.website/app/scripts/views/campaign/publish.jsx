define([
	'moment',
	'backbone',
	'react',
	'pubsub',
	'views/user/adminList',
	'react.backbone'
], function (moment, Backbone, React, Topic, AdminUserList) {
	// var userList = new UserCollection();
	// var SelectionView = React.createFactory(UserSelection);
	// var selectionView = SelectionView({collection: userList});

	return React.createBackboneClass({
		componentWillMount: function(){
		},
		onUserSelected: function(user){
			this.setState({selectedUser: user});
		},
		onDbUserSelected: function(user){
			this.setState({selectedUser: user});
			Topic.publish('campaign/publish', user);
		},
		onClose: function(){
			Topic.publish("showDialog", null);
		},
		onProcess: function(){
			console.log('campaign publish topic');
			if(this.state && this.state.selectedUser){
				Topic.publish('campaign/publish', this.state.selectedUser);
			}
		},
		render: function(){
			
			return (
				<div>
					<h5>Campaign Publish</h5>
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