define(['moment', 'backbone', 'react', 'views/base', 'views/print/shared/footer', 'react.backbone'], function (moment, Backbone, React, BaseView, FooterView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getExportParamters: function () {
			var model = this.getModel();
			return {
				'type': 'cover',
				'options': [{
					'title': 'This Custom Campaign is Presented to:'
				}, {
					'key': '',
					'image': model.get('Logo') || '',
					'height': '80',
					'align': 'center',
					'top': '40',
					'bottom': '40'
				}, {
					'key': 'Client Name:',
					'text': model.get('ClientName')
				}, {
					'key': 'Created for:',
					'text': model.get('ContactName')
				}, {
					'key': 'Created on:',
					'text': model.get('Date') ? moment(model.get('Date')).format("MMM DD, YYYY") : ''
				}, {
					'key': ' ',
					'text': ' '
				}, {
					'key': 'Presented by:',
					'image': 'images/vargainc-logo.png',
					'height': '40',
					'align': 'center',
					'top': '10',
					'bottom': '10'
				}, {
					'key': 'Master Campaign #:',
					'text': model.get('DisplayName')
				}, {
					'key': 'Created by:',
					'text': model.get('CreatorName')
				}]

			};
		},
		render: function () {
			var model = this.getModel(),
			    displayDate = model.get('Date') ? moment(model.get('Date')).format("MMM DD, YYYY") : '';
			if (model.get('Logo')) {
				var clientLog = React.createElement('img', { className: 'client-logo', src: model.get('Logo') });
			} else {
				var clientLog = React.createElement('div', { className: 'client-logo' });
			}

			return React.createElement(
				'div',
				{ className: 'page cover' },
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center title' },
						React.createElement(
							'span',
							{ className: 'editable', role: 'title' },
							'This Custom Campaign is Presented to:'
						)
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						clientLog
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Client Name:'
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						model.get('ClientName')
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Created for:'
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						model.get('ContactName')
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Created on:'
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						displayDate
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						' '
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						' '
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Presented by:'
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						React.createElement('div', { className: 'vargainc-log' })
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Master Campaign #:'
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						model.get('DisplayName')
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						'Created by:'
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						model.get('CreatorName')
					)
				),
				React.createElement(FooterView, { model: model.get('Footer') })
			);
		}
	});
});
