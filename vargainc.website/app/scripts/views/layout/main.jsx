define([
	'jquery',
	'react',
	'react-dom',
	'views/base',
	'views/layout/menu',
	'views/layout/user',
	'views/layout/loading',

	'react.backbone',
	'foundation'
], function($, React, ReactDOM, BaseView, MenuView, UserView, LoadingView){
	return React.createBackboneClass({
		mixins: [
			BaseView,
			React.BackboneMixin("user", "change:FullName"),
		],
		getInitialState: function(){
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
		componentDidMount: function(){
			var self = this;
			/**
			 * set main view
			 * @param  {React} view
			 * @param  {Backbone.Collection} or Backbone.Model} params
			 * @param  {showMenu: {boolean}
			 */
			this.subscribe("loadView", function(view, params, options){
				self.setState({
					mainView: view,
					mainParams: params
				});
				self.setState({
					pageTitle: _.has(options, 'pageTitle') ? options.pageTitle : 'TIMM System',
					showMenu: _.has(options, 'showMenu') ? options.showMenu : true,
					showSearch: _.has(options, 'showSearch') ? options.showSearch : true,
					showUser: _.has(options, 'showUser') ? options.showUser : true
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
					dialogParams: params,
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
			this.subscribe('showLoading', function(){
				loadingCount++;
				window.clearTimeout(loadingTimeout);
				loadingTimeout = window.setTimeout(function(){
					self.setState({loading: true});
				}, loadingDelay);
				
			});
			this.subscribe('hideLoading', function(){
				loadingCount--;
				window.setTimeout(function(){
					if(loadingCount <= 0){
						window.clearTimeout(loadingTimeout);
						self.setState({loading: false});
						loadingCount = 0;
					}
				}, 300);
			});
			/**
			 * fix main view size
			 */
			$(window).on('resize', function(){
				$(".off-canvas-wrapper-inner").height($(window).height());
			});
			$(window).trigger('resize');
		},
		componentDidUpdate: function(prevProps, prevState){
			if(this.state.dialogView && Foundation){
				$('.reveal').foundation();
				$('.reveal').foundation('open');
			}else{
				$('.reveal').foundation();
				$('.reveal').foundation('close');
			}
			$('iframe').height($(window).height());
		},
		fullTextSearch: function(e){
			this.publish('search', e.currentTarget.value);
		},
		menuSwitch: function(){
			this.refs.sideMenu && this.refs.sideMenu.switch();
		},
		/**
		 * build main view
		 */
		getMainView: function(){
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
		onCloseDialog: function(){
			this.publish('showDialog');
		},
		/**
         * build dialog view
         */
		getDialogView: function(){
			if (this.state.dialogView) {
				if (_.isString(this.state.dialogView)) {
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
						params = _.extend(this.state.dialogParams, {
							ref: "DialogView"
						});
					return DialogView(params);
				}
			}
			return null;
		},
		render: function() {
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
				var user = <UserView model={this.props.user} / >
			} else {
				var user = null;
			}

	        return (
	        	<div>
					<div className="off-canvas-wrapper">
		        		<div className='off-canvas-wrapper-inner' data-off-canvas-wrapper>
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
								<div id="google-map" className="google-map"></div>
							</div>
						</div>
					</div>
					
					<div className={'reveal ' + this.state.dialogSize + ' ' + this.state.dialogCustomClass} data-reveal data-options="closeOnClick: false; closeOnEsc: false">
						{dialogView}
					</div>

					<div className="overlayer" style={{'display': this.state.loading ? 'block' : 'none'}}>
				        <LoadingView />
				    </div>
				</div>
	        );
	    }
	});
});