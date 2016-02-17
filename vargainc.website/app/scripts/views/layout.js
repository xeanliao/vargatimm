var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

define(['jquery', 'react', 'react-dom', 'views/base', 'models/user', 'react.backbone', 'foundation'], function ($, React, ReactDOM, BaseView, UserModel) {
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

	var User = React.createBackboneClass({
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

	return React.createClass({
		mixins: [BaseView],
		displayName: 'Layout',
		componentDidMount: function () {
			var self = this;
			this.subscribe("loadView", function (view, collection, model) {
				self.setState({ mainView: view, mainCollection: collection, mainModel: model });
			});
			/**
    * show a dialog
    * @param  {React} view
    * @param  {Backbone.Collection} collection
    * @param  {Backbone.Model} model
    * @param  {String} size Foundation Reveal Size Value: tiny, small, large, full
    */
			this.subscribe("showDialog", function (view, collection, model, size) {
				if (view) {
					self.setState({
						dialogView: view,
						dialogCollection: collection,
						dialogSize: size || '',
						dialogModel: model
					});
				} else {
					$('.reveal').foundation('close');
					self.setState({ dialogView: null, dialogCollection: null, dialogModel: null });
				}
			});

			/**
    * loading control
    */
			var loadingCount = 0,
			    loadingDelay = 500,
			    loadingTimeout = null;
			this.subscribe('showLoading', function () {
				loadingCount++;
				window.clearTimeout(loadingTimeout);
				loadingTimeout = window.setTimeout(function () {
					self.setState({ loading: true });
				}, loadingDelay);
			});
			this.subscribe('hideLoading', function () {
				loadingCount--;
				window.setTimeout(function () {
					if (loadingCount <= 0) {
						window.clearTimeout(loadingTimeout);
						self.setState({ loading: false });
						loadingCount = 0;
					}
				}, 300);
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
			} else {
				$('.reveal').foundation();
				$('.reveal').foundation('close');
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
		getInitialState: function () {
			return {
				dialogSize: 'small',
				loading: false
			};
		},
		fullTextSearch: function (e) {
			this.publish('search', e.currentTarget.value);
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
				var mainView = null;
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
					{ className: 'reveal ' + this.state.dialogSize, 'data-reveal': true },
					dialogView
				),
				React.createElement(
					'div',
					{ className: 'overlayer', style: { 'display': this.state.loading ? 'block' : 'none' } },
					React.createElement(
						'div',
						{ className: 'loading' },
						React.createElement(
							'div',
							{ className: 'gear one' },
							React.createElement(
								'svg',
								{ id: 'blue', viewBox: '0 0 100 100', fill: '#94DDFF' },
								React.createElement('path', { d: 'M97.6,55.7V44.3l-13.6-2.9c-0.8-3.3-2.1-6.4-3.9-9.3l7.6-11.7l-8-8L67.9,20c-2.9-1.7-6-3.1-9.3-3.9L55.7,2.4H44.3l-2.9,13.6      c-3.3,0.8-6.4,2.1-9.3,3.9l-11.7-7.6l-8,8L20,32.1c-1.7,2.9-3.1,6-3.9,9.3L2.4,44.3v11.4l13.6,2.9c0.8,3.3,2.1,6.4,3.9,9.3      l-7.6,11.7l8,8L32.1,80c2.9,1.7,6,3.1,9.3,3.9l2.9,13.6h11.4l2.9-13.6c3.3-0.8,6.4-2.1,9.3-3.9l11.7,7.6l8-8L80,67.9      c1.7-2.9,3.1-6,3.9-9.3L97.6,55.7z M50,65.6c-8.7,0-15.6-7-15.6-15.6s7-15.6,15.6-15.6s15.6,7,15.6,15.6S58.7,65.6,50,65.6z' })
							)
						),
						React.createElement(
							'div',
							{ className: 'gear two' },
							React.createElement(
								'svg',
								{ id: 'pink', viewBox: '0 0 100 100', fill: '#FB8BB9' },
								React.createElement('path', { d: 'M97.6,55.7V44.3l-13.6-2.9c-0.8-3.3-2.1-6.4-3.9-9.3l7.6-11.7l-8-8L67.9,20c-2.9-1.7-6-3.1-9.3-3.9L55.7,2.4H44.3l-2.9,13.6      c-3.3,0.8-6.4,2.1-9.3,3.9l-11.7-7.6l-8,8L20,32.1c-1.7,2.9-3.1,6-3.9,9.3L2.4,44.3v11.4l13.6,2.9c0.8,3.3,2.1,6.4,3.9,9.3      l-7.6,11.7l8,8L32.1,80c2.9,1.7,6,3.1,9.3,3.9l2.9,13.6h11.4l2.9-13.6c3.3-0.8,6.4-2.1,9.3-3.9l11.7,7.6l8-8L80,67.9      c1.7-2.9,3.1-6,3.9-9.3L97.6,55.7z M50,65.6c-8.7,0-15.6-7-15.6-15.6s7-15.6,15.6-15.6s15.6,7,15.6,15.6S58.7,65.6,50,65.6z' })
							)
						),
						React.createElement(
							'div',
							{ className: 'gear three' },
							React.createElement(
								'svg',
								{ id: 'yellow', viewBox: '0 0 100 100', fill: '#FFCD5C' },
								React.createElement('path', { d: 'M97.6,55.7V44.3l-13.6-2.9c-0.8-3.3-2.1-6.4-3.9-9.3l7.6-11.7l-8-8L67.9,20c-2.9-1.7-6-3.1-9.3-3.9L55.7,2.4H44.3l-2.9,13.6      c-3.3,0.8-6.4,2.1-9.3,3.9l-11.7-7.6l-8,8L20,32.1c-1.7,2.9-3.1,6-3.9,9.3L2.4,44.3v11.4l13.6,2.9c0.8,3.3,2.1,6.4,3.9,9.3      l-7.6,11.7l8,8L32.1,80c2.9,1.7,6,3.1,9.3,3.9l2.9,13.6h11.4l2.9-13.6c3.3-0.8,6.4-2.1,9.3-3.9l11.7,7.6l8-8L80,67.9      c1.7-2.9,3.1-6,3.9-9.3L97.6,55.7z M50,65.6c-8.7,0-15.6-7-15.6-15.6s7-15.6,15.6-15.6s15.6,7,15.6,15.6S58.7,65.6,50,65.6z' })
							)
						),
						React.createElement('div', { className: 'lil-circle' }),
						React.createElement(
							'div',
							{ className: 'text' },
							'LOADING'
						)
					)
				)
			);
		}
	});
});
