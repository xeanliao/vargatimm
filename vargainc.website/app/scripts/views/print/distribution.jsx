define([
	'underscore',
	'moment',
	'backbone',
	'react',
	'views/base',
	'views/print/shared/distributionMap',
	'react.backbone'
], function (_, moment, Backbone, React, BaseView, DMap) {
	return React.createBackboneClass({
		mixins: [BaseView],
		componentWillMount: function () {
		},
		render: function () {
			return (
				<div className="section row">
					<div className="small-12 columns">
						<div className="section-header">
							<div className="row">
								<div className="small-12 column"><h5>TIMM Print Preview</h5></div>
							</div>
						</div>
						
					</div>
				</div>
			);
		}
	});
});