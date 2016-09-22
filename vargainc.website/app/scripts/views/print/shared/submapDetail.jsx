define([
	'numeral',
	'backbone',
	'react',
	'views/base',
	'views/print/shared/footer',
	'react.backbone'
], function (Numeral, Backbone, React, BaseView, FooterView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		componentDidMount: function () {
			this.getModel().fetchBySubMap({quite: true});
		},
		getExportParamters: function () {
			var model = this.getModel();
			return {
				"type": "submapDetail",
				"options": [{
					"title": 'CARRIER ROUTES CONTAINED IN SUB-MAP ' + model.get('OrderId') + ' (' + model.get('Name') + ')'
				}, {
					"table": "submap-detail",
					"submapId": model.get('SubMapId')
				}]
			};
		},
		render: function () {
			var model = this.getModel(),
				croutes = model.get('CRoutes') ? model.get('CRoutes') : [];

			return (
				<div className="page submap-detail">
					<div className="row">
						<div className="small-12 columns text-center title">CARRIER ROUTES CONTAINED IN SUB-MAP {model.get('OrderId')} ({model.get('Name')})</div>
					</div>
					<div className="row collapse">
						<div className="small-12 columns">
							<table>
								<thead>
									<tr>
										<th>#</th>
										<th>CARRIER ROUTE #</th>
										<th>TOTAL H/H</th>
										<th>TARGET H/H</th>
										<th>PENETRATION</th>
									</tr>
								</thead>
  								<tbody>
  								{croutes.map(function(item, index){
  									return (
  										<tr key={item.Name}>
											<td>{index}</td>
											<td>{item.Name}</td>
											<td>{Numeral(item.TotalHouseHold).format('0,0')}</td>
											<td>{Numeral(item.TargetHouseHold).format('0,0')}</td>
											<td>{Numeral(item.Penetration).format('0.00%')}</td>
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
});
