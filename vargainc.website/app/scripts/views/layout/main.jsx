import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import $ from 'jquery';
import BaseView from 'views/base';
import MenuView from 'views/layout/menu';
import UserView from 'views/layout/user';
import LoadingView from 'views/layout/loading';
import {
	isString,
	has,
	extend
} from 'lodash';

export default React.createBackboneClass({
	mixins: [
		BaseView,
		React.BackboneMixin("user", "change:FullName"),
	],
	getInitialState: function () {
		return {
			mainView: null,
			mainParams: null,
			dialogView: null,
			dialogParams: null,
			dialogSize: 'small',
			dialogCustomClass: '',
			loading: false,
			pageTitle: 'TIMM System',
			showMenu: null,
			showSearch: null,
			showUser: null
		}
	},
	componentDidMount: function () {
		var self = this;
		/**
		 * set main view
		 * @param  {React} view
		 * @param  {Backbone.Collection} or Backbone.Model} params
		 * @param  {showMenu: {boolean}
		 */
		this.subscribe('loadView', function (data) {
			self.setState({
				mainView: data.view,
				mainParams: data.params
			});
			var options = data.options;
			self.setState({
				pageTitle: has(options, 'pageTitle') ? options.pageTitle : 'TIMM System',
				showMenu: has(options, 'showMenu') ? options.showMenu : true,
				showSearch: has(options, 'showSearch') ? options.showSearch : true,
				showUser: has(options, 'showUser') ? options.showUser : true
			});
		});
		/**
		 * show a dialog
		 * @param  {React} view
		 * @param  {Backbone.Collection} or Backbone.Model} params
		 * @param  {size: {String} size Foundation Reveal Size Value: tiny, small, large, full} options
		 */
		this.subscribe('showDialog', function (data) {
			if (data) {
				self.setState({
					dialogView: data.view,
					dialogParams: data.params,
				});
				var options = data.options;
				self.setState({
					dialogSize: has(options, 'size') ? options.size : 'small',
					dialogCustomClass: has(options, 'customClass') ? options.customClass : ''
				});
			} else {
				self.setState({
					dialogView: null,
					dialogParams: null,
					dialogSize: null,
					dialogCustomClass: null,
				});
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
				self.setState({
					loading: true
				});
			}, loadingDelay);

		});
		this.subscribe('hideLoading', function () {
			loadingCount--;
			window.setTimeout(function () {
				if (loadingCount <= 0) {
					window.clearTimeout(loadingTimeout);
					self.setState({
						loading: false
					});
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

		$(window).on('click', function(){
			self.publish('Global.Window.Click');
		});
	},
	componentDidUpdate: function (prevProps, prevState) {
		if (this.state.dialogView && Foundation) {
			$('.reveal').foundation();
			var dialogSize = this.state.dialogSize;
			// $(document).off('open.zf.reveal.mainView');
			// $(document).one('open.zf.reveal.mainView', function () {
			// 	console.log('open.zf.reveal.mainView');
			// 	$('.reveal-overlay').css({
			// 		display: dialogSize == 'full' ? 'none' : 'block'
			// 	});
			// });
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
	onCloseDialog: function () {
		this.publish('showDialog');
	},
	/**
	 * build dialog view
	 */
	getDialogView: function () {
		if (this.state.dialogView) {
			if (isString(this.state.dialogView)) {
				var content = this.state.dialogView;
				return (
					<div className="row">
						<div className="small-12 columns">
							<p>&nbsp;</p>
							<h5>{content}</h5>
							<p>&nbsp;</p>
						</div>
						<div className="small-12 columns">
							<div className="button-group float-right">
								<a href="javascript:;" className="button tiny" onClick={this.onCloseDialog}>Okay</a>
							</div>
						</div>
						<button onClick={this.onCloseDialog} className="close-button" data-close aria-label="Close reveal" type="button">
					    	<span aria-hidden="true">&times;</span>
					  	</button>
					</div>
				);
			} else if (React.isValidElement(this.state.dialogView)) {
				return this.state.dialogView;
			} else {
				var DialogView = React.createFactory(this.state.dialogView),
					params = extend(this.state.dialogParams, {
						ref: "DialogView"
					});
				return DialogView(params);
			}
		}
		return null;
	},
	render: function () {
		var model = this.getModel(),
			mainView = this.getMainView(),
			dialogView = this.getDialogView();


		if (this.state.showMenu === true) {
			var mainMenuClassName = 'left-menu';
			var menu = <MenuView ref="sideMenu" />;
		} else {
			var mainMenuClassName = '';
			var menu = null;
		}
		if (this.state.showSearch === true) {
			var search = (
				<span className="title-bar-center">
					<div className="topSearchBar hide-for-small-only">
						<input type="text" placeholder="Search" onChange={this.fullTextSearch} />
					</div>
				</span>
			);
		} else {
			var search = null;
		}
		if (this.state.showUser === true) {
			var user = <UserView model={this.props.user} />
		} else {
			var user = null;
		}
		var loadingDisplay = this.state.loading ? 'block' : 'none';
		return (
			<div>
			    {menu}
				<div className={"main off-canvas-content " + mainMenuClassName} data-off-canvas-content>
					<div className="title-bar">
						<div className="title-bar-left">
							<button aria-expanded="true" className="menu-icon" type="button" onClick={this.menuSwitch}></button>
							<span className="title-text">{this.state.pageTitle}</span>
						</div>
						{user}
						{search}
					</div>
					{mainView}
					<div id="google-map-wrapper">
						<div id="google-map" className="google-map"></div>
					</div>
				</div>
				<div key='dialog' className={'reveal ' + this.state.dialogSize + ' ' + this.state.dialogCustomClass} data-reveal data-options="closeOnClick: false; closeOnEsc: false;">
					{dialogView}
				</div>

				<div className="overlayer" style={{'display': loadingDisplay}}>
			        <LoadingView />
			    </div>
			</div>
		);
	}
});