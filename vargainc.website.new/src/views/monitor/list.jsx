import Backbone from 'backbone'
import React from 'react'
import 'react.backbone'
import { each, uniq, orderBy, filter, values, some, toString, indexOf, map } from 'lodash'
import $ from 'jquery'
import moment from 'moment'

import BaseView from 'views/base'
import MonitorRow from './row'

export default React.createBackboneClass({
    mixins: [BaseView],
    getInitialState: function () {
        return {
            orderByFiled: null,
            orderByAsc: false,
            search: null,
            filterField: null,
            filterValues: [],
        }
    },
    componentDidMount: function () {
        var self = this
        this.subscribe('monitor/refresh', function () {
            self.getCollection().fetchForTask()
        })
        this.subscribe('search', function (words) {
            self.setState({
                search: words,
                filterField: null,
                filterValues: [],
            })
        })

        $('#monitor-filter-ddl-ClientName, #monitor-filter-ddl-ClientCode, #monitor-filter-ddl-Date, #monitor-filter-ddl-AreaDescription').foundation()
    },
    onOrderBy: function (field, e) {
        e.preventDefault()
        e.stopPropagation()

        if (this.state.orderByFiled == field) {
            this.setState({
                orderByAsc: !this.state.orderByAsc,
            })
        } else {
            this.setState({
                orderByFiled: field,
                orderByAsc: true,
            })
        }
    },
    onFilter: function (field, e) {
        e.preventDefault()
        e.stopPropagation()

        var els = $(':checked', e.currentTarget),
            values = map(els, function (item) {
                return $(item).val()
            })

        this.setState({
            filterField: values.length > 0 ? field : null,
            filterValues: values.length > 0 ? values : [],
            search: null,
        })
        $('#monitor-filter-ddl-' + field).foundation('close')
    },
    onClearFilter: function (field, e) {
        e.preventDefault()
        e.stopPropagation()
        $('#monitor-filter-ddl-' + field).foundation('close')
        $(e.currentTarget).closest('form').get(0).reset()

        this.setState({
            filterField: null,
            filterValues: [],
            search: null,
        })
    },
    renderHeader: function (field, displayName) {
        var dataSource = this.getCollection(),
            sortIcon = null,
            filterIcon = null,
            filterMenu = null
        dataSource = dataSource ? dataSource.toArray() : []

        if (this.state.orderByFiled == field) {
            if (this.state.orderByAsc) {
                sortIcon = <i className="fa fa-sort-up active"></i>
            } else {
                sortIcon = <i className="fa fa-sort-down active"></i>
            }
        } else {
            sortIcon = <i className="fa fa-sort"></i>
        }
        sortIcon = <a onClick={this.onOrderBy.bind(null, field)}>&nbsp;{sortIcon}</a>

        if (this.state.filterField && this.state.filterField == field) {
            filterIcon = (
                <a data-toggle={'monitor-filter-ddl-' + field}>
                    &nbsp;<i className="fa fa-filter active"></i>
                </a>
            )
        }
        if (dataSource) {
            var fieldValues = map(dataSource, function (i) {
                var fieldValue = i.get(field)
                var dateCheck = moment(fieldValue, moment.ISO_8601)
                if (dateCheck.isValid()) {
                    return dateCheck.format('MMM DD, YYYY')
                }
                return fieldValue
            })
            var menuItems = uniq(fieldValues).sort()
            filterMenu = (
                <div id={'monitor-filter-ddl-' + field} className="dropdown-pane bottom" style={{ width: 'auto' }} data-dropdown data-close-on-click="true" data-auto-focus="false">
                    <form onSubmit={this.onFilter.bind(this, field)}>
                        <ul className="vertical menu">
                            {menuItems.map(function (item, index) {
                                return (
                                    <li key={field + index}>
                                        <input id={field + index} name={field} type="checkbox" value={item} />
                                        <label htmlFor={field + index}>{item}</label>
                                    </li>
                                )
                            })}
                        </ul>
                        <button className="button tiny success" type="submit">
                            Filter
                        </button>
                        <button className="button tiny warning" type="reset" onClick={this.onClearFilter.bind(this, field)}>
                            Clear
                        </button>
                    </form>
                </div>
            )
        }

        return (
            <div>
                <a data-toggle={'monitor-filter-ddl-' + field}>{displayName}</a>
                {sortIcon}
                {filterIcon}
                {filterMenu}
            </div>
        )
    },
    getDataSource: function () {
        var dataSource = this.getCollection()
        dataSource = dataSource ? dataSource.toArray() : []
        if (this.state.orderByFiled) {
            dataSource = orderBy(dataSource, ['attributes.' + this.state.orderByFiled], [this.state.orderByAsc ? 'asc' : 'desc'])
        }
        if (this.state.search) {
            var keyword = this.state.search.toLowerCase(),
                campaignValues = null,
                campaignSearch = null,
                taskValues = null,
                taskSearch = null
            dataSource = filter(dataSource, function (i) {
                campaignValues = values(i.attributes)
                campaignSearch = some(campaignValues, function (i) {
                    var dateCheck = moment(i, moment.ISO_8601)
                    if (dateCheck.isValid()) {
                        return dateCheck.format('MMM DD YYYY MMM DD, YYYY YYYY-MM-DD MM/DD/YYYY YYYY MM MMM DD').toLowerCase().indexOf(keyword) > -1
                    }
                    return toString(i).toLowerCase().indexOf(keyword) > -1
                })
                /**
                 * update task visiable logical.
                 * if campaign in search keyward. show all task
                 * otherwise only show task name in search word.
                 * if there is no task need show hide this campaign.
                 */
                taskValues = values(i.attributes.Tasks)
                each(taskValues, function (i) {
                    i.visiable = campaignSearch || i.Name.toLowerCase().indexOf(keyword) > -1
                })
                return (
                    campaignSearch ||
                    some(taskValues, {
                        visiable: true,
                    })
                )
            })
        } else if (this.state.filterField && this.state.filterValues) {
            var filterField = this.state.filterField,
                filterValues = this.state.filterValues
            dataSource = filter(dataSource, function (i) {
                var fieldValue = i.get(filterField)
                var dateCheck = moment(fieldValue, moment.ISO_8601)
                if (dateCheck.isValid()) {
                    return indexOf(filterValues, dateCheck.format('MMM DD, YYYY')) > -1
                }
                return indexOf(filterValues, fieldValue) > -1
            })
        }
        return dataSource
    },
    render: function () {
        var list = this.getDataSource()
        return (
            <div className="section row">
                <div className="small-12 columns">
                    <div className="section-header">
                        <div className="row">
                            <div className="small-12 column">
                                <h5>GPS Monitor</h5>
                            </div>
                            <div className="small-12 column">
                                <nav aria-label="You are here:" role="navigation">
                                    <ul className="breadcrumbs">
                                        <li>
                                            <a href="#">Control Center</a>
                                        </li>
                                        <li>
                                            <span className="show-for-sr">Current: </span> GPS Monitor
                                        </li>
                                    </ul>
                                </nav>
                            </div>
                        </div>
                    </div>
                    <div className="scroll-list-section-body">
                        <div className="row scroll-list-header">
                            <div className="hide-for-small-only medium-2 columns">{this.renderHeader('ClientName', 'Client Name')}</div>
                            <div className="small-12 medium-5 columns">{this.renderHeader('ClientCode', 'Client Code')}</div>
                            <div className="hide-for-small-only medium-2 columns">{this.renderHeader('Date', 'Date')}</div>
                            <div className="small-12 medium-3 columns">
                                <span className="show-for-large">{this.renderHeader('AreaDescription', 'Type')}</span>
                            </div>
                        </div>
                        {list.map(function (item) {
                            return <MonitorRow key={item.get('Id')} model={item} />
                        })}
                    </div>
                </div>
            </div>
        )
    },
})
