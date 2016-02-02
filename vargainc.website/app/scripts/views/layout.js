var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

define(['jquery', 'react', 'react-dom', 'pubsub', 'models/user', 'react.backbone', 'foundation'], function ($, React, ReactDOM, Topic, UserModel) {
	var MenuItem = React.createClass({
		displayName: 'MenuItem',

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

	var User = React.createBackboneClass({
		getDefaultProps: function () {
			return {
				model: new UserModel()
			};
		},
		componentDidMount: function () {
			this.getModel().fetchCurrentUser();
			Topic.subscribe("NOT_LOGIN", function () {
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

	return React.createClass({
		displayName: 'Layout',
		componentDidMount: function () {
			var self = this;
			Topic.subscribe("loadView", function (view, collection, model) {
				self.setState({ mainView: view, mainCollection: collection, mainModel: model });
			});
			Topic.subscribe("showDialog", function (view, collection, model) {
				if (view) {
					self.setState({ dialogView: view, dialogCollection: collection, dialogModel: model });
				} else {
					$('.reveal').foundation('close');
					self.setState({ dialogView: null, dialogCollection: null, dialogModel: null });
				}
			});
			/**
    * fix main view size
    */
			$(window).on('resize', function () {
				console.log('resize', $(window).height());
				$(".off-canvas-wrapper-inner").height($(window).height());
			});
			$(window).trigger('resize');
		},
		componentDidUpdate: function (prevProps, prevState) {
			if (this.state.dialogView && Foundation) {
				$('.reveal').foundation();
				$('.reveal').foundation('open');
			}
			$('iframe').height($(window).height());
		},
		menuSwitch: function () {
			console.log('menu switch', this.state.isSideMenuOpen);
			var isSideMenuOpen = !this.state.isSideMenuOpen;
			this.setState({ isSideMenuOpen: isSideMenuOpen });
		},
		closeMenu: function () {
			this.setState({ isSideMenuOpen: false });
		},
		getDefaultProps: function () {
			var menu = [{
				//key: 'campaign', icon: 'fa fa-paper-plane-o', address: 'campaign', name: 'Campaign'
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
			return { menu: menu };
		},
		fullTextSearch: function (e) {
			Topic.publish('search', e.currentTarget.value);
		},
		render: function () {
			/**
    * build main view
    */
			if (this.state && this.state.mainView) {
				if (React.isValidElement(this.state.mainView)) {
					var mainView = this.state.mainView;
				} else {
					var MainView = React.createFactory(this.state.mainView);
					if (this.state.mainCollection || this.state.mainModel) {
						var mainView = MainView({ collection: this.state.mainCollection, model: this.state.mainModel });
					} else {
						var mainView = MainView();
					}
				}
			} else {
				var mainView = React.createElement('h1', null, 'loading');
			}
			/**
    * build dialog view
    */
			if (this.state && this.state.dialogView) {
				if (typeof this.state.dialogView === 'string') {
					var dialogView = React.createElement('h5', null, this.state.dialogView);
				} else if (React.isValidElement(this.state.dialogView)) {
					var dialogView = this.state.dialogView;
				} else {
					var DialogView = React.createFactory(this.state.dialogView);
					var dialogView = DialogView({ collection: this.state.dialogCollection, model: this.state.dialogModel, ref: "DialogView" });
				}
			} else {
				var dialogView = null;
			}

			var offCanvasClassName = this.state && this.state.isSideMenuOpen ? 'off-canvas-wrapper-inner is-off-canvas-open is-open-left' : 'off-canvas-wrapper-inner';
			return React.createElement(
				'div',
				null,
				React.createElement(
					'div',
					{ className: 'off-canvas-wrapper' },
					React.createElement(
						'div',
						{ className: offCanvasClassName, 'data-off-canvas-wrapper': true },
						React.createElement(
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
						),
						React.createElement(
							'div',
							{ className: 'main off-canvas-content', 'data-off-canvas-content': true },
							React.createElement(
								'div',
								{ className: 'title-bar' },
								React.createElement(
									'div',
									{ className: 'title-bar-left' },
									React.createElement('button', { 'aria-expanded': 'true', className: 'menu-icon', type: 'button', onClick: this.menuSwitch }),
									React.createElement(
										'span',
										{ className: 'title-text' },
										'TIMM System'
									)
								),
								React.createElement(User, null),
								React.createElement(
									'span',
									{ className: 'title-bar-center' },
									React.createElement(
										'div',
										{ className: 'topSearchBar hide-for-small-only' },
										React.createElement('input', { type: 'text', placeholder: 'Search', onChange: this.fullTextSearch })
									)
								)
							),
							mainView
						)
					)
				),
				React.createElement(
					'div',
					{ className: 'reveal', 'data-reveal': true },
					dialogView
				)
			);
		}
	});
});
