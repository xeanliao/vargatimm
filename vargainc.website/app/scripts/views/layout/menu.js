var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

define(['jquery', 'react', 'react-dom', 'views/base', 'react.backbone', 'foundation'], function ($, React, ReactDOM, BaseView, UserModel) {
	var MenuItem = React.createClass({
		displayName: 'MenuItem',

		mixins: [BaseView],
		getInitialState: function () {
			return {
				active: false
			};
		},
		getDefaultProps: function () {
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		render: function () {
			return React.createElement(
				'li',
				null,
				React.createElement(
					'a',
					{ className: this.state.active ? 'active' : '', href: "#" + this.props.address },
					React.createElement('i', { className: this.props.icon }),
					'  ',
					this.props.name
				)
			);
		}
	});

	return React.createClass({
		mixins: [BaseView],
		getInitialState: function () {
			return {
				open: false
			};
		},
		getDefaultProps: function () {
			var menu = [{
				key: 'campaign',
				icon: 'fa fa-trophy',
				address: 'campaign',
				name: 'Campaign'
			}, {
				key: 'distribution',
				icon: 'fa fa-map',
				address: 'distribution',
				name: 'Distribution Maps'
			}, {
				key: 'monitor',
				icon: 'fa fa-truck',
				address: 'monitor',
				name: 'GPS Monitor'
			}, {
				key: 'report',
				icon: 'fa fa-file-pdf-o',
				address: 'report',
				name: 'Reports'
			}];
			return {
				menu: menu
			};
		},
		closeMenu: function () {
			this.setState({ open: false });
		},
		switch: function () {
			this.setState({ open: !this.state.open });
		},
		render: function () {
			if (this.state.open) {
				$('.off-canvas-wrapper-inner').addClass('is-off-canvas-open is-open-left');
			} else {
				$('.off-canvas-wrapper-inner').removeClass('is-off-canvas-open is-open-left');
			}
			return React.createElement(
				'div',
				{ className: 'sidebar off-canvas position-left', 'data-off-canvas': true, 'data-position': 'left' },
				React.createElement(
					'ul',
					{ className: 'menu vertical', onClick: this.closeMenu },
					this.props.menu.map(function (item) {
						return React.createElement(MenuItem, _extends({ key: item.key }, item));
					}),
					React.createElement(
						'li',
						null,
						React.createElement(
							'a',
							{ href: '#admin' },
							React.createElement('i', { className: 'fa fa-gear' }),
							'  Administration'
						),
						React.createElement(
							'ul',
							{ className: 'submenu menu vertical' },
							React.createElement(
								'li',
								null,
								React.createElement(
									'a',
									{ href: '#frame/Users.aspx' },
									React.createElement(
										'span',
										null,
										'User Management'
									)
								)
							),
							React.createElement(
								'li',
								null,
								React.createElement(
									'a',
									{ href: '#frame/NonDeliverables.aspx' },
									React.createElement(
										'span',
										null,
										'Non-Deliverables'
									)
								)
							),
							React.createElement(
								'li',
								null,
								React.createElement(
									'a',
									{ href: '#frame/GtuAdmin.aspx?AssignNameToGTUFlag=true' },
									React.createElement(
										'span',
										null,
										'GTU Management'
									)
								)
							),
							React.createElement(
								'li',
								null,
								React.createElement(
									'a',
									{ href: '#frame/AvailableGTUList.aspx' },
									React.createElement(
										'span',
										null,
										'GTU Available List'
									)
								)
							),
							React.createElement(
								'li',
								null,
								React.createElement(
									'a',
									{ href: '#frame/AdminGtuToBag.aspx' },
									React.createElement(
										'span',
										null,
										'GTU bag Management '
									)
								)
							),
							React.createElement(
								'li',
								null,
								React.createElement(
									'a',
									{ href: '#frame/AdminGtuBagToAuditor.aspx' },
									React.createElement(
										'span',
										null,
										'Assign GTU-Bags to Auditors'
									)
								)
							),
							React.createElement(
								'li',
								null,
								React.createElement(
									'a',
									{ href: '#frame/AdminDistributorCompany.aspx' },
									React.createElement(
										'span',
										null,
										'Distributor Management'
									)
								)
							)
						)
					)
				)
			);
		}
	});
});
