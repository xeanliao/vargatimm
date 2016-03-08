define(['underscore', 'moment', 'sprintf', 'backbone', 'react', 'views/base', 'models/task', 'react.backbone'], function (_, moment, helper, Backbone, React, BaseView, TaskModel) {
	var ReportRow = React.createBackboneClass({
		mixins: [BaseView],
		menuKey: 'report-menu-ddl-',
		getDefaultProps: function () {
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		componentDidMount: function () {
			$('.has-tip').foundation();
		},
		componentDidUpdate: function () {},
		onReOpenTask: function (taskId) {
			var confirmResult = confirm('Do you really want to move report back to GPS Montor?'),
			    self = this;
			if (confirmResult) {
				var model = new TaskModel({
					Id: taskId
				});
				model.reOpen({
					success: function (result) {
						if (result && result.success) {
							self.publish('report/refresh');
						} else {
							alert(result.error);
						}
					}
				});
			}
		},
		onGotoReport: function (taskId) {
			window.location.hash = 'frame/ReportsTask.aspx?tid=' + taskId;
		},
		onGotoReview: function (campaignId, taskName, taskId) {
			window.location.hash = helper.sprintf('campaign/%d/%s/%d/monitor', campaignId, taskName, taskId);
			// window.location.hash = 'frame/EditGTU.aspx?id=' + taskId;
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
					{ className: 'button cirle', 'data-toggle': id },
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
								{ href: 'javascript:;', onClick: this.onEdit },
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
					{ className: 'small-10 medium-5 columns' },
					model.get('ClientCode')
				),
				React.createElement(
					'div',
					{ className: 'small-2 medium-2 columns' },
					displayDate
				),
				React.createElement(
					'div',
					{ className: 'small-2 medium-3 columns' },
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
							{ className: 'button row-button', href: "#frame/handler/PhantomjsPrintHandler.ashx?campaignId=" + model.get('Id') + "&type=print" },
							React.createElement('i', { className: 'fa fa-print' }),
							React.createElement(
								'small',
								null,
								'Print'
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
								var campaignId = model.get('Id'),
								    taskName = task.Name;
								return React.createElement(
									'tr',
									{ key: task.Id },
									React.createElement(
										'td',
										{ onDoubleClick: self.onGotoReview.bind(null, campaignId, taskName, task.Id) },
										task.Name
									),
									React.createElement(
										'td',
										null,
										React.createElement(
											'div',
											{ className: 'float-right tool-bar' },
											React.createElement(
												'a',
												{ className: 'button row-button', onClick: self.onGotoReport.bind(null, task.Id) },
												React.createElement('i', { className: 'fa fa-file-text-o' }),
												React.createElement(
													'small',
													null,
													'Report'
												)
											),
											React.createElement(
												'button',
												{ onClick: self.onReOpenTask.bind(null, task.Id),
													className: 'button has-tip top', title: 'dismiss',
													'data-tooltip': true, 'aria-haspopup': 'true',
													'data-disable-hover': 'false', tabIndex: '1' },
												React.createElement('i', { className: 'fa fa-reply' })
											)
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
	return React.createBackboneClass({
		mixins: [BaseView],
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
			this.subscribe('report/refresh', function () {
				self.getCollection().fetchForReport();
			});
			this.subscribe('search', function (words) {
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			$("#report-filter-ddl-ClientName, #report-filter-ddl-ClientCode, #report-filter-ddl-Date, #report-filter-ddl-AreaDescription").foundation();
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
			$('#report-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function (field, e) {
			e.preventDefault();
			e.stopPropagation();
			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
			$('#report-filter-ddl-' + field).foundation('close');
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
					{ 'data-toggle': "report-filter-ddl-" + field },
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
					{ id: "report-filter-ddl-" + field,
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
					{ 'data-toggle': "report-filter-ddl-" + field },
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
									'Reports'
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
								{ className: 'small-10 medium-5 columns' },
								this.renderHeader('ClientCode', 'Client Code')
							),
							React.createElement(
								'div',
								{ className: 'hide-for-small-only medium-2 columns' },
								this.renderHeader('Date', 'Date')
							),
							React.createElement(
								'div',
								{ className: 'small-2 medium-3 columns' },
								React.createElement(
									'span',
									{ className: 'show-for-large' },
									this.renderHeader('AreaDescription', 'Type')
								)
							)
						),
						list.map(function (item) {
							return React.createElement(ReportRow, { key: item.get('Id'), model: item });
						})
					)
				)
			);
		}
	});
});
