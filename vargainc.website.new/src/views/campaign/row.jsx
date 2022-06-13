import Backbone from 'backbone'
import React from 'react'
import 'react.backbone'
import moment from 'moment'
import $ from 'jquery'

import BaseView from 'views/base'
import EditView from './edit'
import PublishView from './publish'

export default React.createBackboneClass({
    mixins: [BaseView],
    menuKey: 'campaign-menu-ddl-',
    getDefaultProps: function () {
        return {
            address: null,
            icon: null,
            name: null,
        }
    },
    componentDidMount: function () {
        $('#' + this.menuKey + this.getModel().get('Id')).foundation()
    },
    onCopy: function (e) {
        e.preventDefault()
        e.stopPropagation()
        $(e.currentTarget).closest('.dropdown-pane').foundation('close')
        var model = this.getModel()
        model.copy().then((response) => {
            if (response && response.success) {
                this.publish('camapign/refresh')
            }
        })
    },
    onEdit: function (e) {
        e.preventDefault()
        e.stopPropagation()
        $(e.currentTarget).closest('.dropdown-pane').foundation('close')
        var model = this.getModel().clone()
        var view = <EditView model={model} />
        this.publish('showDialog', {
            view: view,
        })
    },
    onDelete: function (e) {
        e.preventDefault()
        e.stopPropagation()
        $(e.currentTarget).closest('.dropdown-pane').foundation('close')
        var self = this
        this.confirm('are you sure want delete this campaign?').then(() => {
            var model = self.getModel()
            model.destroy({
                wait: true,
                success: function () {
                    self.publish('camapign/refresh')
                },
            })
        })
    },
    onPublishToDMap: function (e) {
        e.preventDefault()
        e.stopPropagation()
        var model = this.getModel(),
            self = this

        self.publish('showDialog', {
            view: <PublishView />,
        })
        this.unsubscribe('campaign/publish')
        this.subscribe('campaign/publish', function (user) {
            model.publishToDMap(user).then((result) => {
                self.publish('showDialog')
                if (result && result.success) {
                    self.publish('camapign/refresh')
                } else {
                    self.alert(result.error)
                }
            })
        })
    },
    onOpenMoreMenu: function (e) {
        e.preventDefault()
        e.stopPropagation()
    },
    onCloseMoreMenu: function (key, e) {
        e.preventDefault()
        e.stopPropagation()
        $('#' + this.menuKey + key).foundation('close')
    },
    renderMoreMenu: function (key) {
        var id = this.menuKey + key
        return (
            <span>
                <button className="button cirle" data-toggle={id} onClick={this.onOpenMoreMenu}>
                    <i className="fa fa-ellipsis-h"></i>
                </button>
                <div id={id} className="dropdown-pane bottom" data-dropdown data-close-on-click="true" data-auto-focus="false" onClick={this.onCloseMoreMenu.bind(null, key)}>
                    <ul className="vertical menu">
                        <li>
                            <a href="javascript:;" onClick={this.onEdit}>
                                <i className="fa fa-edit"></i>
                                <span>Edit</span>
                            </a>
                        </li>
                        <li>
                            <a href="javascript:;" onClick={this.onCopy}>
                                <i className="fa fa-copy"></i>
                                <span>Copy</span>
                            </a>
                        </li>
                        <li>
                            <a href="javascript:;" onClick={this.onDelete}>
                                <i className="fa fa-trash"></i>
                                <span>Delete</span>
                            </a>
                        </li>
                    </ul>
                </div>
            </span>
        )
    },
    onGotoCMap: function (id) {
        window.open(`./#campaign/${id}`)
    },
    render: function () {
        var model = this.getModel()
        var date = model.get('Date')
        var displayDate = date ? moment(date).format('MMM DD, YYYY') : ''
        return (
            <div className="row scroll-list-item" onClick={this.onGotoCMap.bind(null, model.get('Id'))}>
                <div className="hide-for-small-only medium-2 columns">{model.get('ClientName')}</div>
                <div className="small-12 medium-5 columns">{model.get('ClientCode')}</div>
                <div className="hide-for-small-only medium-2 columns">{displayDate}</div>
                <div className="small-12 medium-3 columns">
                    <span className="show-for-large">{model.get('AreaDescription')}</span>
                    <div className="float-right tool-bar">
                        <button onClick={this.onPublishToDMap} className="button">
                            <i className="fa fa-upload"></i>
                            <small>Publish</small>
                        </button>
                        {this.renderMoreMenu(model.get('Id'))}
                    </div>
                </div>
            </div>
        )
    },
})
