define(['react', 'backbone', 'views/base', 'models/user', 'react.backbone'], function (React, Backbone, BaseView, UserModel) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getDefaultProps: function () {
			return {
				model: new UserModel()
			};
		},
		componentDidMount: function () {
			this.getModel().fetchCurrentUser();
			this.subscribe("NOT_LOGIN", function () {
				window.location = "../login.html";
			});

			$("#userMenu").foundation();
		},
		onLogout: function () {
			this.getModel().logout();
		},
		render: function () {
			var model = this.getModel();
			return React.createElement(
				'div',
				{ className: 'title-bar-right' },
				React.createElement(
					'a',
					{ href: 'javascript:;', onClick: this.onLogout },
					React.createElement(
						'span',
						{ className: 'hide-for-small-only' },
						React.createElement(
							'small',
							null,
							'Welcome, ',
							model.get('FullName'),
							'  '
						)
					),
					React.createElement('i', { className: 'fa fa-sign-out', style: { 'position': 'relative', 'top': '2px' } })
				)
			);
		}
	});
});
