import Backbone from 'backbone'
import React from 'react'
import Topic from 'postal'
import Promise from 'bluebird'
import { assign, unset, each, isString, isFunction, isObject } from 'lodash'
import $ from 'jquery'
import 'foundation-sites'
import { color } from 'd3-color'
import { interpolateTurbo, interpolateRainbow } from 'd3-scale-chromatic'

export const getDefaultProps = function () {
    return {
        registeredTopic: {},
        registeredEvents: [],
    }
}
export const on = function (event, element, callback) {
    $('body').on(event, element, callback)
    this.props.registeredEvents.push(event)
}
export const subscribe = function (opt) {
    var params
    if (arguments.length == 2 && isString(arguments[0]) && isFunction(arguments[1])) {
        params = {
            channel: 'View',
            topic: arguments[0],
            callback: arguments[1],
        }
    } else {
        params = assign(
            {
                channel: 'View',
                topic: 'undefined',
            },
            opt
        )
    }

    var name = params.channel + '.*/-+-*.' + params.topic
    this.props.registeredTopic[name] && this.props.registeredTopic[name].unsubscribe()
    var signal = Topic.subscribe(params)
    this.props.registeredTopic[name] = signal
}
export const unsubscribe = function (topic) {
    var name = 'View' + '.*/-+-*.' + topic
    if (this.props.registeredTopic[name]) {
        this.props.registeredTopic[name].unsubscribe()
        unset(this.props.registeredTopic, name)
    }
}
export const publish = function () {
    var opt
    if (arguments.length == 1 && isString(arguments[0])) {
        opt = {
            channel: 'View',
            topic: arguments[0],
            data: null,
        }
    } else if (arguments.length == 4 && isString(arguments[0]) && isFunction(arguments[1]) && arguments[2] instanceof Backbone.Model && isObject(arguments[3])) {
        opt = {
            channel: 'View',
            topic: arguments[0],
            data: {
                view: arguments[1],
                params: {
                    model: arguments[2],
                },
                options: arguments[3],
            },
        }
    } else if (arguments.length == 4 && isString(arguments[0]) && isFunction(arguments[1]) && arguments[2] instanceof Backbone.Collection && isObject(arguments[3])) {
        opt = {
            channel: 'View',
            topic: arguments[0],
            data: {
                view: arguments[1],
                params: {
                    collection: arguments[2],
                },
                options: arguments[3],
            },
        }
    } else if (arguments.length == 3 && isString(arguments[0]) && isFunction(arguments[1]) && arguments[2] instanceof Backbone.Model) {
        opt = {
            channel: 'View',
            topic: arguments[0],
            data: {
                view: arguments[1],
                params: {
                    model: arguments[2],
                },
            },
        }
    } else if (arguments.length == 3 && isString(arguments[0]) && isFunction(arguments[1]) && arguments[2] instanceof Backbone.Collection) {
        opt = {
            channel: 'View',
            topic: arguments[0],
            data: {
                view: arguments[1],
                params: {
                    collection: arguments[2],
                },
            },
        }
    } else if (arguments.length == 2 && isString(arguments[0]) && isFunction(arguments[1])) {
        opt = {
            channel: 'View',
            topic: arguments[0],
            data: {
                view: arguments[1],
            },
        }
    } else if (arguments.length > 1 && !isFunction(arguments[0])) {
        opt = {
            channel: 'View',
            topic: arguments[0],
            data: arguments[1],
        }
    } else {
        opt = arguments[0]
    }
    Topic.publish(
        assign(
            {
                channel: 'View',
                topic: 'undefined',
            },
            opt
        )
    )
}
export const componentWillUnmount = function () {
    each(this.props.registeredTopic, function (i) {
        i.unsubscribe()
    })
    each(this.props.registeredEvents, function (i) {
        $('body').off(i)
    })
}
export const doUnsubscribe = function () {
    each(this.props.registeredTopic, function (i) {
        i.unsubscribe()
    })
    each(this.props.registeredEvents, function (i) {
        $('body').off(i)
    })
}
export const scrollTop = function (ele) {
    $('.off-canvas-wrapper-inner')
        .stop()
        .animate(
            {
                scrollTop: $(ele).offset().top,
            },
            600
        )
}
export const confirm = function (content) {
    var self = this
    return new Promise((resolve, reject) => {
        var cancel = function () {
            self.publish('showDialogModal')
            reject(new Error('user cancel'))
        }
        var okay = function () {
            self.publish('showDialogModal')
            resolve()
        }
        if (isString(content)) {
            content = content.replace('\r', '').split('\n')
            content = content.map((row, index) => {
                return <p key={`confirm-${index}`}>{row}</p>
            })
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
                        <a href="javascript:;" className="button success tiny" onClick={okay}>
                            Okay
                        </a>
                    </div>
                    <div className="button-group float-right">
                        <a href="javascript:;" className="button tiny" onClick={cancel}>
                            Cancel
                        </a>
                    </div>
                </div>
                <button onClick={cancel} className="close-button" data-close aria-label="Close reveal" type="button">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
        )
        self.publish('showDialogModal', {
            view: view,
        })
    })
}
export const alert = function (content) {
    var self = this
    if (isString(content)) {
        content = content.replace('\r', '').split('\n')
        content = content.map((row, index) => {
            return <p key={`alert-${index}`}>{row}</p>
        })
    }
    var closeDialog = function () {
        self.publish('showDialogModal')
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
                    <a href="javascript:;" className="button tiny" onClick={closeDialog}>
                        Okay
                    </a>
                </div>
            </div>
            <button onClick={closeDialog} className="close-button" data-close aria-label="Close reveal" type="button">
                <span aria-hidden="true">&times;</span>
            </button>
        </div>
    )
    this.publish('showDialogModal', {
        view: view,
    })
}
export const onFormChange = function (e) {
    let model = this.props.model
    if (model) {
        model[e.currentTarget.name] = e.currentTarget.value
    }
}
export const onDialogClose = function () {
    this.publish('showDialog')
}

