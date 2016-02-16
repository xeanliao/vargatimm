define(['backbone', 'react', 'views/print/shared/options', 'views/print/shared/optionsDMapSelector', 'react.backbone'], function (Backbone, React, OptionsView, OptionsDMapSelector) {
	return React.createBackboneClass({
		mixins: [OptionsView],
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
					{ className: 'panel callout secondary' },
					React.createElement(
						'h6',
						null,
						'Distribution Maps'
					),
					React.createElement(
						'div',
						{ className: 'row medium-up-1 large-up-2 collapse' },
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressDMap', name: 'suppressDMap', type: 'checkbox', checked: model.get('suppressDMap'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressDMap' },
								'Suppress Distribution Maps'
							)
						),
						React.createElement(
							'div',
							{ className: 'column' },
							React.createElement('input', { id: 'suppressNDAInDMap', name: 'suppressNDAInDMap', type: 'checkbox', checked: model.get('suppressNDAInDMap'), onChange: this.OnValueChanged }),
							React.createElement(
								'label',
								{ htmlFor: 'suppressNDAInDMap' },
								'Suppress non-deliverables for distribution map'
							)
						)
					)
				),
				React.createElement(OptionsDMapSelector, { collection: model.get('DMaps') }),
				React.createElement(
					'div',
					{ className: 'small-12 columns' },
					React.createElement(
						'div',
						{ className: 'button-group float-right' },
						React.createElement(
							'button',
							{ className: 'success button', onClick: this.onApply },
							'Save'
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
