define(['jquery', 'underscore', 'moment', 'backbone', 'react', 'views/base', 'models/print/options', 'views/print/shared/campaignOptions', 'views/print/shared/cover', 'views/print/shared/campaign', 'views/print/shared/campaignSummary', 'views/print/shared/submap', 'views/print/shared/submapDetail', 'react.backbone'], function ($, _, moment, Backbone, React, BaseView, OptionsModel, OptionsView, CoverView, CampaignView, CampaignSummaryView, SubMapView, SubMapDetailView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function () {
			var options = new OptionsModel({
				suppressCover: false,
				suppressCampaign: false,
				suppressCampaignSummary: false,
				suppressNDAInCampaign: false,
				suppressSubMap: false,
				suppressSubMapCountDetail: false,
				suppressNDAInSubMap: false,
				suppressDMap: false,
				suppressGTU: false,
				suppressNDAInDMap: true,
				showPenetrationColors: true,
				penetrationColors: [20, 40, 60, 80],
				suppressLocations: false,
				suppressRadii: false
			});
			return { options: options };
		},
		componentDidMount: function () {
			var collecton = this.getCollection(),
			    self = this;
			this.subscribe('print.map.imageloaded', function () {
				_.some(collecton.models, function (page) {
					var currentPage = self.refs[page.get('key')];
					currentPage && currentPage.state && !currentPage.state.imageLoaded && currentPage.state.imageLoading === false && currentPage.loadImage && currentPage.loadImage();
					return currentPage && currentPage.loadImage ? !currentPage.state.imageLoaded : false;
				});
			});
			this.subscribe('print.map.options.changed', this.onApplyOptions);
			this.onOpenOptions({ needTrigger: true });
		},
		onOpenOptions: function (opts) {
			var options = this.state.options;
			if (!options.get('DMaps')) {
				options.attributes['DMaps'] = this.getCollection().getDMaps();
			}
			var model = _.cloneDeep(options);
			var params = _.extend(opts, { model: model });
			this.publish('showDialog', OptionsView, params, { size: 'large' });
		},
		onApplyOptions: function (options) {
			//check need reload images
			var compareProperty = ['suppressNDAInCampaign', 'suppressNDAInSubMap', 'suppressNDAInDMap', 'showPenetrationColors', 'penetrationColors', 'suppressLocations', 'suppressRadii'],
			    oldOptions = _.pick(this.state.options.attributes, compareProperty),
			    newOptions = _.pick(options.attributes, compareProperty);
			if (!_.eq(oldOptions, newOptions)) {
				_.forEach(this.refs, function (page) {
					page.setState({
						imageLoaded: null,
						imageLoading: false
					});
				});
			}
			this.setState({
				options: options
			});
			this.publish("showDialog");
			this.publish('print.map.imageloaded');
		},
		onPrint: function () {
			var collecton = this.getCollection(),
			    campaignId = collecton.getCampaignId(),
			    postData = {
				campaignId: campaignId,
				size: 'A4',
				needFooter: 'true',
				options: []
			},
			    self = this;
			_.forEach(collecton.models, function (page) {
				if (self.refs[page.get('key')]) {
					postData.options.push(self.refs[page.get('key')].getExportParamters());
				}
			});

			collecton.exportPdf(postData).then(function (response) {
				var downloadUrl = '../api/pdf/download/' + campaignId + '/' + response.sourceFile;
				if ($('#downloadForm').size() == 0) {
					$('<form id="downloadForm" action="' + downloadUrl + '" method="GET"></form>').appendTo('body').get(0).submit();
				} else {
					$('#downloadForm').attr('action', downloadUrl).get(0).submit();
				}
			});
		},
		render: function () {
			var pages = this.getCollection(),
			    printOptions = this.state.options,
			    options = _.omit(printOptions.attributes, ['DMaps']),
			    dmaps = printOptions.get('DMaps') || [],
			    hideDMaps = _.map(dmaps.models, function (item) {
				if (item.get('Selected') === true) {
					return item.get('Id');
				}
				return null;
			});
			return React.createElement(
				'div',
				{ className: 'section' },
				React.createElement(
					'div',
					{ className: 'row' },
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						React.createElement(
							'div',
							{ className: 'button-group print-toolbar' },
							React.createElement(
								'button',
								{ onClick: this.onOpenOptions },
								React.createElement('i', { className: 'fa fa-cog' }),
								'Options'
							),
							React.createElement(
								'button',
								{ onClick: this.onPrint },
								React.createElement('i', { className: 'fa fa-print' }),
								'Print'
							)
						)
					)
				),
				React.createElement(
					'div',
					{ className: 'page-container A4' },
					pages.map(function (page) {
						var dmapId = page.get('DMapId');
						if (dmapId && _.indexOf(hideDMaps, dmapId) > -1) {
							return null;
						}
						switch (page.get('type')) {
							case 'Cover':
								return options.suppressCover ? null : React.createElement(CoverView, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							case 'Campaign':
								return options.suppressCampaign ? null : React.createElement(CampaignView, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							case 'CampaignSummary':
								return options.suppressCampaign || options.suppressCampaignSummary ? null : React.createElement(CampaignSummaryView, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							case 'SubMap':
								return options.suppressSubMap ? null : React.createElement(SubMapView, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
								break;
							case 'SubMapDetail':
								return options.suppressSubMap || options.suppressSubMapCountDetail ? null : React.createElement(SubMapDetailView, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
								break;
							default:
								return null;
								break;
						}
					})
				)
			);
		}
	});
});
