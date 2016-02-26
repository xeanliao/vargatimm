define([
	'moment',
	'backbone',
	'react',
	'views/base',
	'views/print/shared/footer',
	'react.backbone'
], function (moment, Backbone, React, BaseView, FooterView) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getExportParamters: function () {
			var model = this.getModel();
			return {
				'type': 'cover',
				'options': [{
					'title': 'This Custom Campaign is Presented to:'
				}, {
					'key': '',
					'image': model.get('Logo') || '',
					'height': '80',
					'align': 'center',
					'top': '40',
					'bottom': '40'
				}, {
					'key': 'Client Name:',
					'text': model.get('ClientName')
				}, {
					'key': 'Created for:',
					'text': model.get('ContactName')
				}, {
					'key': 'Created on:',
					'text': model.get('Date') ? moment(model.get('Date')).format("MMM DD, YYYY") : ''
				}, {
					'key': ' ',
					'text': ' '
				}, {
					'key': 'Presented by:',
					'image': 'images/vargainc-logo.png',
					'height': '40',
					'align': 'center',
					'top': '10',
					'bottom': '10'
				}, {
					'key': 'Master Campaign #:',
					'text': model.get('DisplayName')
				}, {
					'key': 'Created by:',
					'text': model.get('CreatorName')
				}]

			};
		},
		render: function () {
			var model = this.getModel(),
				displayDate = model.get('Date') ? moment(model.get('Date')).format("MMM DD, YYYY") : '';
			if(model.get('Logo')){
				var clientLog = <img className="client-logo" src={model.get('Logo')} />;
			}else{
				var clientLog = <div className="client-logo"></div>
			}

			return (
				<div className="page cover">
					<div className="row">
		  				<div className="small-12 columns text-center title">
		  					<span className="editable" role="title">This Custom Campaign is Presented to:</span>
						</div>
		  			</div>
					<div className="row">
						<div className="small-12 columns text-center">
							{clientLog}
						</div>
					</div>
					<div className="row">
						<div className="small-12 columns text-center">Client Name:</div>
						<div className="small-12 columns text-center">{model.get('ClientName')}</div>
					</div>
					<div className="row">
						<div className="small-12 columns text-center">Created for:</div>
						<div className="small-12 columns text-center">{model.get('ContactName')}</div>
					</div>
					<div className="row">
						<div className="small-12 columns text-center">Created on:</div>
						<div className="small-12 columns text-center">{displayDate}</div>
					</div>
					<div className="row">
						<div className="small-12 columns text-center">&nbsp;</div>
						<div className="small-12 columns text-center">&nbsp;</div>
					</div>
					<div className="row">
						<div className="small-12 columns text-center">Presented by:</div>
						<div className="small-12 columns text-center">
							<div className="vargainc-log"></div>
						</div>
					</div>
					<div className="row">
						<div className="small-12 columns text-center">Master Campaign #:</div>
						<div className="small-12 columns text-center">{model.get('DisplayName')}</div>
					</div>
					<div className="row">
						<div className="small-12 columns text-center">Created by:</div>
						<div className="small-12 columns text-center">{model.get('CreatorName')}</div>
					</div>
					<FooterView model={model.get('Footer')} />
				</div>
			);
		}
	});
});