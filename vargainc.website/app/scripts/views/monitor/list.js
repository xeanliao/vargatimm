define(['jquery', 'underscore', 'moment', 'backbone', 'react', 'pubsub', 'views/monitor/dismiss', 'views/monitor/edit', 'models/task', 'react.backbone'], function ($, _, moment, Backbone, React, Topic, DismissView, EditView, TaskModel) {
	var actionHandle = null,
	    searchHandle = null;

	var MonitorRow = React.createBackboneClass({
		menuKey: 'monitor-menu-ddl-',
		getDefaultProps: function () {
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		componentDidMount: function () {
			var menuKey = this.menuKey;
			_.forEach(this.getModel().get('Tasks'), function (task) {
				$("#" + menuKey + task.Id).foundation();
			});
		},
		componentDidUpdate: function () {},
		onDismiss: function (e) {
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel();
			var confirmResult = confirm('Are you sure you want to delete all selected Tasks?');
			if (confirmResult) {
				Topic.publish('showDialog', DismissView, null, null);
				actionHandler && Topic.unsubscribe(actionHandler);
				actionHandler = Topic.subscribe('monitor/dismiss', function (user) {
					model.dismissToDMap(user, {
						success: function (result) {
							Topic.publish('showDialog', null, null, null);
							if (result && result.success) {
								Topic.publish('monitor/refresh');
							} else {
								alert("something wrong");
							}
						}
					});
				});
			}
		},
		onFinished: function (taskId, reactEvent, reactNumber, evt) {
			evt.preventDefault();
			evt.stopPropagation();

			var model = new TaskModel({ Id: taskId });
			model.markFinished({
				success: function (result) {
					if (result && result.success) {
						Topic.publish('monitor/refresh');
					} else {
						alert(result.error);
					}
				}
			});
		},
		onOpenUploadFile: function (taskId) {
			$("#upload-file-" + taskId).click();
		},
		onImport: function (taskId, e) {
			e.bubbles = false;
			var uploadFile = e.currentTarget.files[0];
			if (uploadFile.size == 0) {
				alert('please select an not empty file!');
				return;
			}
			var model = new TaskModel({ Id: taskId });
			model.importGtu(uploadFile, {
				success: function (result) {
					$("#upload-file-" + taskId).val('');
					if (result && result.success) {
						Topic.publish('monitor/refresh');
					}
					if (result && result.error && result.error.length > 0) {
						alert(result.error.join('\r\n'));
					}
				}
			});
		},
		onEdit: function (taskId, e) {
			e.preventDefault();
			e.stopPropagation();
			var model = new TaskModel({ Id: taskId });
			model.fetch().then(function () {
				Topic.publish('showDialog', EditView, null, model);
			});
		},
		onGotoMonitor: function (taskId, e) {
			window.location.hash = 'frame/taskMonitor.aspx?taskid=' + taskId;
		},
		onOpenMoreMenu: function (e) {
			e.preventDefault();
			e.stopPropagation();
		},
		onCloseMoreMenu: function (key) {
			$("#" + this.menuKey + key).foundation('close');
		},
		renderMoreMenu: function (key) {
			var id = this.menuKey + key;
			return React.createElement(
				'span',
				null,
				React.createElement(
					'button',
					{ className: 'button cirle', 'data-toggle': id, onClick: this.onOpenMoreMenu },
					React.createElement('i', { className: 'fa fa-ellipsis-h' })
				),
				React.createElement(
					'div',
					{ id: id, className: 'dropdown-pane bottom',
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false',
						onClick: this.onCloseMoreMenu.bind(null, key) },
					React.createElement(
						'ul',
						{ className: 'vertical menu' },
						React.createElement(
							'li',
							null,
							React.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onEdit.bind(null, key) },
								React.createElement('i', { className: 'fa fa-edit' }),
								React.createElement(
									'span',
									null,
									'Edit'
								)
							)
						),
						React.createElement(
							'li',
							null,
							React.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onOpenUploadFile.bind(null, key) },
								React.createElement('i', { className: 'fa fa-cloud-upload' }),
								React.createElement(
									'span',
									null,
									'Import'
								)
							)
						),
						React.createElement('input', { type: 'file', id: 'upload-file-' + key, multiple: true, style: { 'display': 'none' }, onChange: this.onImport.bind(null, key) })
					)
				)
			);
		},
		render: function () {
			var model = this.getModel(),
			    date = model.get('Date'),
			    displayDate = date ? moment(date).format("MMM DD, YYYY") : '',
			    self = this;
			return React.createElement(
				'div',
				{ className: 'row scroll-list-item' },
				React.createElement(
					'div',
					{ className: 'hide-for-small-only medium-2 columns' },
					model.get('ClientName')
				),
				React.createElement(
					'div',
					{ className: 'small-12 medium-5 columns' },
					model.get('ClientCode')
				),
				React.createElement(
					'div',
					{ className: 'hide-for-small-only medium-2 columns' },
					displayDate
				),
				React.createElement(
					'div',
					{ className: 'small-12 medium-3 columns' },
					React.createElement(
						'span',
						{ className: 'show-for-large' },
						model.get('AreaDescription')
					),
					React.createElement(
						'div',
						{ className: 'float-right tool-bar' },
						React.createElement(
							'a',
							{ onClick: this.onDismiss, className: 'button row-button' },
							React.createElement('i', { className: 'fa fa-reply' }),
							React.createElement(
								'small',
								null,
								'Dismiss'
							)
						)
					)
				),
				React.createElement(
					'div',
					{ className: 'small-12 columns' },
					React.createElement(
						'table',
						{ className: 'hover' },
						React.createElement(
							'colgroup',
							null,
							React.createElement('col', null),
							React.createElement('col', { style: { 'width': "160px" } })
						),
						React.createElement(
							'tbody',
							null,
							model.get('Tasks').map(function (task) {
								if (task.visiable === false) {
									return null;
								}
								return React.createElement(
									'tr',
									{ key: task.Id },
									React.createElement(
										'td',
										{ onClick: self.onGotoMonitor.bind(null, task.Id) },
										task.Name
									),
									React.createElement(
										'td',
										null,
										React.createElement(
											'div',
											{ className: 'float-right tool-bar' },
											React.createElement(
												'button',
												{ onClick: self.onFinished.bind(null, task.Id), className: 'button' },
												React.createElement('i', { className: 'fa fa-check' }),
												React.createElement(
													'small',
													null,
													'Finish'
												)
											),
											self.renderMoreMenu(task.Id)
										)
									)
								);
							})
						)
					)
				)
			);
		}
	});
	var MonitorList = React.createBackboneClass({
		getInitialState: function () {
			return {
				orderByFiled: null,
				orderByAsc: false,
				search: null,
				filterField: null,
				filterValues: []
			};
		},
		componentDidMount: function () {
			var self = this;
			Topic.subscribe('monitor/refresh', function () {
				self.getCollection().fetchForTask();
			});
			searchHandle && Topic.unsubscribe(searchHandle);
			searchHandle = Topic.subscribe('search', function (words) {
				console.log('on search');
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			$("#monitor-filter-ddl-ClientName, #monitor-filter-ddl-ClientCode, #monitor-filter-ddl-Date, #monitor-filter-ddl-AreaDescription").foundation();
		},
		componentWillUnmount: function () {
			searchHandle && Topic.unsubscribe(searchHandle);
			actionHandle && Topic.unsubscribe(actionHandle);
		},
		onOrderBy: function (field, reactObj, reactId, e) {
			e.preventDefault();
			e.stopPropagation();
			if (this.state.orderByFiled == field) {
				this.setState({ orderByAsc: !this.state.orderByAsc });
			} else {
				this.setState({
					orderByFiled: field,
					orderByAsc: true
				});
			}
		},
		onFilter: function (field, e) {
			e.preventDefault();
			e.stopPropagation();
			var els = $(":checked", e.currentTarget),
			    values = _.map(els, function (item) {
				return $(item).val();
			});

			this.setState({
				filterField: values.length > 0 ? field : null,
				filterValues: values.length > 0 ? values : [],
				search: null
			});
			$('#monitor-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function (field, e) {
			e.preventDefault();
			e.stopPropagation();
			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
			$('#monitor-filter-ddl-' + field).foundation('close');
			$(e.currentTarget).closest('form').get(0).reset();
		},
		renderHeader: function (field, displayName) {
			var dataSource = this.getCollection(),
			    sortIcon = null,
			    filterIcon = null,
			    filterMenu = null;
			dataSource = dataSource ? dataSource.toArray() : [];

			if (this.state.orderByFiled == field) {
				if (this.state.orderByAsc) {
					sortIcon = React.createElement('i', { className: 'fa fa-sort-up active' });
				} else {
					sortIcon = React.createElement('i', { className: 'fa fa-sort-down active' });
				}
			} else {
				sortIcon = React.createElement('i', { className: 'fa fa-sort' });
			}
			sortIcon = React.createElement(
				'a',
				{ onClick: this.onOrderBy.bind(null, field) },
				' ',
				sortIcon
			);

			if (this.state.filterField && this.state.filterField == field) {
				filterIcon = React.createElement(
					'a',
					{ 'data-toggle': "monitor-filter-ddl-" + field },
					' ',
					React.createElement('i', { className: 'fa fa-filter active' })
				);
			}
			if (dataSource) {
				var fieldValues = _.map(dataSource, function (i) {
					var fieldValue = i.get(field);
					var dateCheck = moment(fieldValue, moment.ISO_8601);
					if (dateCheck.isValid()) {
						return dateCheck.format("MMM DD, YYYY");
					}
					return fieldValue;
				});
				var menuItems = _.uniq(fieldValues).sort();
				filterMenu = React.createElement(
					'div',
					{ id: "monitor-filter-ddl-" + field,
						className: 'dropdown-pane bottom',
						style: { width: 'auto' },
						'data-dropdown': true,
						'data-close-on-click': 'true',
						'data-auto-focus': 'false' },
					React.createElement(
						'form',
						{ onSubmit: this.onFilter.bind(this, field) },
						React.createElement(
							'ul',
							{ className: 'vertical menu' },
							menuItems.map(function (item, index) {
								return React.createElement(
									'li',
									{ key: field + index },
									React.createElement('input', { id: field + index, name: field, type: 'checkbox', value: item }),
									React.createElement(
										'label',
										{ htmlFor: field + index },
										item
									)
								);
							})
						),
						React.createElement(
							'button',
							{ className: 'button tiny success', type: 'submit' },
							'Filter'
						),
						React.createElement(
							'button',
							{ className: 'button tiny warning', type: 'reset', onClick: this.onClearFilter.bind(this, field) },
							'Clear'
						)
					)
				);
			}

			return React.createElement(
				'div',
				null,
				React.createElement(
					'a',
					{ 'data-toggle': "monitor-filter-ddl-" + field },
					displayName
				),
				sortIcon,
				filterIcon,
				filterMenu
			);
		},
		getDataSource: function () {
			var dataSource = this.getCollection();
			dataSource = dataSource ? dataSource.toArray() : [];
			if (this.state.orderByFiled) {
				dataSource = _.orderBy(dataSource, ['attributes.' + this.state.orderByFiled], [this.state.orderByAsc ? 'asc' : 'desc']);
			}
			if (this.state.search) {
				var keyword = this.state.search.toLowerCase(),
				    campaignValues = null,
				    campaignSearch = null,
				    taskValues = null,
				    taskSearch = null;
				dataSource = _.filter(dataSource, function (i) {
					campaignValues = _.values(i.attributes);
					campaignSearch = _.some(campaignValues, function (i) {
						var dateCheck = moment(i, moment.ISO_8601);
						if (dateCheck.isValid()) {
							return dateCheck.format("MMM DD YYYY MMM DD, YYYY YYYY-MM-DD MM/DD/YYYY YYYY MM MMM DD").toLowerCase().indexOf(keyword) > -1;
						}
						return _.toString(i).toLowerCase().indexOf(keyword) > -1;
					});
					/**
      * update task visiable logical.
      * if campaign in search keyward. show all task
      * otherwise only show task name in search word.
      * if there is no task need show hide this campaign.
      */
					taskValues = _.values(i.attributes.Tasks);
					_.forEach(taskValues, function (i) {
						i.visiable = campaignSearch || i.Name.toLowerCase().indexOf(keyword) > -1;
					});
					return campaignSearch || _.some(taskValues, { visiable: true });
				});
			} else if (this.state.filterField && this.state.filterValues) {
				var filterField = this.state.filterField,
				    filterValues = this.state.filterValues;
				dataSource = _.filter(dataSource, function (i) {
					var fieldValue = i.get(filterField);
					var dateCheck = moment(fieldValue, moment.ISO_8601);
					if (dateCheck.isValid()) {
						return _.indexOf(filterValues, dateCheck.format("MMM DD, YYYY")) > -1;
					}
					return _.indexOf(filterValues, fieldValue) > -1;
				});
			}
			return dataSource;
		},
		render: function () {
			var list = this.getDataSource();
			return React.createElement(
				'div',
				{ className: 'section row' },
				React.createElement(
					'div',
					{ className: 'small-12 columns' },
					React.createElement(
						'div',
						{ className: 'section-header' },
						React.createElement(
							'div',
							{ className: 'row' },
							React.createElement(
								'div',
								{ className: 'small-12 column' },
								React.createElement(
									'h5',
									null,
									'GPS Monitor'
								)
							)
						)
					),
					React.createElement(
						'div',
						{ className: 'scroll-list-section-body' },
						React.createElement(
							'div',
							{ className: 'row scroll-list-header' },
							React.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('ClientName', 'Client Name')
							),
							React.createElement(
								'div',
								{ className: 'small-12 medium-5 columns' },
								this.renderHeader('ClientCode', 'Client Code')
							),
							React.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('Date', 'Date')
							),
							React.createElement(
								'div',
								{ className: 'small-12 medium-3 columns' },
								React.createElement(
									'span',
									{ className: 'show-for-large' },
									this.renderHeader('AreaDescription', 'Type')
								)
							)
						),
						list.map(function (item) {
							return React.createElement(MonitorRow, { key: item.get('Id'), model: item });
						})
					)
				)
			);
		}
	});
	return MonitorList;
});
