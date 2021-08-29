import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import moment from 'moment';

import BaseView from 'views/base';

export default React.createBackboneClass({
	mixins: [BaseView],
	render: function () {
		var model = this.getModel(),
			date = model.get('Date'),
			displayDate = date ? moment(model.get('Date')).format("MMM DD, YYYY") : '';
		return (
			<div className="footer">
				<div className="left">
					<div className="vargainc-logo"></div>
				</div>
				<div className="center">
					<ul className="no-bullet">
						<li>
							<span>{'MC#:' + model.get('DisplayName')}</span>
							<span>www.vargainc.com</span>
						</li>
						<li>
							<span>{'Created on:' + displayDate}</span>
							<span>PH:949-768-1500</span>
						</li>
						<li>
							<span>{'Created for:' + model.get('ContactName')}</span>
							<span>FX:949-768-1501</span>
						</li>
						<li>
							<span>{'Created by:' + model.get('CreatorName')}</span>
							<span>&copyright;2010 Varga Media Solutions,Inc.All rights reserved.</span>
						</li>
					</ul>
				</div>
				<div className="right">
					<div className="timm-logo"></div>
				</div>
			</div>
		);
	}
});