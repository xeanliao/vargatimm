define(['underscore', 'moment', 'backbone', 'react', 'views/base', 'views/distribution/publish', 'views/distribution/dismiss', 'react.backbone'], function (_, moment, Backbone, React, BaseView, PublishView, DismissView) {
	var DistributionRow = React.createBackboneClass({
		mixins: [BaseView],
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
		onDismiss: function (e) {
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel(),
			    confirmResult = confirm('Are you sure you would like to move \r\n' + model.getDisplayName() + '\r\nto Campaigns? Any changes that were made will be lost.'),
			    self = this;

			if (confirmResult) {
				this.publish('showDialog', DismissView);
				this.unsubscribe('distribution/dismiss');
				this.subscribe('distribution/dismiss', function (user) {
					model.dismissToCampaign(user, {
						success: function (result) {
							self.publish('showDialog');
							if (result && result.success) {
								self.publish('distribution/refresh');
							} else {
								alert("something wrong");
							}
						}
					});
				});
			}
		},
		onPublishToMonitors: function (e) {
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel(),
			    self = this;
			this.publish('showDialog', PublishView);
			this.unsubscribe('distribution/publish');
			this.subscribe('distribution/publish', function (user) {
				model.publishToMonitor(user, {
					success: function (result) {
						self.publish('showDialog');
						if (result && result.success) {
							self.publish('distribution/refresh');
						} else {
							alert(result.error);
						}
					}
				});
			});
		},
		onGotoDMap: function (id) {
			window.location.hash = 'frame/DistributionMap.aspx?cid=' + id;
		},
		render: function () {
			var model = this.getModel();
			var date = model.get('Date');
			var displayDate = date ? moment(date).format("MMM DD, YYYY") : '';
			return React.createElement(
				'div',
				{ className: 'row scroll-list-item', onDoubleClick: this.onGotoDMap.bind(null, model.get('Id')) },
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
							'button',
							{ onClick: this.onPublishToMonitors, className: 'button' },
							React.createElement('i', { className: 'fa fa-upload' }),
							React.createElement(
								'small',
								null,
								'Publish'
							)
						),
						React.createElement(
							'button',
							{ onClick: this.onDismiss, className: 'button has-tip top', title: 'dismiss',
								'data-tooltip': true, 'aria-haspopup': 'true',
								'data-disable-hover': 'false', tabIndex: '1' },
							React.createElement('i', { className: 'fa fa-reply' })
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
			this.subscribe('distribution/refresh', function () {
				self.getCollection().fetchForDistribution();
			});
			this.subscribe('search', function (words) {
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			$("#distribution-filter-ddl-ClientName, #distribution-filter-ddl-ClientCode, #distribution-filter-ddl-Date, #distribution-filter-ddl-AreaDescription").foundation();
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
			$('#distribution-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function (field, e) {
			e.preventDefault();
			e.stopPropagation();
			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
			$('#distribution-filter-ddl-' + field).foundation('close');
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
					{ 'data-toggle': "distribution-filter-ddl-" + field },
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
					{ id: "distribution-filter-ddl-" + field,
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
					{ 'data-toggle': "distribution-filter-ddl-" + field },
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
				    values = null;
				dataSource = _.filter(dataSource, function (i) {
					values = _.values(i.attributes);
					return _.some(values, function (i) {
						var dateCheck = moment(i, moment.ISO_8601);
						if (dateCheck.isValid()) {
							return dateCheck.format("MMM DD YYYY MMM DD, YYYY YYYY-MM-DD MM/DD/YYYY YYYY MM MMM DD").toLowerCase().indexOf(keyword) > -1;
						}
						return _.toString(i).toLowerCase().indexOf(keyword) > -1;
					});
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
									'Distribution Maps'
								)
							),
							React.createElement(
								'div',
								{ className: 'small-12 column' },
								React.createElement(
									'nav',
									{ 'aria-label': 'You are here:', role: 'navigation' },
									React.createElement(
										'ul',
										{ className: 'breadcrumbs' },
										React.createElement(
											'li',
											null,
											React.createElement(
												'a',
												{ href: '#' },
												'Control Center'
											)
										),
										React.createElement(
											'li',
											null,
											React.createElement(
												'span',
												{ className: 'show-for-sr' },
												'Current: '
											),
											' Distribution Maps'
										)
									)
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
							return React.createElement(DistributionRow, { key: item.get('Id'), model: item });
						})
					)
				)
			);
		}
	});
});
