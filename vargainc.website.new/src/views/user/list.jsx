import React from 'react'
import createReactClass from 'create-react-class'
import BaseView from 'views/base'
import 'datatables.net'

import ResetPasswordView from './resetPassword'
import EditView from './edit'
import axios from 'axios'

const columns = [
    { title: 'UserName', data: 'UserName' },
    { title: 'UserCode', data: 'UserCode' },
    { title: 'FullName', data: 'FullName' },
    { title: 'Email', data: 'Email' },
    { title: 'CellPhone', data: 'CellPhone' },
    { title: 'Notes', data: 'Notes' },
    {
        title: 'Groups',
        data: 'Groups',
        visible: false,
        render: function (data) {
            return data.map((i) => `<span class="label">${i}</span>`).join('&nbsp;')
        },
    },
    { title: 'LastLoginTime', data: 'LastLoginTime' },
    {
        title: 'Action',
        width: 240,
        data: 'Id',
        orderable: false,
        render: (data, type, row, meta) => {
            return `
                <div class="small button-group margin-0">
                    <a class="button" userId="${data}" username="${row.UserName}" action="reset">Reset Password</a>
                    <a class="button" userId="${data}" username="${row.UserName}" action="edit">Edit</a>
                    <a class="button alert" style="color: white;" userId="${data}" username="${row.UserName}" action="remove">Remove</a>
                </div>
            `
        },
    },
]
export default createReactClass({
    mixins: [BaseView],
    getInitialState: function () {
        return {
            tableInit: false,
        }
    },
    shouldComponentUpdate: function () {
        return !this.state.tableInit
    },
    onTableInit: function (el) {
        if (!this.state.tableInit && el) {
            let dataTable = $('.userTable').DataTable({
                processing: true,
                serverSide: true,
                ordering: true,
                pageLength: 10,
                info: false,
                ajax: {
                    url: 'api/user/list',
                    type: 'POST',
                    data: (data) => {
                        let order = { field: 'Id', dir: 'desc' }
                        let orderColumnIndex = data?.order?.[0]?.column
                        if (orderColumnIndex != null) {
                            order.field = columns?.[orderColumnIndex]?.data
                            order.dir = data?.order?.[0]?.dir
                        }
                        return JSON.stringify({
                            draw: data?.draw,
                            start: data?.start,
                            length: data?.length,
                            search: data?.search?.value,
                            order: order,
                        })
                    },
                    contentType: 'application/json',
                },
                columns: columns,
                columnDefs: [],
            })
            this.setState({ tableInit: true, dataTable: dataTable })
        }
    },
    onAction: function (evt) {
        let src = evt.target
        let action = src.attributes['action']?.value
        let userId = src.attributes['userId']?.value
        let username = src.attributes['username']?.value
        switch (action) {
            case 'reset':
                this.publish('showDialog', {
                    view: ResetPasswordView,
                    params: { userId: userId, username: username },
                    options: { size: 'tiny' },
                })
                break
            case 'edit':
                this.publish('showDialog', {
                    view: EditView,
                    params: { userId: userId, username: username },
                    options: { size: 'tiny' },
                })
                break
            case 'remove':
                this.confirm(`Are you sure to delete user ${username}?`).then(() => {
                    axios.delete(`user/${userId}`).then((resp) => {
                        if (resp.data.success == true) {
                            this.onRefresh()
                        }
                    })
                })
                break
        }
    },
    onRefresh: function () {
        this.state.dataTable && this.state.dataTable.ajax.reload(null, false)
    },
    render: function () {
        return (
            <div className="user-list margin-2">
                <button className="button">Add User</button>
                <table ref={this.onTableInit} className="userTable" onClick={this.onAction}></table>
            </div>
        )
    },
})
