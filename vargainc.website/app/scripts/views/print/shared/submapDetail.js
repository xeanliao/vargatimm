define(['numeral', 'backbone', 'react', 'views/base', 'views/print/shared/footer', 'react.backbone'], function (Numeral, Backbone, React, BaseView, FooterView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		componentDidMount: function () {
			this.getModel().fetchBySubMap({ quite: true });
		},
		getExportParamters: function () {
			var model = this.getModel();
			return {
				"type": "submapDetail",
				"options": [{
					"title": 'CARRIER ROUTES CONTAINED IN SUBM MAP ' + model.get('OrderId') + ' (' + model.get('Name') + ')'
				}, {
					"table": "submap-detail",
					"submapId": model.get('SubMapId')
				}]
			};
		},
		render: function () {
			var model = this.getModel(),
			    croutes = model.get('CRoutes') ? model.get('CRoutes') : [];

			return React.createElement(
				'div',
				{ className: 'page submap-detail' },
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'CARRIER ROUTES CONTAINED IN SUBM MAP ',
						model.get('OrderId'),
						' (',
						model.get('Name'),
						')'
					)
				),
				React.createElement(
					'div',
					{ className: 'row collapse' },
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'table',
							null,
							React.createElement(
								'thead',
								null,
								React.createElement(
									'tr',
									null,
									React.createElement(
										'th',
										null,
										'#'
									),
									React.createElement(
										'th',
										null,
										'CARRIER ROUTE #'
									),
									React.createElement(
										'th',
										null,
										'TOTAL H/H'
									),
									React.createElement(
										'th',
										null,
										'TARGET H/H'
									),
									React.createElement(
										'th',
										null,
										'PENETRATION'
									)
								)
							),
							React.createElement(
								'tbody',
								null,
								croutes.map(function (item, index) {
									return React.createElement(
										'tr',
										{ key: item.Name },
										React.createElement(
											'td',
											null,
											index
										),
										React.createElement(
											'td',
											null,
											item.Name
										),
										React.createElement(
											'td',
											null,
											Numeral(item.TotalHouseHold).format('0,0')
										),
										React.createElement(
											'td',
											null,
											Numeral(item.TargetHouseHold).format('0,0')
										),
										React.createElement(
											'td',
											null,
											Numeral(item.Penetration).format('0.00%')
										)
									);
								})
							)
						)
					)
				),
				React.createElement(FooterView, { model: model.get('Footer') })
			);
		}
	});
});
