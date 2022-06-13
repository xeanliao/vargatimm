import React from 'react'
import createReactClass from 'create-react-class'
import BaseView from 'views/base'
import axios from 'axios'
import moment from 'moment'
import { groupBy } from 'lodash'
import $ from 'jquery'
import 'select2'
import 'datatables.net'

import BagView from './bag'
import AuditorView from './auditor'

export default createReactClass({
    mixins: [BaseView],
    getInitialState: function () {
        return {
            pageInit: false,
            selectedTaskId: null,
            selectedGtuId: null,
            tasks: [],
            gtus: [],
        }
    },
    shouldComponentUpdate: function () {
        return !this.state.pageInit
    },
    componentDidMount: function () {
        axios.get(`task/active`).then((resp) => {
            let tasks = resp?.data ?? []

            let dataTable = $('.gtuTable').DataTable({
                pageLength: 10,
                ordering: false,
                dom: '<"row"<"columns small-4"l><"columns small-4 selector-container"><"columns small-4"f>>tip',
                columns: [
                    { data: 'BagId' },
                    { data: 'ShortUniqueID' },
                    {
                        data: 'UserColor',
                        render: (data, type, row) => {
                            if (row?.Id == this.state?.selectedGtuId) {
                                return `<input id="userColor" type="color" style="margin: 0 auto;" />`
                            }
                            return data ? `<div class="color-block" style="background: ${data}"></div>` : null
                        },
                    },
                    {
                        render: (data, type, row) => {
                            if (row?.Id == this.state?.selectedGtuId) {
                                return `<select id="rowSelectorUser"><option></option></select>`
                            }
                            return `
                                <div class="row">
                                    <div class="columns">${row?.Company}</div>
                                    <div class="columns">${row?.Auditor}</div>
                                    <div class="columns">${row?.Role}</div>
                                </div>
                            `
                        },
                    },
                    {
                        data: 'IsOnline',
                        render: (data) => {
                            return data ? 'Online' : 'Offline'
                        },
                    },
                    {
                        orderable: false,
                        width: 240,
                        render: (data, type, row, meta) => {
                            if (row?.Id == this.state?.selectedGtuId) {
                                return `
                                    <div class="small button-group align-center margin-0">
                                        <a class="button primary" gtuId="${row?.Id}" action="save" style="width: 60px;">Save</a>
                                        <a class="button secondary" gtuId="${row?.Id}" action="cancel" style="width: 60px;">Cancel</a>
                                    </div>
                                `
                            }

                            if (row?.IsAssign) {
                                return `
                                    <div class="small button-group align-center margin-0">
                                        <a class="button warning" gtuId="${row?.Id}" action="unassign" style="width: 60px;">Unassign</a>
                                    </div>
                                `
                            } else {
                                return `
                                    <div class="small button-group align-center margin-0">
                                        <a class="button" gtuId="${row?.Id}" action="assign" style="width: 60px;">Assign</a>
                                    </div>
                                `
                            }
                        },
                    },
                ],
                columnDefs: [],
            })

            dataTable.on('draw', () => {
                if ($('#rowSelectorUser').length > 0) {
                    $('#rowSelectorUser').select2({
                        placeholder: 'Please Select Monitor',
                        tags: false,
                        multiple: false,
                        data: this.state.users,
                        theme: 'foundation',
                    })
                }
            })

            $('.selector-container').html(`<select id="monitor"><option>Please Select Monitor</option></select>`)

            $('#monitor').select2({
                placeholder: 'Please Select Monitor',
                tags: false,
                multiple: false,
                data: tasks.map((t) => {
                    return {
                        id: t.Id,
                        text: `${t.Name} @ ${moment(t.Date).format('dddd, MMMM Do YYYY')}`,
                    }
                }),
                theme: 'foundation',
            })

            $(document).on('select2:select', '#monitor', (evt) => {
                let taskId = evt?.params?.data?.id
                if (taskId) {
                    this.setState({ selectedTaskId: taskId }, this.loadGtusByTask)
                } else {
                    this.setState({ gtus: [], selectedTaskId: null }, this.state.dataTable.clear)
                }
            })

            this.setState({ pageInit: true, tasks: tasks, dataTable: dataTable })
        })
    },
    loadGtusByTask: function () {
        axios.get(`gtu/task/${this.state.selectedTaskId}`).then((resp) => {
            this.setState({ gtus: resp.data }, () => {
                this.state.dataTable.clear()
                this.state.dataTable.rows.add(this.state.gtus).draw()
            })
        })
    },
    redraw: function () {
        this.state.dataTable.clear().rows.add(this.state.gtus).draw()
    },
    onAction: function (evt) {
        let src = evt.target
        let action = src.attributes['action']?.value
        let gtuId = src.attributes['gtuId']?.value
        switch (action) {
            case 'assign':
                axios.get(`user/gtu`).then((resp) => {
                    let groupData = groupBy(resp.data, (i) => i?.CompanyId)
                    let select2Data = Object.keys(groupData).map((k) => {
                        return {
                            text: groupData[k][0].CompanyName,
                            children: groupData[k].map((i) => {
                                return { id: i.UserId, text: `${i.UserName} - ${i.Role}` }
                            }),
                        }
                    })
                    this.setState({ users: select2Data, selectedGtuId: gtuId }, this.redraw)
                })
                break
            case 'save': {
                let color = $('#userColor').val()
                let auditorId = $('#rowSelectorUser').val()
                axios.put(`/gtu/task/assign/`, { TaskId: this.state.selectedTaskId, Id: this.state.selectedGtuId, UserColor: color, AuditorId: auditorId }).then(() => {
                    this.setState({ selectedGtuId: null }, this.loadGtusByTask)
                })
                break
            }
            case 'cancel':
                $('#rowSelectorUser').select2('destroy')
                this.setState({ selectedGtuId: null }, this.redraw)
                break
            case 'unassign': {
                this.confirm(`Are you sure to unassign this GTU`).then(() => {
                    axios.delete(`/gtu/task/${this.state.selectedTaskId}/unassign/${gtuId}`).then(this.loadGtusByTask)
                })
                break
            }
        }
    },
    openBagModal: function () {
        this.publish('showDialog', {
            view: BagView,
            options: { size: 'tiny' },
        })
    },
    openAuditorModal: function () {
        this.publish('showDialog', {
            view: AuditorView,
            options: { size: 'tiny' },
        })
    },
    render: function () {
        return (
            <div className="gtu-list margin-2">
                <div className="row">
                    <div className="columns small-12">
                        <button className="button" onClick={this.openBagModal}>
                            GTU in Bags
                        </button>
                        <button className="button" onClick={this.openAuditorModal}>
                            Bag in Auditors
                        </button>
                    </div>
                    <div className="columns small-12">
                        <table ref={this.onTableInit} className="gtuTable hover text-center" onClick={this.onAction}>
                            <colgroup>
                                <col style={{ textAlign: 'center' }} />
                                <col style={{ textAlign: 'center' }} />
                                <col style={{ textAlign: 'center' }} />
                                <col style={{ textAlign: 'center' }} />
                                <col style={{ textAlign: 'center' }} />
                            </colgroup>
                            <thead>
                                <tr>
                                    <th className="text-center">Bag#</th>
                                    <th className="text-center">GTU#</th>
                                    <th className="text-center">Color</th>
                                    <th>
                                        <div className="row">
                                            <div className="columns text-center">Team</div>
                                            <div className="columns text-center">Auditor</div>
                                            <div className="columns text-center">Role</div>
                                        </div>
                                    </th>
                                    <th className="text-center">Status</th>
                                    <th className="text-center">Actions</th>
                                </tr>
                            </thead>
                        </table>
                    </div>
                </div>
            </div>
        )
    },
})
