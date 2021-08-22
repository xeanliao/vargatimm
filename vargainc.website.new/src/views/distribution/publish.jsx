import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';

import BaseView from 'views/base';
import AdminUserList from 'views/user/adminList';

export default React.createBackboneClass({
	mixins: [BaseView],
	componentWillMount: function () {},
	onUserSelected: function (user) {
		this.setState({
			selectedUser: user
		});
	},
	onDbUserSelected: function (user) {
		this.setState({
			selectedUser: user
		});
		this.publish('distribution/publish', user);
	},
	onClose: function () {
		this.publish("showDialog");
	},
	onProcess: function () {
		if (this.state && this.state.selectedUser) {
			this.publish('distribution/publish', this.state.selectedUser);
		}
	},
	render: function () {
		return (
			<div>
				<h5>Distribution Map Publish</h5>
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