import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import BaseView from 'views/base';

export default React.createBackboneClass({
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
		return (
			<div className="title-bar-right">
				<a href="javascript:;" onClick={this.onLogout}>
					<span className="hide-for-small-only">
						<small>Welcome,&nbsp;{this.getModel().get('FullName')}&nbsp;&nbsp;</small>
					</span>
					<i className="fa fa-sign-out" style={{'position':'relative', 'top':'2px'}}></i>
				</a>
			</div>
		);
	}
});