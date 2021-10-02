import $ from 'jquery'
import Backbone from 'backbone'
import { hot } from 'react-hot-loader/root'
import React from 'react'
import ReactDOM from 'react-dom'
import Topic from 'postal'
import { isFunction, startsWith } from 'lodash'
import AppRouter from 'route'
import UserModel from 'models/user'
import LayoutView from 'views/layout/main'
import Promise from 'bluebird'
import Axios from 'axios'

/**
 * register loading event except ajax request is quite
 */
$(document).ajaxSend(function (event, xhr, settings) {
    if (settings.quite !== true) {
        Topic.publish({
            channel: 'View',
            topic: 'showLoading',
        })
    }
})
$(document).ajaxComplete(function (event, xhr, settings) {
    if (settings.quite !== true) {
        Topic.publish({
            channel: 'View',
            topic: 'hideLoading',
        })
    }
})

Axios.interceptors.request.use(
    function (config) {
        Topic.publish({
            channel: 'View',
            topic: 'showLoading',
        })
        return config
    },
    function (error) {
        Topic.publish({
            channel: 'View',
            topic: 'hideLoading',
        })
        return Promise.reject(error)
    }
)

// Add a response interceptor
Axios.interceptors.response.use(
    function (response) {
        Topic.publish({
            channel: 'View',
            topic: 'hideLoading',
        })
        return response
    },
    function (error) {
        Topic.publish({
            channel: 'View',
            topic: 'hideLoading',
        })
        return Promise.reject(error)
    }
)

/**
 * override base url
 */
var backboneSync = Backbone.sync
Backbone.sync = function (method, model, options) {
    if (!options.url) {
        options.url = isFunction(model.url) ? model.url() : model.url
    }
    if (!options.url) {
        options.url = model.urlRoot
    }
    options.url = './api/' + options.url
    if (typeof RELEASE_VERSION !== 'undefined') {
        options.url = '//timm.vargainc.com/' + RELEASE_VERSION + '/api/' + options.url
    }
    return backboneSync(method, model, options)
}

if (typeof RELEASE_VERSION !== 'undefined') {
    Axios.defaults.baseURL = '//timm.vargainc.com/' + RELEASE_VERSION + '/api/'
} else {
    let baseUrl = new URL('./api/', `${location.protocol}//${location.host}${location.pathname}`)
    Axios.defaults.baseURL = baseUrl.toString()
}

var userModel = new UserModel()
userModel.fetchCurrentUser().then(function (isSuccess) {
    if (!isSuccess && !startsWith(window.location.hash, '#driver')) {
        window.location = './login.html'
        return Promise.reject()
    }
    var LayoutViewInstance = React.createFactory(hot(LayoutView))
    var layoutViewInstance = LayoutViewInstance({
        user: userModel,
    })
    ReactDOM.render(layoutViewInstance, document.getElementById('main-container'))
    var appRouter = new AppRouter()
    appRouter.on('route', function () {})
    Backbone.history.start()
    return Promise.resolve()
})
