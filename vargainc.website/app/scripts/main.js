import $ from 'jquery';
import Backbone from 'backbone';
import React from 'react';
import ReactDOM from 'react-dom';
import Topic from 'postal';
import {
	isFunction,
	startsWith
} from 'lodash';
import AppRouter from 'route';
import UserModel from 'models/user';
import LayoutView from 'views/layout/main';
import Promise from 'bluebird';

/**
 * register loading event except ajax request is quite
 */
$(document).ajaxSend(function (event, xhr, settings) {
	if (settings.quite !== true) {
		Topic.publish({
			channel: 'View',
			topic: 'showLoading'
		});

	}
});
$(document).ajaxComplete(function (event, xhr, settings) {
	if (settings.quite !== true) {
		Topic.publish({
			channel: 'View',
			topic: 'hideLoading'
		});
	}
});

/**
 * override base url
 */
var backboneSync = Backbone.sync;
Backbone.sync = function (method, model, options) {
	if (!options.url) {
		options.url = isFunction(model.url) ? model.url() : model.url;
	}
	if (!options.url) {
		options.url = model.urlRoot;
	}
	options.url = '../api/' + options.url;
	if (typeof RELEASE_VERSION !== 'undefined') {
		options.url = 'http://timm.vargainc.com/' + RELEASE_VERSION + '/api/' + options.url;
	}
	return backboneSync(method, model, options);
}

var appRouter = new AppRouter;
var userModel = new UserModel();
userModel.fetchCurrentUser().then(function (isSuccess) {
	if(!isSuccess && !startsWith(window.location.hash, '#driver')){
		window.location = '../login.html';
		return Promise.reject();
	}
	var LayoutViewInstance = React.createFactory(LayoutView);
	var layoutViewInstance = LayoutViewInstance({
		user: userModel
	});
	var layout = ReactDOM.render(layoutViewInstance, document.getElementById('main-container'));
	var appRouter = new AppRouter;
	appRouter.on('route', function () {
		// Topic.publish({
		// 	channel: 'View',
		// 	topic: 'showLoading'
		// });
	});
	Backbone.history.start();
	return Promise.resolve();
});