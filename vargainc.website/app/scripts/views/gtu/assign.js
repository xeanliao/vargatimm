define(['jquery', 'backbone', 'react', 'views/base', 'react.backbone', 'select2', 'spectrum'], function ($, Backbone, React, BaseView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function () {
			return { editId: null };
		},
		componentDidMount: function () {
			var self = this,
			    model = this.getModel();

			$('form').foundation();
		},
		componentWillUpdate: function (newProps, newState) {
			if (this.state.editId != null && newState.editId != this.state.editId && this.refs['userSelector-' + this.state.editId]) {
				//console.log('destory');
				$(this.refs['userSelector-' + this.state.editId]).select2('destroy');
			}
		},
		componentDidUpdate: function (prevProps, prevState) {
			if (this.state.editId != null && this.state.editId != prevState.editId && this.refs['userSelector-' + this.state.editId]) {
				//console.log('init');
				$(this.refs['userSelector-' + this.state.editId]).select2();
			}
		},
		onClose: function () {
			this.publish("showDialog");
		},
		onAdd: function (gtuId) {
			this.setState({ editId: gtuId });
		},
		onSave: function (gtuId) {
			var taskId = this.props.taskId,
			    collection = this.getCollection(),
			    model = collection.get(gtuId),
			    color = $(this.refs['userColor']).val(),
			    user = $(this.refs['userSelector-' + gtuId]).val(),
			    self = this;
			model.assignGTUToTask({
				Id: gtuId,
				TaskId: taskId,
				UserColor: color,
				AuditorId: user
			}).done(function () {
				self.setState({
					editId: null
				});
			});
		},
		onCacnel: function () {
			this.setState({ editId: null });
		},
		onRemove: function (gtuId) {
			var result = confirm('Are you sure you want to remove the assignment from GTU and Employee?');
			if (result) {
				var taskId = this.props.taskId,
				    collection = this.getCollection(),
				    model = collection.get(gtuId);
				model.unassignGTUFromTask(taskId, gtuId);
			}
		},
		renderEditForm: function (gtu) {
			var user = _.groupBy(this.props.user.models, function (item) {
				return item.get('CompanyId');
			}),
			    self = this;
			return React.createElement(
				'tr',
				{ key: gtu.get('Id') },
				React.createElement(
					'td',
					null,
					gtu.get('ShortUniqueID')
				),
				React.createElement(
					'td',
					null,
					React.createElement('input', { ref: 'userColor', type: 'color', autocomplete: true, defaultValue: '#' + Math.floor(Math.random() * 16777215).toString(16) })
				),
				React.createElement(
					'td',
					{ colSpan: '2' },
					React.createElement(
						'select',
						{ ref: 'userSelector-' + gtu.get('Id') },
						_.map(user, function (v, k) {
							return React.createElement(
								'optgroup',
								{ key: k, label: v[0].get('CompanyName') },
								v.map(function (u) {
									return React.createElement(
										'option',
										{ key: u.get('UserId'), value: u.get('UserId') },
										u.get('UserName'),
										' - ',
										u.get('Role')
									);
								})
							);
						})
					)
				),
				React.createElement(
					'td',
					null,
					gtu.get('IsOnline') ? 'Online' : 'Offline'
				),
				React.createElement(
					'td',
					null,
					React.createElement(
						'button',
						{ className: 'button tiny', onClick: self.onSave.bind(self, gtu.get('Id')) },
						'Save'
					),
					React.createElement(
						'button',
						{ className: 'button tiny alert', onClick: self.onCacnel },
						'Cancel'
					)
				)
			);
		},
		render: function () {
			var collection = this.getCollection().where(function (i) {
				return i.get('IsAssignedToOther') == false;
			}),
			    showError = false,
			    errorMessage = '',
			    self = this;

			return React.createElement(
				'div',
				null,
				React.createElement(
					'h3',
					null,
					'Assign GTU'
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
					'table',
					null,
					React.createElement(
						'colgroup',
						null,
						React.createElement('col', { style: { "width": "160px" } }),
						React.createElement('col', { style: { "width": "160px" } }),
						React.createElement('col', null),
						React.createElement('col', { style: { "width": "160px" } }),
						React.createElement('col', { style: { "width": "160px" } }),
						React.createElement('col', { style: { "width": "150px" } })
					),
					React.createElement(
						'thead',
						null,
						React.createElement(
							'tr',
							null,
							React.createElement(
								'th',
								null,
								'GTU#'
							),
							React.createElement(
								'th',
								null,
								'Color'
							),
							React.createElement(
								'th',
								null,
								React.createElement(
									'div',
									{ className: 'row' },
									React.createElement(
										'div',
										{ className: 'small-6 column' },
										'Team'
									),
									React.createElement(
										'div',
										{ className: 'small-6 column' },
										'Auditor'
									)
								)
							),
							React.createElement(
								'th',
								null,
								'Role'
							),
							React.createElement(
								'th',
								null,
								'Status'
							),
							React.createElement('th', null)
						)
					),
					React.createElement(
						'tbody',
						null,
						collection.map(function (gtu) {
							var isAssign = gtu.get('IsAssign'),
							    gtuId = gtu.get('Id'),
							    assignButton = React.createElement(
								'button',
								{ className: 'button tiny', onClick: self.onAdd.bind(null, gtuId) },
								React.createElement('i', { className: 'fa fa-plus' }),
								' Assign'
							),
							    removeButton = React.createElement(
								'button',
								{ className: 'button alert tiny', onClick: self.onRemove.bind(null, gtuId) },
								React.createElement('i', { className: 'fa fa-remove' }),
								' Remove'
							),
							    colorInput = gtu.get('UserColor') ? React.createElement('div', { className: 'color-block', style: { background: gtu.get('UserColor') } }) : null;
							var actionButton = isAssign ? removeButton : assignButton;
							if (gtu.get('Id') == self.state.editId) {
								return self.renderEditForm(gtu);
							} else {
								return React.createElement(
									'tr',
									{ key: gtu.get('Id') },
									React.createElement(
										'td',
										null,
										gtu.get('ShortUniqueID')
									),
									React.createElement(
										'td',
										null,
										colorInput
									),
									React.createElement(
										'td',
										null,
										React.createElement(
											'div',
											{ className: 'row' },
											React.createElement(
												'div',
												{ className: 'small-6 column' },
												gtu.get('Company')
											),
											React.createElement(
												'div',
												{ className: 'small-6 column' },
												gtu.get('Auditor')
											)
										)
									),
									React.createElement(
										'td',
										null,
										gtu.get('Role')
									),
									React.createElement(
										'td',
										null,
										gtu.get('IsOnline') ? 'Online' : 'Offline'
									),
									React.createElement(
										'td',
										null,
										actionButton
									)
								);
							}
						})
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
