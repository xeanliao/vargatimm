import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import $ from 'jquery';
import {
	some,
	cloneDeep,
	extend,
	pick,
	eq,
	each,
	omit,
	map,
	indexOf,
} from 'lodash';

import OptionsModel from 'models/print/options';

import BaseView from 'views/base';
import OptionsView from 'views/print/shared/distributionOptions';
import DistributionMapView from 'views/print/shared/distributionMap';
import DistributionDetailMapView from 'views/print/shared/distributionDetailMap';

export default React.createBackboneClass({
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
			suppressLocations: true,
			suppressRadii: true
		});
		return {
			options: options
		};
	},
	componentDidMount: function () {
		var collecton = this.getCollection(),
			self = this;
		this.subscribe('print.map.imageloaded', function () {
			some(collecton.models, function (page) {
				var currentPage = self.refs[page.get('key')];
				currentPage && currentPage.state && !currentPage.state.imageLoaded && currentPage.state.imageLoading == false && currentPage.loadImage();
				return currentPage && currentPage.loadImage ? !currentPage.state.imageLoaded : false;
			});
		});
		this.subscribe('print.map.options.changed', this.onApplyOptions);
		this.onOpenOptions({
			needTrigger: true
		});
	},
	onOpenOptions: function (opts) {
		var options = this.state.options;
		if (!options.get('DMaps')) {
			options.attributes['DMaps'] = this.getCollection().getDMaps();
		}
		var model = cloneDeep(options);
		var params = extend(opts, {
			model: model
		});
		this.publish('showDialog', OptionsView, params, {
			size: 'large'
		});
	},
	onApplyOptions: function (options) {
		//check need reload images
		var compareProperty = ['suppressDMap', 'suppressNDAInDMap'],
			oldOptions = pick(this.state.options.attributes, compareProperty),
			newOptions = pick(options.attributes, compareProperty);
		if (!eq(oldOptions, newOptions)) {
			forEach(this.refs, function (page) {
				page.setState({
					imageLoaded: null
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
				size: 'Distribute',
				needFooter: 'false',
				options: []
			},
			self = this;
		forEach(collecton.models, function (page) {
			if (self.refs[page.get('key')]) {
				postData.options.push(self.refs[page.get('key')].getExportParamters());
			}
		})

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
			options = omit(printOptions.attributes, ['DMaps']),
			dmaps = printOptions.get('DMaps') || [],
			hideDMaps = map(dmaps.models, function (item) {
				if (item.get('Selected') === true) {
					return item.get('Id');
				}
				return null;
			});
		return (
			<div className="section">
				<div className="row">
					<div className="small-12 columns text-center">
						<div className="button-group print-toolbar">
							<button onClick={this.onOpenOptions}><i className="fa fa-cog"></i>Options</button>
							<button onClick={this.onPrint}><i className="fa fa-print"></i>Print</button>
						</div>
					</div>
				</div>
				<div className="page-container distribution-print">
					{pages.map(function(page){
						var dmapId = page.get('DMapId');
						if (dmapId && indexOf(hideDMaps, dmapId) > -1) {
							return null;
						}
						switch (page.get('type')) {
							case 'DistributionDetail':
								return <DistributionDetailMapView ref={page.get('key')} key={page.get('key')} model={page} options={options} />;
							default :
								return <DistributionMapView ref={page.get('key')} key={page.get('key')} model={page} options={options} />;
						}
					})}
				</div>
			</div>
		);
	}
});