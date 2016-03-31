define(['moment', 'backbone', 'react', 'views/base', 'react.backbone'], function (moment, Backbone, React, BaseView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		componentDidMount: function () {
			var self = this,
			    model = this.getModel();

			$('#birthdayDatePicker').fdatepicker({
				format: 'yyyy-mm-dd'
			}).on('changeDate', function (e) {
				self.getModel().set('DateOfBirth', e.date);
			});

			$(this.refs.companySelector).select2();

			$('form').foundation();
		},
		componentWillUnmount: function () {
			$(companySelector).select2('destroy');
			$('#birthdayDatePicker').off('changeDate').fdatepicker('remove');
		},
		onSave: function (e) {
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel(),
			    file = this.refs.employeePicture.files.length > 0 ? this.refs.employeePicture.files[0] : null,
			    self = this;
			model.set('CompanyId', $(this.refs.companySelector).val());
			model.addEmployee(file).done(function () {
				self.publish("showDialog");
			});
		},
		onClose: function () {
			this.publish("showDialog");
		},
		onChange: function (e) {
			this.getModel().set(e.currentTarget.name, e.currentTarget.value);
		},
		render: function () {
			var showError = this.state && this.state.error ? true : false;
			var errorMessage = showError ? this.state.error : "";
			return React.createElement(
				'form',
				{ 'data-abide': true, onSubmit: this.onSave },
				React.createElement(
					'h3',
					null,
					'Add Employee'
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
							'Distributor'
						),
						React.createElement(
							'select',
							{ ref: 'companySelector' },
							this.props.company.models.map(function (item) {
								return React.createElement(
									'option',
									{ key: item.get('Id'), value: item.get('Id') },
									item.get('Name')
								);
							})
						),
						React.createElement(
							'span',
							{ className: 'form-error' },
							'it is required.'
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Full Name',
							React.createElement('input', { onChange: this.onChange, name: 'FullName', type: 'text', required: true })
						)
					),
					React.createElement(
						'fieldset',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Role'
						),
						React.createElement('input', { type: 'radio', onChange: this.onChange, name: 'Role', value: 'Walker', id: 'walker' }),
						React.createElement(
							'label',
							{ htmlFor: 'walker' },
							'Walker'
						),
						React.createElement('input', { type: 'radio', onChange: this.onChange, name: 'Role', value: 'Driver', id: 'driver' }),
						React.createElement(
							'label',
							{ htmlFor: 'driver' },
							'Driver'
						),
						React.createElement('input', { type: 'radio', onChange: this.onChange, name: 'Role', value: 'Auditor', id: 'autitor' }),
						React.createElement(
							'label',
							{ htmlFor: 'autitor' },
							'Auditor'
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Cell Phone',
							React.createElement('input', { onChange: this.onChange, name: 'CellPhone', type: 'text' })
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 medium-12 large-4 columns end' },
						React.createElement(
							'label',
							null,
							'Birthday',
							React.createElement('input', { id: 'birthdayDatePicker', className: 'fdatepicker', onChange: this.onChange, name: 'DateOfBirth', type: 'date', readOnly: true })
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Photo',
							React.createElement('input', { ref: 'employeePicture', name: 'picture', type: 'file' })
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Notes',
							React.createElement('textarea', { onChange: this.onChange, name: 'Notes' })
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
