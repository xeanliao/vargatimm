define([
	'jquery',
	'underscore',
	'moment',
	'backbone',
	'react',
	'views/base',

	'react.backbone'
], function ($, _, moment, Backbone, React, BaseView) {
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
							AreaDescription:i.AreaDescription
						};
					});
					if(!_.isEmpty(data)){
						collection.add(data);
						self.setState({srcUrl: url});
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
			return (
				<div className="section row">
					<div className="small-12 columns">
						<div className="section-header">
							<div className="row" data-equalizer>
								<div className="small-12 column"><h5>Import Campaign</h5></div>
								<div className="small-8 column">
									<nav aria-label="You are here:" role="navigation">
										<ul className="breadcrumbs">
											<li><a href="#">Control Center</a></li>
											<li>
												<span className="show-for-sr">Current: </span> Import Campaign
											</li>
										</ul>
									</nav>
								</div>
								<div className="small-12 column">
									<div className="input-group">
										<input ref="sourceUrl" className="input-group-field" type="text" placeholder="Please input server address and query campaign from this server." />
										<div className="input-group-button">
											<input onClick={this.onSearch} type="button" className="button" value="Query" />
										</div>
									</div>
								</div>
							</div>
						</div>
						<div className="scroll-list-section-body">
							<div className="row scroll-list-header">
								<div className="small-2 columns">ClientName</div>
								<div className="small-4 columns">ClientCode</div>
								<div className="small-2 columns">Date</div>
								<div className="small-2 columns">AreaDescription</div>
								<div className="small-2 columns">Action</div>
							</div>
							{this.getCollection().map(function(item){
								return(
									<div key={item.get('Id')} className="row scroll-list-item">
										<div className="small-2 columns">{item.get('ClientName')}</div>
										<div className="small-4 columns">{item.get('ClientCode')}</div>
										<div className="small-2 columns">{moment(item.get('Date'), moment.ISO_8601).format("MMM DD, YYYY")}</div>
										<div className="small-2 columns">{item.get('AreaDescription')}</div>
										<div className="small-2 columns">
											<button class="button" onClick={self.onImport.bind(self, item.get('Id'))}>Import</button>
										</div>
									</div>
								);
							})}
				        </div>
				    </div>
		        </div>
			);
		}
	});
});