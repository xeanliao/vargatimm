import Backbone from 'backbone';
import React from 'react';
import Topic from 'postal';
import Promise from 'bluebird';
import {
	assign,
	unset,
	forEach,
	isString,
	isFunction,
	isObject
} from 'lodash';
import 'jquery';
import 'foundation-sites';

export default {
	getDefaultProps: function () {
		return {
			registeredTopic: {}
		}
	},
	subscribe: function (opt) {
		var params;
		if (arguments.length == 2 && isString(arguments[0]) && isFunction(arguments[1])) {
			params = {
				channel: 'View',
				topic: arguments[0],
				callback: arguments[1]
			}
		} else {
			params = assign({
				channel: 'View',
				topic: 'undefined'
			}, opt);
		}

		var name = params.channel + '.*/-+-\*.' + params.topic;
		this.props.registeredTopic[name] && this.props.registeredTopic[name].unsubscribe();
		var signal = Topic.subscribe(params);
		this.props.registeredTopic[name] = signal;
	},
	unsubscribe: function (topic) {
		var name = 'View' + '.*/-+-\*.' + topic;
		if (this.props.registeredTopic[name]) {
			this.props.registeredTopic[name].unsubscribe();
			unset(this.props.registeredTopic, name);
		}
	},
	publish: function () {
		var opt;
		if (arguments.length == 1 && isString(arguments[0])) {
			opt = {
				channel: 'View',
				topic: arguments[0],
				data: null
			}
		} else if (arguments.length == 4 && isString(arguments[0]) && isFunction(arguments[1]) && arguments[2] instanceof Backbone.Model && isObject(arguments[3])) {
			opt = {
				channel: 'View',
				topic: arguments[0],
				data: {
					view: arguments[1],
					params: {
						model: arguments[2]
					},
					options: arguments[3]
				}
			}
		} else if (arguments.length == 4 && isString(arguments[0]) && isFunction(arguments[1]) && arguments[2] instanceof Backbone.Collection && isObject(arguments[3])) {
			opt = {
				channel: 'View',
				topic: arguments[0],
				data: {
					view: arguments[1],
					params: {
						collection: arguments[2]
					},
					options: arguments[3]
				}
			}
		} else if (arguments.length == 3 && isString(arguments[0]) && isFunction(arguments[1]) && arguments[2] instanceof Backbone.Model) {
			opt = {
				channel: 'View',
				topic: arguments[0],
				data: {
					view: arguments[1],
					params: {
						model: arguments[2]
					}
				}
			}
		} else if (arguments.length == 3 && isString(arguments[0]) && isFunction(arguments[1]) && arguments[2] instanceof Backbone.Collection) {
			opt = {
				channel: 'View',
				topic: arguments[0],
				data: {
					view: arguments[1],
					params: {
						collection: arguments[2]
					}
				}
			}
		} else if (arguments.length == 2 && isString(arguments[0]) && isFunction(arguments[1])) {
			opt = {
				channel: 'View',
				topic: arguments[0],
				data: {
					view: arguments[1]
				}
			}
		} else if (arguments.length > 1 && isString(arguments[0])) {
			opt = {
				channel: 'View',
				topic: arguments[0],
				data: arguments[1]
			}
		} else {
			opt = arguments[0];
		}
		Topic.publish(assign({
			channel: 'View',
			topic: 'undefined'
		}, opt));
	},
	componentWillUnmount: function () {
		forEach(this.props.registeredTopic, function (i) {
			i.unsubscribe();
		});
	},
	scrollTop: function (ele) {
		$('.off-canvas-wrapper-inner').stop().animate({
			scrollTop: $(ele).offset().top
		}, 600);
	},
	confirm: function (content) {
		var self = this;
		return new Promise((resolve, reject) => {
			var cancel = function () {
				self.publish('showDialog');
				reject(new Error('user cancel'));
			};
			var okay = function () {
				self.publish('showDialog');
				resolve();
			}
			if (isString(content)) {
				content = content.replace('\r', '').split('\n');
				content = content.map(row => {
					return (<p>{row}</p>);
				});
			}
			var view = (
				<div className="row">
					<div className="small-12 columns">
						<p>&nbsp;</p>
						<h5>{content}</h5>
						<p>&nbsp;</p>
					</div>
					<div className="small-12 columns">
						<div className="button-group float-right">
							<a href="javascript:;" className="button success tiny" onClick={okay}>Okay</a>
						</div>
						<div className="button-group float-right">
							<a href="javascript:;" className="button tiny" onClick={cancel}>Cancel</a>
						</div>
					</div>
					<button onClick={cancel} className="close-button" data-close aria-label="Close reveal" type="button">
				    	<span aria-hidden="true">&times;</span>
				  	</button>
				</div>
			);
			self.publish('showDialog', {
				view: view
			});
		});
	},
	alert: function (content) {
		var self = this;
		if (isString(content)) {
			content = content.replace('\r', '').split('\n');
			content = content.map(row => {
				return (<p>{row}</p>);
			});
		}
		var closeDialog = function () {
			self.publish('showDialog');
		};
		var view = (
			<div className="row">
					<div className="small-12 columns">
						<p>&nbsp;</p>
						<h5>{content}</h5>
						<p>&nbsp;</p>
					</div>
					<div className="small-12 columns">
						<div className="button-group float-right">
							<a href="javascript:;" className="button tiny" onClick={closeDialog}>Okay</a>
						</div>
					</div>
					<button onClick={closeDialog} className="close-button" data-close aria-label="Close reveal" type="button">
				    	<span aria-hidden="true">&times;</span>
				  	</button>
				</div>
		);
		this.publish('showDialog', {
			view: view
		});
	}
};