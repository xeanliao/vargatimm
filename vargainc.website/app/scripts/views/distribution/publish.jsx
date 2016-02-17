define([
	'moment',
	'backbone',
	'react',
	'views/base',
	'views/user/adminList',
	'react.backbone'
], function (moment, Backbone, React, BaseView, AdminUserList) {
	return React.createBackboneClass({
		mixins: [BaseView],
		componentWillMount: function(){
		},
		onUserSelected: function(user){
			this.setState({selectedUser: user});
		},
		onDbUserSelected: function(user){
			this.setState({selectedUser: user});
			this.publish('distribution/publish', user);
		},
		onClose: function(){
			this.publish("showDialog");
		},
		onProcess: function(){
			console.log('dmap publish topic');
			if(this.state && this.state.selectedUser){
				this.publish('distribution/publish', this.state.selectedUser);
			}
		},
		render: function(){
			
			return (
				<div>
					<h5>Campaign Publish</h5>
					<span>Assign to</span>
					<AdminUserList onSelect={this.onUserSelected} onDbSelect={this.onDbUserSelected} group="monitor" />
					<div className="float-right">
						<button className="success button" onClick={this.onProcess}>Okay</button>
						<a href="javascript:;" className="button" onClick={this.onClose}>Cancel</a>
					</div>
				</div>
			);
		}
	});
});