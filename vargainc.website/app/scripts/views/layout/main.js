define(['jquery', 'react', 'react-dom', 'views/base', 'models/user', 'views/layout/menu', 'views/layout/loading', 'react.backbone', 'foundation'], function ($, React, ReactDOM, BaseView, UserModel, MenuView, LoadingView) {
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
		getInitialState: function () {
			return {
				mainView: null,
				mainParams: null,
				dialogView: null,
				dialogParams: null,
				dialogSize: 'small',
				dialogCustomClass: '',
				loading: false,
				showMenu: true
			};
		},
		componentDidMount: function () {
			var self = this;
			/**
    * set main view
    * @param  {React} view
    * @param  {Backbone.Collection} or Backbone.Model} params
    * @param  {showMenu: {boolean}
    */
			this.subscribe("loadView", function (view, params, options) {
				self.setState({
					mainView: view,
					mainParams: params
				});
				self.setState({
					showMenu: _.has(options, 'showMenu') ? options.showMenu : true
				});
			});
			/**
    * show a dialog
    * @param  {React} view
    * @param  {Backbone.Collection} or Backbone.Model} params
    * @param  {size: {String} size Foundation Reveal Size Value: tiny, small, large, full} options
    */
			this.subscribe("showDialog", function (view, params, options) {
				self.setState({
					dialogView: view,
					dialogParams: params
				});

				self.setState({
					dialogSize: _.has(options, 'size') ? options.size : 'small',
					dialogCustomClass: _.has(options, 'customClass') ? options.customClass : ''
				});
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
		fullTextSearch: function (e) {
			this.publish('search', e.currentTarget.value);
		},
		menuSwitch: function () {
			this.refs.sideMenu && this.refs.sideMenu.switch();
		},
		/**
   * build main view
   */
		getMainView: function () {
			if (this.state.mainView) {
				if (React.isValidElement(this.state.mainView)) {
					return this.state.mainView;
				} else {
					var MainView = React.createFactory(this.state.mainView);
					return MainView(this.state.mainParams);
				}
			}
			return null;
		},
		/**
         * build dialog view
         */
		getDialogView: function () {
			if (this.state.dialogView) {
				if (_.isString(this.state.dialogView)) {
					var dialogView = React.createElement('h5', null, this.state.dialogView);
				} else if (React.isValidElement(this.state.dialogView)) {
					var dialogView = this.state.dialogView;
				} else {
					var DialogView = React.createFactory(this.state.dialogView),
					    params = _.extend(this.state.dialogParams, {
						ref: "DialogView"
					});
					return DialogView(params);
				}
			}
			return null;
		},
		render: function () {
			var mainView = this.getMainView(),
			    dialogView = this.getDialogView();

			if (this.state.showMenu) {
				var mainMenuClassName = 'left-menu';
				var menu = React.createElement(MenuView, { ref: 'sideMenu' });
			} else {
				var mainMenuClassName = '';
				var menu = null;
			}

			return React.createElement(
				'div',
				null,
				React.createElement(
					'div',
					{ className: 'off-canvas-wrapper' },
					React.createElement(
						'div',
						{ className: 'off-canvas-wrapper-inner', 'data-off-canvas-wrapper': true },
						menu,
						React.createElement(
							'div',
							{ className: "main off-canvas-content " + mainMenuClassName, 'data-off-canvas-content': true },
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
					{ className: 'reveal ' + this.state.dialogSize + ' ' + this.state.dialogCustomClass, 'data-reveal': true, 'data-options': 'closeOnClick: false; closeOnEsc: false' },
					dialogView
				),
				React.createElement(
					'div',
					{ className: 'overlayer', style: { 'display': this.state.loading ? 'block' : 'none' } },
					React.createElement(LoadingView, null)
				)
			);
		}
	});
});
