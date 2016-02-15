define(['jquery', 'underscore', 'moment', 'backbone', 'react', 'views/base', 'models/print/options', 'views/print/shared/distributionOptions', 'views/print/shared/distributionMap', 'views/print/shared/distributionDetailMap', 'react.backbone'], function ($, _, moment, Backbone, React, BaseView, OptionsModel, OptionsView, DistributionMapView, DistributionDetailMapView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function () {
			var options = new OptionsModel({
				suppressCover: true,
				suppressCampaign: true,
				suppressCampaignSummary: true,
				suppressNDAInCampaign: true,
				suppressSubMap: true,
				suppressSubMapCountDetail: true,
				suppressNDAInSubMap: true,
				suppressDMap: false,
				suppressGTU: true,
				suppressNDAInDMap: true,
				customSubMapPenetrationColors: false,
				suppressLocations: true,
				suppressRadii: true,
				mapType: 'ROADMAP'
			});
			return { options: options };
		},
		componentDidMount: function () {
			var self = this;
			this.subscribe('print.map.imageloaded', function () {
				_.some(self.refs, function (page) {
					page.state.ImageLoaded == false && page.loadImage();
					return !page.state.ImageLoaded;
				});
			});
			this.subscribe('print.map.options.changed', this.onApplyOptions);
			this.onOpenOptions({ needTrigger: true });
		},
		onOpenOptions: function (opts) {
			var options = this.state.options;
			if (!options.get('DMaps')) {
				console.log('dmaps');
				options.attributes['DMaps'] = this.getCollection().getDMaps();
			}
			var model = _.cloneDeep(options);
			var view = React.createFactory(OptionsView);
			var viewInstance = new view(_.extend(opts, { model: model }));
			this.publish('showDialog', viewInstance, null, null, 'large');
		},
		onApplyOptions: function (options) {
			//check need reload images
			var compareProperty = ['suppressDMap', 'suppressNDAInDMap'],
			    oldOptions = _.pick(this.state.options.attributes, compareProperty),
			    newOptions = _.pick(options.attributes, compareProperty);
			if (!_.eq(oldOptions, newOptions)) {
				_.forEach(this.refs, function (page) {
					page.setState({ ImageLoaded: false });
				});
			}
			this.setState({ options: options });
			this.publish("showDialog", null);
			this.publish('print.map.imageloaded');
		},
		onPrint: function () {
			var collecton = this.getCollection(),
			    campaignId = collecton.getCampaignId(),
			    postData = {
				campaignId: campaignId,
				size: "Distribute",
				needFooter: false,
				options: _.map(this.refs, function (page) {
					console.log(page, page.getExportParamters);
					return page.getExportParamters();
				})
			};
			console.log(postData);
			collecton.exportPdf(postData).then(function (response) {
				var downloadUrl = '../api/pdf/download/' + campaignId + '/' + response.sourceFile;
				$('<form action="' + downloadUrl + '" method="GET"></form>').appendTo('body').get(0).submit();
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
										'TIMM Print Preview'
									)
								)
							)
						)
					),
					React.createElement(
						'div',
						{ className: 'small-12 columns text-center' },
						React.createElement(
							'div',
							{ className: 'button-group' },
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
					{ className: 'page-container dmapPrint' },
					pages.map(function (page) {
						var dmapId = page.get('DMapId');
						if (dmapId && _.indexOf(hideDMaps, dmapId) > -1) {
							return null;
						}
						switch (page.get('type')) {
							case 'DistributionDetail':
								return React.createElement(DistributionDetailMapView, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
							default:
								return React.createElement(DistributionMapView, { ref: page.get('key'), key: page.get('key'), model: page, options: options });
						}
					})
				)
			);
		}
	});
});
