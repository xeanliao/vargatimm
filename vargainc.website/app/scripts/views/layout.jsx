define([
	'jquery',
	'react',
	'react-dom',
	'pubsub',
	'models/user',

	'react.backbone',
	'foundation'
], function($, React, ReactDOM, Topic, UserModel){
	var MenuItem = React.createClass({
		getInitialState: function(){
			return {
				active: false
			};
		},
		getDefaultProps: function(){
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		render: function(){
			return (
				<li><a className={this.state.active ? 'active': ''} href={"#" + this.props.address}><i className={this.props.icon}></i>&nbsp; {this.props.name}</a></li>
			);
		}
	});

	var User = React.createBackboneClass({
		getDefaultProps: function(){
			return {
				model: new UserModel()
			}
		},
		componentDidMount: function(){
			this.getModel().fetchCurrentUser();
			Topic.subscribe("NOT_LOGIN", function(){
				window.location = "../login.html";
			});

			$("#userMenu").foundation();
		},
		onLogout: function(){
			this.getModel().logout();
		},
		render: function(){
			var model = this.getModel();
			return (
				<div className="title-bar-right">
					<a href="javascript:;" onClick={this.onLogout}>
						<span className="hide-for-small-only">
							<small>Welcome,&nbsp;{model.get('FullName')}&nbsp;&nbsp;</small>
						</span>
						<i className="fa fa-sign-out" style={{'position':'relative', 'top':'2px'}}></i>
					</a>
				</div>
			);
		}
	});

	return React.createClass({
		displayName: 'Layout',
		componentDidMount: function(){
			var self = this;
			Topic.subscribe("loadView", function(view, collection, model){
				self.setState({mainView: view, mainCollection: collection, mainModel: model});
			});
			/**
			 * show a dialog
			 * @param  {React} view
			 * @param  {Backbone.Collection} collection
			 * @param  {Backbone.Model} model
			 * @param  {String} size Foundation Reveal Size Value: tiny, small, large, full
			 */
			Topic.subscribe("showDialog", function(view, collection, model, size){
				if(view){
					self.setState({
						dialogView: view, 
						dialogCollection: collection, 
						dialogSize: size || '',
						dialogModel: model
					});
				}else{
					$('.reveal').foundation('close');
					self.setState({dialogView: null, dialogCollection: null, dialogModel: null});
				}
			});
			/**
			 * fix main view size
			 */
			$(window).on('resize', function(){
				console.log('resize', $(window).height());
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
		menuSwitch: function(){
			console.log('menu switch', this.state.isSideMenuOpen);
			var isSideMenuOpen = !this.state.isSideMenuOpen;
			this.setState({isSideMenuOpen: isSideMenuOpen});
		},
		closeMenu: function(){
			this.setState({isSideMenuOpen: false});
		},
		getDefaultProps: function(){
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
			return {menu: menu};
		},
		getInitialState: function(){
			return {
				dialogSize: 'small'
			}
		},
		fullTextSearch: function(e){
			Topic.publish('search', e.currentTarget.value);
		},
		render: function() {
			/**
			 * build main view
			 */
	        if(this.state && this.state.mainView) {
	        	if(React.isValidElement(this.state.mainView)){
	        		var mainView = this.state.mainView;
	        	}else{
	        		var MainView = React.createFactory(this.state.mainView);
	        		if(this.state.mainCollection || this.state.mainModel){
		            	var mainView = MainView({collection: this.state.mainCollection, model: this.state.mainModel});
		            }else{
		            	var mainView = MainView();
		            }
	        	}
	        }else{
	        	var mainView = React.createElement('h1', null, 'loading');
	        }
	        /**
	         * build dialog view
	         */
	        if(this.state && this.state.dialogView) {
	        	if(typeof this.state.dialogView === 'string'){
	        		var dialogView = React.createElement('h5', null, this.state.dialogView);
	        	}else if(React.isValidElement(this.state.dialogView)){
					var dialogView = this.state.dialogView;
	        	}else{
	        		var DialogView = React.createFactory(this.state.dialogView);
	            	var dialogView = DialogView({collection: this.state.dialogCollection, model: this.state.dialogModel, ref: "DialogView"});	
	        	}
	        }else{
	        	var dialogView = null;
	        }

	        var offCanvasClassName = this.state && this.state.isSideMenuOpen ? 'off-canvas-wrapper-inner is-off-canvas-open is-open-left' : 'off-canvas-wrapper-inner';
	        return (
	        	<div>
					<div className="off-canvas-wrapper">
		        		<div className={offCanvasClassName} data-off-canvas-wrapper>
			        		<div className="sidebar off-canvas position-left" data-off-canvas data-position="left">
					        	<ul className="menu vertical" onClick={this.closeMenu}>
						        	{this.props.menu.map(function(item) {
							          	return (
							          		<MenuItem key={item.key} {...item} />
						          		);
							        })}
					        		<li>
					        			<a href="#admin"><i className="fa fa-gear"></i>&nbsp; Administration</a>
					        			<ul className="submenu menu vertical">
					        				<li><a href="#frame/Users.aspx"><span>User Management</span></a></li>
					        				<li><a href="#frame/NonDeliverables.aspx"><span>Non-Deliverables</span></a></li>
					        				<li><a href="#frame/GtuAdmin.aspx?AssignNameToGTUFlag=true"><span>GTU Management</span></a></li>
					        				<li><a href="#frame/AvailableGTUList.aspx"><span>GTU Available List</span></a></li>
					        				<li><a href="#frame/AdminGtuToBag.aspx"><span>GTU bag Management </span></a></li>
					        				<li><a href="#frame/AdminGtuBagToAuditor.aspx"><span>Assign GTU-Bags to Auditors</span></a></li>
					        				<li><a href="#frame/AdminDistributorCompany.aspx"><span>Distributor Management</span></a></li>
					        			</ul>
				        			</li>
					        	</ul>
							</div>
							<div className="main off-canvas-content" data-off-canvas-content>
								<div className="title-bar">
									<div className="title-bar-left">
										<button aria-expanded="true" className="menu-icon" type="button" onClick={this.menuSwitch}></button>
										<span className="title-text">TIMM System</span>
									</div>
									<User />
									<span className="title-bar-center">
										<div className="topSearchBar hide-for-small-only">
											<input type="text" placeholder="Search" onChange={this.fullTextSearch} />
										</div>
									</span>
								</div>
								{mainView}
							</div>
						</div>
					</div>
					
					<div className={'reveal ' + this.state.dialogSize} data-reveal>
						{dialogView}
					</div>
				</div>
	        );
	    }
	});
});