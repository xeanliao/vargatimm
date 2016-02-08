define(['underscore', 'moment', 'backbone', 'react', 'views/base', 'models/task', 'react.backbone'], function (_, moment, Backbone, React, BaseView, TaskModel) {
	return React.createBackboneClass({
		mixins: [BaseView],
		getInitialState: function () {
			return {
				date: ''
			};
		},
		componentWillMount: function () {
			var self = this;
			this.subscribe("print/debug", function () {
				self.setState({
					date: new Date().toString()
				});
			});
		},
		updateDate: function () {
			this.publish('print/debug');
		},
		render: function () {
			return React.createElement(
				'div',
				null,
				React.createElement(
					'h1',
					{ className: 'myname' },
					this.state.date
				),
				React.createElement(
					'button',
					{ onClick: this.updateDate },
					'Click Me'
				)
			);
		}
	});
});
