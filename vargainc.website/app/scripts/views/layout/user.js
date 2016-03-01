define(['react', 'backbone', 'views/base', 'models/user', 'react.backbone'], function (React, Backbone, BaseView, UserModel) {
	return React.createBackboneClass({
		mixins: [BaseView],
		componentDidMount: function () {
			$("#userMenu").foundation();
		},
		onLogout: function () {
			this.getModel().logout().always(function () {
				window.location.reload(true);
			});
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
							this.getModel().get('FullName'),
							'  '
						)
					),
					React.createElement('i', { className: 'fa fa-sign-out', style: { 'position': 'relative', 'top': '2px' } })
				)
			);
		}
	});
});
