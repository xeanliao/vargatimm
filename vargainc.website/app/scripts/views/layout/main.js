define(['jquery', 'react', 'react-dom', 'views/base', 'models/user', 'views/layout/menu', 'react.backbone', 'foundation'], function ($, React, ReactDOM, BaseView, UserModel, MenuView) {

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
					dialogSize: _.has(options, 'size') ? options.size : 'small'
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
