define(['jquery', 'underscore', 'moment', 'backbone', 'react', 'views/base', 'react.backbone'], function ($, _, moment, Backbone, React, BaseView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		onSearch: function () {
			var url = this.refs.sourceUrl.value;
			if (_.isEmpty(url)) {
				this.publish('showDialog', 'Please input source url');
				return;
			}
			var self = this,
			    collection = this.getCollection();
			collection.reset();
			$.ajax({
				url: url,
				method: 'GET',
				dataType: "json",
				cache: false,
				success: function (result) {
					var data = _.map(result, function (i) {
						return {
							Id: i.Id,
							ClientName: i.ClientName,
							ClientCode: i.ClientCode,
							Date: i.Date,
							AreaDescription: i.AreaDescription
						};
					});
					if (!_.isEmpty(data)) {
						collection.add(data);
						self.setState({ srcUrl: url });
					}
				}
			});
		},
		onImportFailed: function () {
			this.publish('showDialog', 'copy campaign failed. please contact us!');
		},
		onImport: function (campaignId) {
			console.log(campaignId);
			var exportUrl = _.trimEnd(this.state.srcUrl, '/') + '/' + campaignId + '/export',
			    importUrl = '../api/campaign/import',
			    self = this;
			$.getJSON(exportUrl).then(function (campaign) {
				return $.post(importUrl, campaign, function (response) {
					if (response && response.success) {
						self.publish('showDialog', 'copy success. please refresh control center!');
					} else {
						self.onImportFailed();
					}
				});
			}, self.onImportFailed);
		},
		render: function () {
			var self = this;
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
								{ className: 'small-12 column' },
								React.createElement(
									'h5',
									null,
									'Import Campaign'
								)
							),
							React.createElement(
								'div',
								{ className: 'small-8 column' },
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
											' Import Campaign'
										)
									)
								)
							),
							React.createElement(
								'div',
								{ className: 'small-12 column' },
								React.createElement(
									'div',
									{ className: 'input-group' },
									React.createElement('input', { ref: 'sourceUrl', className: 'input-group-field', type: 'text', placeholder: 'Please input server address and query campaign from this server.' }),
									React.createElement(
										'div',
										{ className: 'input-group-button' },
										React.createElement('input', { onClick: this.onSearch, type: 'button', className: 'button', value: 'Query' })
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
								{ className: 'small-2 columns' },
								'ClientName'
							),
							React.createElement(
								'div',
								{ className: 'small-4 columns' },
								'ClientCode'
							),
							React.createElement(
								'div',
								{ className: 'small-2 columns' },
								'Date'
							),
							React.createElement(
								'div',
								{ className: 'small-2 columns' },
								'AreaDescription'
							),
							React.createElement(
								'div',
								{ className: 'small-2 columns' },
								'Action'
							)
						),
						this.getCollection().map(function (item) {
							return React.createElement(
								'div',
								{ key: item.get('Id'), className: 'row scroll-list-item' },
								React.createElement(
									'div',
									{ className: 'small-2 columns' },
									item.get('ClientName')
								),
								React.createElement(
									'div',
									{ className: 'small-4 columns' },
									item.get('ClientCode')
								),
								React.createElement(
									'div',
									{ className: 'small-2 columns' },
									moment(item.get('Date'), moment.ISO_8601).format("MMM DD, YYYY")
								),
								React.createElement(
									'div',
									{ className: 'small-2 columns' },
									item.get('AreaDescription')
								),
								React.createElement(
									'div',
									{ className: 'small-2 columns' },
									React.createElement(
										'button',
										{ 'class': 'button', onClick: self.onImport.bind(self, item.get('Id')) },
										'Import'
									)
								)
							);
						})
					)
				)
			);
		}
	});
});
