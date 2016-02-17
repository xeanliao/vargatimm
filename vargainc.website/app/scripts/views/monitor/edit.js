define(['moment', 'backbone', 'react', 'pubsub', 'react.backbone'], function (moment, Backbone, React, Topic) {
	return React.createBackboneClass({
		componentDidMount: function () {
			var self = this,
			    model = this.getModel();

			$('.fdatepicker').fdatepicker({
				format: 'yyyy-mm-dd'
			}).on('changeDate', function (e) {
				console.log(e.date);
				self.getModel().set('Date', e.date);
			});
			$('form').foundation();
		},
		onSave: function (e) {
			var self = this;
			this.getModel().save(null, {
				success: function (model, response) {
					if (response && response.success) {
						self.onClose();
					} else {
						self.setState({ error: "opps! something wrong. please contact us!" });
					}
				},
				error: function () {
					self.setState({ error: "opps! something wrong. please contact us!" });
				}
			});
			e.preventDefault();
			e.stopPropagation();
		},
		onClose: function () {
			$('.fdatepicker').off('changeDate').fdatepicker('remove');
			Topic.publish("showDialog");
		},
		onChange: function (e) {
			console.log(e.currentTarget.name);
			this.getModel().set(e.currentTarget.name, e.currentTarget.value);
		},
		render: function () {
			var model = this.getModel();
			var date = model.get('Date');
			var displayDate = date ? moment(date).format("YYYY-MM-DD") : '';
			var tel = model.get('Telephone'),
			    telphone,
			    operator;
			if (tel) {
				var telArray = tel.split('@');
				telphone = telArray[0];
				operator = '@' + telArray[1];
			}
			var showError = this.state && this.state.error ? true : false;
			var errorMessage = showError ? this.state.error : "";
			return React.createElement(
				'form',
				{ 'data-abide': true, onSubmit: this.onSave },
				React.createElement(
					'h3',
					null,
					'Edit Task'
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
							'Name',
							React.createElement('input', { type: 'text', value: model.get('Name'), readOnly: true })
						)
					),
					React.createElement(
						'div',
						{ className: 'small-4 columns end' },
						React.createElement(
							'label',
							null,
							'Distribution Date',
							React.createElement('input', { className: 'fdatepicker', onChange: this.onChange, name: 'Date', type: 'date', readOnly: true, value: displayDate })
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Select Auditor',
							React.createElement('input', { type: 'text', defaultValue: model.get('AuditorName'), readOnly: true })
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Email',
							React.createElement('input', { onChange: this.onChange, name: 'Email', type: 'text', defaultValue: model.get('Email') })
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Telephone',
							React.createElement('input', { onChange: this.onChange, name: 'Telephone', type: 'text', defaultValue: telphone })
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns' },
						React.createElement(
							'label',
							null,
							'Telecommunications Operator',
							React.createElement(
								'select',
								{ defaultValue: operator, onChange: this.onChange, name: 'Operator' },
								React.createElement(
									'option',
									{ value: '@message.alltel.com' },
									'Alltel'
								),
								React.createElement(
									'option',
									{ value: '@txt.att.net' },
									'AT&T'
								),
								React.createElement(
									'option',
									{ value: '@messaging.nextel.com' },
									'Nextel'
								),
								React.createElement(
									'option',
									{ value: '@messaging.sprintpcs.com' },
									'Sprint'
								),
								React.createElement(
									'option',
									{ value: '@tms.suncom.com' },
									'SunCom'
								),
								React.createElement(
									'option',
									{ value: '@tmomail.net' },
									'T-mobile'
								),
								React.createElement(
									'option',
									{ value: '@voicestream.net' },
									'VoiceStream'
								),
								React.createElement(
									'option',
									{ value: '@vtext.com' },
									'Verizon(text only)'
								)
							)
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
