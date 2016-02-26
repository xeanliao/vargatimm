define(['numeral', 'backbone', 'react', 'views/base', 'views/print/shared/footer', 'react.backbone'], function (Numeral, Backbone, React, BaseView, FooterView) {
	return React.createBackboneClass({
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

			return React.createElement(
				'div',
				{ className: 'page campaign-summary' },
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						'Summary of Sub Maps'
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
										'SUB MAP NAME'
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
								submaps.map(function (item, index) {
									var totalHouseHold = item.TotalHouseHold,
									    targetHouseHold = item.TargetHouseHold,
									    penetration = item.Penetration,
									    displayTotalHouseHold = totalHouseHold ? Numeral(totalHouseHold).format('0,0') : '',
									    displayTargetHouseHold = targetHouseHold ? Numeral(targetHouseHold).format('0,0') : '',
									    displayPenetration = penetration ? Numeral(penetration).format('0.00%') : '';
									return React.createElement(
										'tr',
										{ key: item.OrderId },
										React.createElement(
											'td',
											null,
											item.OrderId
										),
										React.createElement(
											'td',
											null,
											item.Name
										),
										React.createElement(
											'td',
											null,
											displayTotalHouseHold
										),
										React.createElement(
											'td',
											null,
											displayTargetHouseHold
										),
										React.createElement(
											'td',
											null,
											displayPenetration
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
