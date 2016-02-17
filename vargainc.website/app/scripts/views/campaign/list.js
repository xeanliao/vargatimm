define(['underscore', 'moment', 'backbone', 'react', 'views/base', 'views/campaign/edit', 'views/campaign/publish', 'models/campaign', 'react.backbone'], function (_, moment, Backbone, React, BaseView, EditView, PublishView, Model) {
	var actionHandle = null,
	    searchHandle = null;
	var CampaignRow = React.createBackboneClass({
		mixins: [BaseView],
		menuKey: 'campaign-menu-ddl-',
		getDefaultProps: function () {
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		componentDidMount: function () {
			$("#" + this.menuKey + this.getModel().get('Id')).foundation();
		},
		componentDidUpdate: function () {},
		onCopy: function (e) {
			e.preventDefault();
			e.stopPropagation();
			$(e.currentTarget).closest('.dropdown-pane').foundation('close');
			var model = this.getModel(),
			    self = this;
			model.copy({
				success: function (response) {
					if (response && response.success) {
						var copiedModel = new Model(response.data);
						model.collection.add(copiedModel, { at: 0 });
					}
				},
				error: function () {}
			});
		},
		onEdit: function (e) {
			e.preventDefault();
			e.stopPropagation();
			$(e.currentTarget).closest('.dropdown-pane').foundation('close');
			this.publish('showDialog', EditView, {
				model: this.getModel().clone()
			});
		},
		onDelete: function (e) {
			e.preventDefault();
			e.stopPropagation();
			$(e.currentTarget).closest('.dropdown-pane').foundation('close');
			var self = this;
			var result = confirm('are you sure want delete this campaign?');
			if (result) {
				var model = self.getModel();
				model.destroy({
					wait: true,
					success: function () {
						self.publish('camapign/refresh');
					}
				});
			}
		},
		onPublishToDMap: function (e) {
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel();
			this.publish('showDialog', PublishView);
			actionHandle && this.unsubscribe(actionHandle);
			actionHandle = this.subscribe('campaign/publish', function (user) {
				model.publishToDMap(user, {
					success: function (result) {
						this.publish('showDialog');
						if (result && result.success) {
							this.publish('camapign/refresh');
						} else {
							alert(result.error);
						}
					}
				});
			});
		},
		onOpenMoreMenu: function (e) {
			e.preventDefault();
			e.stopPropagation();
		},
		onCloseMoreMenu: function (key, e) {
			e.preventDefault();
			e.stopPropagation();
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
								{ href: 'javascript:;', onClick: this.onCopy },
								React.createElement('i', { className: 'fa fa-copy' }),
								React.createElement(
									'span',
									null,
									'Copy'
								)
							)
						),
						React.createElement(
							'li',
							null,
							React.createElement(
								'a',
								{ href: 'javascript:;', onClick: this.onDelete },
								React.createElement('i', { className: 'fa fa-trash' }),
								React.createElement(
									'span',
									null,
									'Delete'
								)
							)
						)
					)
				)
			);
		},
		onGotoCMap: function (id) {
			window.location.hash = 'frame/campaign.aspx?cid=' + id;
		},
		render: function () {
			var model = this.getModel();
			var date = model.get('Date');
			var displayDate = date ? moment(date).format("MMM DD, YYYY") : '';
			return React.createElement(
				'div',
				{ className: 'row scroll-list-item', onDoubleClick: this.onGotoCMap.bind(null, model.get('Id')) },
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
							{ onClick: this.onPublishToDMap, className: 'button' },
							React.createElement('i', { className: 'fa fa-upload' }),
							React.createElement(
								'small',
								null,
								'Publish'
							)
						),
						this.renderMoreMenu(model.get('Id'))
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
			this.subscribe('camapign/refresh', function () {
				self.getCollection().fetch();
			});
			searchHandle && this.unsubscribe(searchHandle);
			searchHandle = this.subscribe('search', function (words) {
				console.log('on search');
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			$("#campaign-filter-ddl-ClientName, #campaign-filter-ddl-ClientCode, #campaign-filter-ddl-Date, #campaign-filter-ddl-AreaDescription").foundation();
		},
		componentWillUnmount: function () {
			searchHandle && this.unsubscribe(searchHandle);
			actionHandle && this.unsubscribe(actionHandle);
		},
		onNew: function () {
			this.publish('showDialog', EditView, {
				model: new Model()
			});
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
			$('#campaign-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function (field, e) {
			e.preventDefault();
			e.stopPropagation();
			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
			$('#campaign-filter-ddl-' + field).foundation('close');
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
					{ 'data-toggle': "campaign-filter-ddl-" + field },
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
					{ id: "campaign-filter-ddl-" + field,
						className: 'dropdown-pane bottom',
						style: { 'width': 'auto' },
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
					{ 'data-toggle': "campaign-filter-ddl-" + field },
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
							{ className: 'row', 'data-equalizer': true },
							React.createElement(
								'div',
								{ className: 'small-4 column' },
								React.createElement(
									'h5',
									null,
									'Campaign'
								)
							),
							React.createElement(
								'div',
								{ className: 'small-8 column' },
								React.createElement(
									'button',
									{ onClick: this.onNew, className: 'float-right section-button' },
									React.createElement('i', { className: 'fa fa-plus' }),
									React.createElement(
										'span',
										null,
										'New Campaign'
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
							return React.createElement(CampaignRow, { key: item.get('Id'), model: item });
						})
					)
				)
			);
		}
	});
});
