import Backbone from 'backbone';
import React from 'react';
import 'react.backbone';
import $ from 'jquery';

import BaseView from 'views/base';

export default React.createBackboneClass({
	mixins: [BaseView],
	getDefaultProps: function () {
		return {
			colors: [20, 40, 60, 80]
		};
	},
	componentWillUnmount: function () {
		$(this.refs.colorSlider).slider("destroy");
	},
	componentDidMount: function () {
		var self = this;
		$(this.refs.colorSlider).slider({
			step: 1,
			min: 0,
			max: 100,
			values: this.props.colors,
			slide: function (event, ui) {
				var colorGroup = ['Blue', 'Green', 'Yellow', 'Orange', 'Red'],
					colors = [0].concat(ui.values),
					result = [],
					min = 0;
				colors.push(100);
				for (var i = 0; i < colors.length - 1; i++) {
					if (min >= colors[i + 1]) {
						continue;
					}
					min = colors[i + 1];
					result.push(colorGroup[i] + ' (' + colors[i] + '% - ' + colors[i + 1] + '%) ');
				}
				$(this).parents('label').find('span').eq(0).text(result.join('  '));
				self.props.onChange && self.props.onChange(ui.values);
			}
		});

	},
	onReset: function () {
		$(this.refs.colorSlider).slider('values', [20, 40, 60, 80]);
		self.props.onChange && self.props.onChange([20, 40, 60, 80]);
	},
	render: function () {
		return (
			<div className="column">
            	<div className="row">
				    <div className="custom-colors small-10 columns">
				        <label>
				            <span>Blue (0% - 20%) Green (20% - 40%) Yellow (40% - 60%) Orange (60% - 80%) Red (80% - 100%) </span>
				            <div className="slider" ref="colorSlider"></div>
				        </label>
				    </div>
				    <div className="small-2 columns">
				        <button className="button tiny alert" onClick={this.onReset}>Reset</button>
				    </div>
				</div>
            </div>
		);
	}
});