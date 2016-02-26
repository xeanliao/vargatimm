define(['moment', 'backbone', 'react', 'views/base', 'react.backbone'], function (moment, Backbone, React, BaseView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		render: function () {
			var model = this.getModel(),
			    date = model.get('Date'),
			    displayDate = date ? moment(model.get('Date')).format("MMM DD, YYYY") : '';
			return React.createElement(
				'div',
				{ className: 'footer' },
				React.createElement(
					'div',
					{ className: 'left' },
					React.createElement('div', { className: 'vargainc-logo' })
				),
				React.createElement(
					'div',
					{ className: 'center' },
					React.createElement(
						'ul',
						{ className: 'no-bullet' },
						React.createElement(
							'li',
							null,
							React.createElement(
								'span',
								null,
								'MC#:' + model.get('DisplayName')
							),
							React.createElement(
								'span',
								null,
								'www.vargainc.com'
							)
						),
						React.createElement(
							'li',
							null,
							React.createElement(
								'span',
								null,
								'Created on:' + displayDate
							),
							React.createElement(
								'span',
								null,
								'PH:949-768-1500'
							)
						),
						React.createElement(
							'li',
							null,
							React.createElement(
								'span',
								null,
								'Created for:' + model.get('ContactName')
							),
							React.createElement(
								'span',
								null,
								'FX:949-768-1501'
							)
						),
						React.createElement(
							'li',
							null,
							React.createElement(
								'span',
								null,
								'Created by:' + model.get('CreatorName')
							),
							React.createElement(
								'span',
								null,
								'&copyright;2010 Varga Media Solutions,Inc.All rights reserved.'
							)
						)
					)
				),
				React.createElement(
					'div',
					{ className: 'right' },
					React.createElement('div', { className: 'timm-logo' })
				)
			);
		}
	});
});
