define(['backbone', 'react', 'views/print/shared/options', 'views/print/shared/penetrationColor', 'react.backbone'], function (Backbone, React, OptionsView, PenetrationColorView) {
	return React.createBackboneClass({
		mixins: [OptionsView],
		onPenetrationColorsChanged: function (values) {
			var model = this.getModel();
			model.set('penetrationColors', values, {
				silent: true
			});
		},
		render: function () {
			var model = this.getModel();
			return React.createElement(
				'div',
				null,
				React.createElement(
					'h3',
					null,
					'Print Options'
				),
				React.createElement(
					'div',
					{ className: 'row collapse' },
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Target Method:',
							React.createElement('input', { type: 'text', name: 'targetMethod', defaultValue: model.get('targetMethod'), onChange: this.OnValueChanged })
						)
					)
				),
				React.createElement(
					'div',
					{ className: 'panel callout secondary' },
					React.createElement(
						'h6',
						null,
						'Campaign Maps'
					),
					React.createElement(
						'div',
						{ className: 'row medium-up-2 large-up-2 collapse' },
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressCover', name: 'suppressCover', type: 'checkbox', checked: model.get('suppressCover'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressCover' },
								'Suppress Cover'
							)
						),
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressCampaign', name: 'suppressCampaign', type: 'checkbox', checked: model.get('suppressCampaign'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressCampaign' },
								'Suppress Campaign Page'
							)
						),
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressSubMap', name: 'suppressSubMap', type: 'checkbox', checked: model.get('suppressSubMap'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressSubMap' },
								'Suppress Sub Maps'
							)
						),
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressCampaignSummary', name: 'suppressCampaignSummary', type: 'checkbox', checked: model.get('suppressCampaignSummary'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressCampaignSummary' },
								'Suppress Sub Map Summary'
							)
						),
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressSubMapCountDetail', name: 'suppressSubMapCountDetail', type: 'checkbox', checked: model.get('suppressSubMapCountDetail'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressSubMapCountDetail' },
								'Suppress Sub Map Croute Counts'
							)
						),
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressNDAInCampaign', name: 'suppressNDAInCampaign', type: 'checkbox', checked: model.get('suppressNDAInCampaign'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressNDAInCampaign' },
								'Suppress non-deliverables for campaign map'
							)
						),
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressNDAInSubMap', name: 'suppressNDAInSubMap', type: 'checkbox', checked: model.get('suppressNDAInSubMap'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressNDAInSubMap' },
								'Suppress non-deliverables for sub map'
							)
						),
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressLocations', name: 'suppressLocations', type: 'checkbox', checked: model.get('suppressLocations'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressLocations' },
								'Suppress Locations'
							)
						),
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressRadii', name: 'suppressRadii', type: 'checkbox', checked: model.get('suppressRadii'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressRadii' },
								'Suppress Radii'
							)
						)
					),
					React.createElement(
						'div',
						{ className: 'row small-up-1 collapse' },
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'showPenetrationColors', name: 'showPenetrationColors', type: 'checkbox', checked: model.get('showPenetrationColors'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'showPenetrationColors' },
								'Show Penetration Colors:'
							)
						),
						React.createElement(PenetrationColorView, { colors: model.get('penetrationColors'), onChange: this.onPenetrationColorsChanged })
					)
				),
				React.createElement(
					'div',
					{ className: 'small-12 columns' },
					React.createElement(
						'div',
						{ className: 'button-group float-right' },
						React.createElement(
							'button',
							{ className: 'success button', onClick: this.onApply },
							'Apply'
						),
						React.createElement(
							'button',
							{ className: 'button', onClick: this.onClose },
							'Cancel'
						)
					)
				),
				React.createElement(
					'button',
					{ onClick: this.onClose, className: 'close-button', 'data-close': true, 'aria-label': 'Close reveal', type: 'button' },
					React.createElement(
						'span',
						{ 'aria-hidden': 'true' },
						'×'
					)
				)
			);
		}
	});
});
