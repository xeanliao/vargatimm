define(['moment', 'backbone', 'react', 'pubsub', 'views/user/adminList', 'react.backbone'], function (moment, Backbone, React, Topic, AdminUserList) {
	return React.createBackboneClass({
		componentWillMount: function () {},
		onUserSelected: function (user) {
			this.setState({ selectedUser: user });
		},
		onDbUserSelected: function (user) {
			this.setState({ selectedUser: user });
			Topic.publish('monitor/dismiss', user);
		},
		onClose: function () {
			Topic.publish("showDialog");
		},
		onProcess: function () {
			console.log('dmap publish topic');
			if (this.state && this.state.selectedUser) {
				Topic.publish('monitor/dismiss', this.state.selectedUser);
			}
		},
		render: function () {
			return React.createElement(
				'div',
				null,
				React.createElement(
					'h5',
					null,
					'Dismiss back to Distribution Map'
				),
				React.createElement(
					'span',
					null,
					'Assign to'
				),
				React.createElement(AdminUserList, { onSelect: this.onUserSelected, onDbSelect: this.onDbUserSelected, group: 'distribution' }),
				React.createElement(
					'div',
					{ className: 'float-right' },
					React.createElement(
						'button',
						{ className: 'success button', onClick: this.onProcess },
						'Okay'
					),
					React.createElement(
						'a',
						{ href: 'javascript:;', className: 'button', onClick: this.onClose },
						'Cancel'
					)
				)
			);
		}
	});
});
