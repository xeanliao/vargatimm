define(['moment', 'backbone', 'react', 'views/base', 'react.backbone'], function (moment, Backbone, React, BaseView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		componentDidMount: function () {
			var self = this,
			    model = this.getModel();

			if (!model.get('Date')) {
				console.log('no init date in campaign');
				model.set('Date', new Date());
			}

			$('.fdatepicker').fdatepicker({
				format: 'yyyy-mm-dd'
			}).on('changeDate', function (e) {
				self.getModel().set('Date', e.date);
			});
			$('form').foundation();
		},
		onSave: function (e) {
			e.preventDefault();
			e.stopPropagation();
			var self = this;
			this.getModel().save(null, {
				success: function (model, response) {
					if (response && response.success) {
						self.publish('camapign/refresh');
						self.onClose();
					} else {
						self.setState({ error: "opps! something wrong. please contact us!" });
					}
				},
				error: function () {
					self.setState({ error: "opps! something wrong. please contact us!" });
				}
			});
		},
		onClose: function () {
			$('.fdatepicker').off('changeDate').fdatepicker('remove');
			this.publish("showDialog");
		},
		onChange: function (e) {
			this.getModel().set(e.currentTarget.name, e.currentTarget.value);
		},
		render: function () {
			var model = this.getModel();
			var title = model.get('Id') ? 'Edit Campaign' : 'New Campaign';
			var AreaDescription = model.get('AreaDescription');
			var date = model.get('Date');
			var displayDate = date ? moment(date).format("YYYY-MM-DD") : '';
			var showError = this.state && this.state.error ? true : false;
			var errorMessage = showError ? this.state.error : "";
			return React.createElement(
				'form',
				{ 'data-abide': true, onSubmit: this.onSave },
				React.createElement(
					'h3',
					null,
					title
				),
				React.createElement(
					'div',
					{ 'data-abide-error': true, className: 'alert callout', style: { display: showError ? 'block' : 'none' } },
					React.createElement(
						'p',
						null,
						React.createElement('i', { className: 'fa fa-exclamation-circle' }),
						' ',
						errorMessage
					)
				),
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Client Name',
							React.createElement('input', { onChange: this.onChange, name: 'ClientName', type: 'text', defaultValue: model.get('ClientName'), required: true }),
							React.createElement(
								'span',
								{ className: 'form-error' },
								'it is required.'
							)
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Contact Name',
							React.createElement('input', { onChange: this.onChange, name: 'ContactName', type: 'text', defaultValue: model.get('ContactName'), required: true })
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Client Code',
							React.createElement('input', { onChange: this.onChange, name: 'ClientCode', type: 'text', defaultValue: model.get('ClientCode'), required: true })
						)
					),
					React.createElement(
						'fieldset',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Total Type'
						),
						React.createElement('input', { type: 'radio', onChange: this.onChange, name: 'AreaDescription', checked: "APT + HOME" == AreaDescription, value: 'APT + HOME', id: 'apthome' }),
						React.createElement(
							'label',
							{ htmlFor: 'apthome' },
							'APT + HOME'
						),
						React.createElement('input', { type: 'radio', onChange: this.onChange, name: 'AreaDescription', checked: "APT ONLY" == AreaDescription, value: 'APT ONLY', id: 'aptonly' }),
						React.createElement(
							'label',
							{ htmlFor: 'aptonly' },
							'APT ONLY'
						),
						React.createElement('input', { type: 'radio', onChange: this.onChange, name: 'AreaDescription', checked: "HOME ONLY" == AreaDescription, value: 'HOME ONLY', id: 'homeonly' }),
						React.createElement(
							'label',
							{ htmlFor: 'homeonly' },
							'HOME ONLY'
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 medium-12 large-4 columns end' },
						React.createElement(
							'label',
							null,
							'Date',
							React.createElement('input', { className: 'fdatepicker', onChange: this.onChange, name: 'Date', type: 'date', readOnly: true, value: displayDate, required: true })
						)
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
							{ type: 'submit', className: 'success button' },
							'Save'
						),
						React.createElement(
							'a',
							{ href: 'javascript:;', className: 'button', onClick: this.onClose },
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
