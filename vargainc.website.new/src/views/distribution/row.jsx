import Backbone from 'backbone'
import React from 'react'
import 'react.backbone'
import moment from 'moment'
import $ from 'jquery'

import BaseView from 'views/base'
import PublishView from './publish'
import DismissView from './dismiss'

export default React.createBackboneClass({
    mixins: [BaseView],
    getDefaultProps: function () {
        return {
            address: null,
            icon: null,
            name: null,
        }
    },
    componentDidMount: function () {
        $('.has-tip').foundation()
    },
    componentDidUpdate: function () {},
    onDismiss: function (e) {
        e.preventDefault()
        e.stopPropagation()

        var self = this,
            model = this.getModel(),
            msg = `Are you sure you would like to move \r\n${model.getDisplayName()}\r\nto Campaigns? Any changes that were made will be lost.`

        this.confirm(msg).then(() => {
            self.publish('showDialog', {
                view: DismissView,
            })
            self.unsubscribe('distribution/dismiss')
            self.subscribe('distribution/dismiss', function (user) {
                model.dismissToCampaign(user, {
                    success: function (result) {
                        self.publish('showDialog')
                        if (result && result.success) {
                            self.publish('distribution/refresh')
                        } else {
                            alert('something wrong')
                        }
                    },
                })
            })
        })
    },
    onPublishToMonitors: function (e) {
        e.preventDefault()
        e.stopPropagation()
        var model = this.getModel(),
            self = this
        this.publish('showDialog', {
            view: PublishView,
        })
        this.unsubscribe('distribution/publish')
        this.subscribe('distribution/publish', function (user) {
            model.publishToMonitor(user, {
                success: function (result) {
                    self.publish('showDialog')
                    if (result && result.success) {
                        self.publish('distribution/refresh')
                    } else {
                        alert(result.error)
                    }
                },
            })
        })
    },
    onGotoDMap: function (id) {
        window.open(`./#campaign/${id}/dmap`)
    },
    render: function () {
        var model = this.getModel()
        var date = model.get('Date')
        var displayDate = date ? moment(date).format('MMM DD, YYYY') : ''
        return (
            <div className="row scroll-list-item" onClick={this.onGotoDMap.bind(null, model.get('Id'))}>
                <div className="hide-for-small-only medium-2 columns">{model.get('ClientName')}</div>
                <div className="small-12 medium-5 columns">{model.get('ClientCode')}</div>
                <div className="hide-for-small-only medium-2 columns">{displayDate}</div>
                <div className="small-12 medium-3 columns">
                    <span className="show-for-large">{model.get('AreaDescription')}</span>
                    <div className="float-right tool-bar">
                        <button onClick={this.onPublishToMonitors} className="button">
                            <i className="fa fa-upload"></i>
                            <small>Publish</small>
                        </button>
                        <button onClick={this.onDismiss} className="button has-tip top" title="dismiss" data-tooltip aria-haspopup="true" data-disable-hover="false" tabIndex="1">
                            <i className="fa fa-reply"></i>
                        </button>
                    </div>
                </div>
            </div>
        )
    },
})
