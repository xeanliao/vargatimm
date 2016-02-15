define(['underscore', 'backbone', 'react', 'views/base', 'react.backbone'], function (_, Backbone, React, BaseView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		onSelectAll: function (e) {
			var collection = this.getCollection(),
			    value = e.currentTarget.checked;
			collection.forEach(function (item) {
				item.set('Selected', value);
			});
			this.forceUpdate();
		},
		onItemSelect: function (model, e) {
			model.set('Selected', e.currentTarget.checked);
			this.forceUpdate();
		},
		render: function () {
			var dmaps = this.getCollection(),
			    btnSelectedAllStatus = dmaps.every({ Selected: true }),
			    self = this;
			return React.createElement(
				'div',
				{ className: 'panel callout primary' },
				React.createElement('input', { id: 'btnCheckAllDMap', className: 'btnCheckAllDMap', type: 'checkbox',
					checked: btnSelectedAllStatus,
					onChange: this.onSelectAll }),
				React.createElement(
					'label',
					{ htmlFor: 'btnCheckAllDMap' },
					'Suppress All Distribute Maps'
				),
				React.createElement(
					'div',
					{ className: 'row small-up-1 medium-up-2 large-up-4 collapse' },
					dmaps.map(function (map) {
						return React.createElement(
							'div',
							{ className: 'column', key: 'optionDmap-' + map.get('Id') },
							React.createElement('input', { id: 'optionDmap-' + map.get('Id'), name: map.get('Id'), type: 'checkbox',
								checked: map.get('Selected'),
								onChange: self.onItemSelect.bind(null, map) }),
							React.createElement(
								'label',
								{ htmlFor: 'optionDmap-' + map.get('Id') },
								map.get('Name')
							)
						);
					})
				)
			);
		}
	});
});
