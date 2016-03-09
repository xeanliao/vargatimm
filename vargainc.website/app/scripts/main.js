define(function (require, exports, module) {
	var $ = require('jquery'),
		Backbone = require('backbone'),
		React = require('react'),
		ReactDOM = require('react-dom'),
		Topic = require('pubsub');

	/**
	 * register loading event except ajax request is quite
	 */
	$(document).ajaxSend(function (event, xhr, settings) {
		if (settings.quite !== true) {
			Topic.publish('showLoading');
		}
	});
	$(document).ajaxComplete(function (event, xhr, settings) {
		if (settings.quite !== true) {
			Topic.publish('hideLoading');
		}
	});

	/**
	 * override base url
	 */
	var backboneSync = Backbone.sync;
	Backbone.sync = function (method, model, options) {
		if (!options.url) {
			options.url = _.isFunction(model.url) ? model.url() : model.url;
		}
		if (!options.url) {
			options.url = model.urlRoot;
		}
		options.url = '../api/' + options.url;

		return backboneSync(method, model, options);
	}

	var AppRouter = require('route');
	var appRouter = new AppRouter;
		var UserModel = require('models/user');
	var LayoutView = require('views/layout/main');
	var userModel = new UserModel();
	userModel.fetchCurrentUser().done(function () {
		var LayoutViewInstance = React.createFactory(LayoutView);
		var layoutViewInstance = LayoutViewInstance({
			user: userModel
		});
		var layout = ReactDOM.render(layoutViewInstance, document.getElementById('main-container'));

		var appRouter = new AppRouter;

		Backbone.history.start();
	}).fail(function () {
		window.location = '../login.html';
	});

	Backbone.history.start();
});