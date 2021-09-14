import Backbone from 'backbone'
import React from 'react'
import 'react.backbone'
import moment from 'moment'
import $ from 'jquery'

import TaskModel from 'models/task'
import BaseView from 'views/base'

export default React.createBackboneClass({
    mixins: [BaseView],
    menuKey: 'report-menu-ddl-',
    getDefaultProps: function () {
        return {
            address: null,
            icon: null,
            name: null,
            scrollToTaskId: null,
        }
    },
    componentDidMount: function () {
        $('.has-tip').foundation()
        var scrollToTaskId = this.props.scrollToTaskId
        if (scrollToTaskId && this.refs[scrollToTaskId]) {
            this.scrollTop(this.refs[scrollToTaskId])
            this.props.scrollToTaskId = null
        }
    },
    onReOpenTask: function (taskId) {
        var self = this
        this.confirm('Do you really want to move report back to GPS Montor?').then(() => {
            var model = new TaskModel({
                Id: taskId,
            })
            model.reOpen({
                success: function (result) {
                    if (result && result.success) {
                        self.publish('report/refresh')
                    } else {
                        self.alert(result.error)
                    }
                },
            })
        })
    },
    onGotoReport: function (taskId) {
        window.location.hash = 'frame/ReportsTask.aspx?tid=' + taskId
    },
    onGotoReview: function (campaignId, taskName, taskId) {
        window.open(`index.html#campaign/${campaignId}/${taskName}/${taskId}/edit`)
    },
    onCloseMoreMenu: function (key) {
        $('#' + this.menuKey + key).foundation('close')
    },
    renderMoreMenu: function (key) {
        var id = this.menuKey + key
        return (
            <span>
                <button className="button cirle" data-toggle={id}>
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
                            <a href="javascript:;" onClick={this.onOpenUploadFile.bind(null, key)}>
                                <i className="fa fa-cloud-upload"></i>
                                <span>Import</span>
                            </a>
                        </li>
                        <input type="file" id={'upload-file-' + key} multiple style={{ display: 'none' }} onChange={this.onImport.bind(null, key)} />
                    </ul>
                </div>
            </span>
        )
    },
    render: function () {
        var self = this,
            model = this.getModel(),
            date = model.get('Date'),
            displayDate = date ? moment(date).format('MMM DD, YYYY') : ''

        return (
            <div className="row scroll-list-item">
                <div className="hide-for-small-only medium-2 columns">{model.get('ClientName')}</div>
                <div className="small-10 medium-5 columns">{model.get('ClientCode')}</div>
                <div className="small-2 medium-2 columns">{displayDate}</div>
                <div className="small-2 medium-3 columns">
                    <span className="show-for-large">{model.get('AreaDescription')}</span>
                    <div className="float-right tool-bar">
                        <a className="button row-button" href={'#frame/handler/PhantomjsPrintHandler.ashx?campaignId=' + model.get('Id') + '&type=print'}>
                            <i className="fa fa-print"></i>
                            <small>Print</small>
                        </a>
                    </div>
                </div>
                <div className="small-12 columns">
                    <table className="hover">
                        <colgroup>
                            <col />
                            <col style={{ width: '160px' }} />
                        </colgroup>
                        <tbody>
                            {model.get('Tasks').map(function (task) {
                                if (task.visiable === false) {
                                    return null
                                }
                                var campaignId = model.get('Id'),
                                    taskName = task.Name
                                return (
                                    <tr key={task.Id} ref={task.Id}>
                                        <td onClick={self.onGotoReview.bind(null, campaignId, taskName, task.Id)}>{task.Name}</td>
                                        <td>
                                            <div className="float-right tool-bar">
                                                <a className="button row-button" onClick={self.onGotoReport.bind(null, task.Id)}>
                                                    <i className="fa fa-file-text-o"></i>
                                                    <small>Report</small>
                                                </a>
                                                <button
                                                    onClick={self.onReOpenTask.bind(null, task.Id)}
                                                    className="button has-tip top"
                                                    title="dismiss"
                                                    data-tooltip
                                                    aria-haspopup="true"
                                                    data-disable-hover="false"
                                                    tabIndex="1"
                                                >
                                                    <i className="fa fa-reply"></i>
                                                </button>
                                            </div>
                                        </td>
                                    </tr>
                                )
                            })}
                        </tbody>
                    </table>
                </div>
            </div>
        )
    },
})
