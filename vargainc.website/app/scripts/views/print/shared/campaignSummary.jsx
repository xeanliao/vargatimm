import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import Numeral from 'numeral';
import BaseView from 'views/base';
import FooterView from 'views/print/shared/footer';

export default React.createBackboneClass({
	mixins: [BaseView],
	getExportParamters: function () {
		return {
			"type": "campaign-summary",
			"options": [{
				"title": "Summary of Sub Maps"
			}, {
				"table": "submap-list"
			}]
		};
	},
	render: function () {
		var model = this.getModel(),
			submaps = model.get('SubMaps');

		return (
			<div className="page campaign-summary">
				<div className="row">
					<div className="small-12 columns text-center title">
						Summary of Sub Maps
					</div>
				</div>
				<div className="row collapse">
					<div className="small-12 columns">
						<table>
							<thead>
								<tr>
									<th>#</th>
									<th>SUB MAP NAME</th>
									<th>TOTAL H/H</th>
									<th>TARGET H/H</th>
									<th>PENETRATION</th>
								</tr>
							</thead>
								<tbody>
								{submaps.map(function(item, index){
									var totalHouseHold = item.TotalHouseHold,
									targetHouseHold = item.TargetHouseHold,
									penetration = item.Penetration,
									displayTotalHouseHold = totalHouseHold ? Numeral(totalHouseHold).format('0,0') : '',
									displayTargetHouseHold = targetHouseHold ? Numeral(targetHouseHold).format('0,0') : '',
									displayPenetration = penetration ? Numeral(penetration).format('0.00%') : '';
									return (
										<tr key={item.OrderId}>
										<td>{item.OrderId}</td>
										<td>{item.Name}</td>
										<td>{displayTotalHouseHold}</td>
										<td>{displayTargetHouseHold}</td>
										<td>{displayPenetration}</td>
									</tr>
								);
								})}
  							</tbody>
						</table>
						</div>
				</div>
				<FooterView model={model.get('Footer')} />
			</div>
		);
	}
});