export const randomColor = function (index) {
    const schema = [
        '#d62728',
        '#f0027f',
        '#fdbf6f',
        '#fdc086',
        '#ff7f0e',
        '#a6cee3',
        '#ffff33',
        '#ff4040',
        '#e2b72f',
        '#8c564b',
        '#cab2d6',
        '#666666',
        '#4daf4a',
        '#bf5b17',
        '#e377c2',
        '#aff05b',
        '#ffff99',
        '#a703d5',
        '#ff7847',
        '#7fc97f',
        '#4c6edb',
        '#bf3caf',
        '#beaed4',
        '#a7d503',
        '#1ddfa3',
        '#e41a1c',
        '#e70b8d',
        '#33a02c',
        '#f781bf',
        '#377eb8',
        '#b2df8a',
        '#1872f4',
        '#58fc2a',
        '#999999',
        '#b15928',
        '#6a3d9a',
        '#18f472',
        '#fb9a99',
        '#23abd8',
        '#a65628',
        '#984ea3',
        '#582afc',
        '#e31a1c',
        '#6e40aa',
        '#9467bd',
        '#00bfbf',
        '#1f77b4',
        '#e78d0b',
        '#7f7f7f',
        '#386cb0',
        '#ff7f00',
        '#bcbd22',
        '#fe4b83',
        '#2ca02c',
        '#52f667',
        '#1f78b4',
    ]
    if (!index) {
        index = Math.round(Math.random() * 100) % schema.length
    }
    return schema[index % schema.length]
}

export default { getDefaultProps, on, subscribe, unsubscribe, publish, componentWillUnmount, doUnsubscribe, scrollTop, confirm, alert, onFormChange, onDialogClose, randomColor }
