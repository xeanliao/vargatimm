define([
	'jquery',
	'react',
	'react-dom',
	'views/base',

	'react.backbone',
	'foundation'
], function ($, React, ReactDOM, BaseView, UserModel) {
	var MenuItem = React.createClass({
		mixins: [BaseView],
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
				<li>
					<a className={this.state.active ? 'active': ''} href={"#" + this.props.address}>
						<i className={this.props.icon}></i>&nbsp; {this.props.name}
					</a>
				</li>
			);
		}
	});

	return React.createClass({
		mixins: [BaseView],
		getInitialState: function(){
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
		closeMenu: function(){
			this.setState({open: false});
		},
		switch: function(){
			this.setState({open: !this.state.open});
		},
		render: function(){
			if (this.state.open) {
				$('.off-canvas-wrapper-inner').addClass('is-off-canvas-open is-open-left');
			} else {
				$('.off-canvas-wrapper-inner').removeClass('is-off-canvas-open is-open-left');
			}
			return (
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
			);
		}
	});
});

