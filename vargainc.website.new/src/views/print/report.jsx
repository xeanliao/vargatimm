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
import OptionsView from 'views/print/shared/reportOptions';
import CoverView from 'views/print/shared/cover';
import CampaignView from 'views/print/shared/campaign';
import CampaignSummaryView from 'views/print/shared/campaignSummary';
import SubMapView from 'views/print/shared/submap';
import SubMapDetailView from 'views/print/shared/submapDetail';
import DMapView from 'views/print/shared/dmap';
import DMapDetailMapView from 'views/print/shared/dmapDetailMap';

export default React.createBackboneClass({
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
				if(currentPage && currentPage.state && !currentPage.state.imageLoaded && currentPage.state.imageLoading === false && currentPage.loadImage){
					currentPage.loadImage();	
					return true;
				}
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
		this.publish('showDialog', {
			view: OptionsView,
			params: params,
			options: {
				size: 'large'
			}
		});
	},
	onApplyOptions: function (options) {
		var self = this;
		//check need reload images
		var compareProperty = [
				'suppressNDAInCampaign',
				'suppressNDAInSubMap',
				'suppressNDAInDMap',
				'showPenetrationColors',
				'penetrationColors',
				'suppressLocations',
				'suppressRadii'
			],
			oldOptions = pick(this.state.options.attributes, compareProperty),
			newOptions = pick(options.attributes, compareProperty);
		if (!eq(oldOptions, newOptions)) {
			each(this.refs, function (page) {
				page.setState({
					imageLoaded: null
				});
			});
		}
		this.setState({
			options: options
		}, ()=>{
			self.publish("showDialog");
			self.publish('print.map.imageloaded');	
		});
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
		each(collecton.models, function (page) {
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
		var self = this,
			pages = this.getCollection(),
			printOptions = this.state.options,
			options = omit(printOptions.attributes, ['DMaps']),
			dmaps = printOptions.get('DMaps') || [];
		var hideDMaps = map(dmaps.models, function (item) {
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
							<button className="button" onClick={this.onOpenOptions}><i className="fa fa-cog"></i>Options</button>
							<button className="button" onClick={this.onPrint}><i className="fa fa-print"></i>Print</button>
						</div>
					</div>
				</div>
				<div className="page-container A4">
					{pages.map(function(page){
						switch (page.get('type')) {
							case 'Cover':
								return options.suppressCover ? null : <CoverView ref={page.get('key')} key={page.get('key')} model={page} options={options} />;
							case 'Campaign':
								return options.suppressCampaign ? null : <CampaignView ref={page.get('key')} key={page.get('key')} model={page} options={options} />;
							case 'CampaignSummary':
								return options.suppressCampaign || options.suppressCampaignSummary ? null : <CampaignSummaryView ref={page.get('key')} key={page.get('key')} model={page} options={options} />;
							case 'SubMap':
								return options.suppressSubMap ? null : <SubMapView ref={page.get('key')} key={page.get('key')} model={page} options={options} />;
								break;
							case 'SubMapDetail':
								return options.suppressSubMap || options.suppressSubMapCountDetail ? null : <SubMapDetailView ref={page.get('key')} key={page.get('key')} model={page} options={options} />;
								break;
							case 'DMap':
								return options.suppressDMap || indexOf(hideDMaps, page.get('DMapId')) > -1 ? null : <DMapView ref={page.get('key')} key={page.get('key')} model={page} options={options} />;
								break;
							case 'DMapDetailMap':
								return options.suppressDMap || indexOf(hideDMaps, page.get('DMapId')) > -1 ? null : <DMapDetailMapView ref={page.get('key')} key={page.get('key')} model={page} options={options} />;
								break;
							default:
								return null;
								break;
						}
					})}
				</div>
			</div>
		);
	}
});