import React from 'react'
import createReactClass from 'create-react-class'
import BaseView from 'views/base'
import axios from 'axios'

export default createReactClass({
    mixins: [BaseView],
    getInitialState: function () {
        return {}
    },
    componentDidMount: function () {
        Promise.all([
            axios.get(`user/${this.props.userId}`).then((resp) => Promise.resolve(resp.data)),
            axios.get(`user/groups`).then((resp) => Promise.resolve({ AllGroups: resp.data })),
        ]).then((data) => {
            let entity = Object.assign.apply(null, data)
            this.setState(entity)
        })
    },
    onClose: function () {
        this.publish('showDialog')
        return false
    },
    onSubmit: function (evt) {
        evt && evt.preventDefault()
        evt && evt.stopPropagation()
        var formData = new FormData(evt.currentTarget)
        var data = {}
        formData.forEach((value, key) => {
            // Reflect.has in favor of: object.hasOwnProperty(key)
            if (!Reflect.has(data, key)) {
                data[key] = value
                return
            }
            if (!Array.isArray(data[key])) {
                data[key] = [data[key]]
            }
            data[key].push(value)
        })
        data.Groups = (data?.Groups ?? []).map((g) => parseInt(g))
        axios.post(`user/edit/${this.props.userId}`, data).then((resp) => {
            if (resp?.data?.success) {
                this.onClose()
            }
        })
    },
    render: function () {
        // i.Id, i.UserName, i.UserCode, i.FullName, i.Email, i.CellPhone, i.LastLoginTime, i.DateOfBirth, i.Notes, Groups
        const fields = ['UserCode', 'FullName', 'Email', 'CellPhone', 'DateOfBirth', 'Notes']
        return (
            <div>
                <button className="close-button" onClick={this.onClose}>
                    <span aria-hidden="true">×</span>
                </button>
                <form onSubmit={this.onSubmit} className="margin-top-1">
                    <div className="grid-container">
                        <h5 className="text-center">Edit User {this.props.username}</h5>
                        <div className="grid-x grid-padding-x">
                            {fields.flatMap((item) => {
                                let inputType = 'text'
                                switch (item) {
                                    case 'Email':
                                        inputType = 'email'
                                        break
                                    case 'DateOfBirth':
                                        inputType = 'date'
                                        break
                                }
                                return [
                                    <div key={`${item}-label`} className="small-3 cell">
                                        <label htmlFor={item} className="text-right middle">
                                            {item}
                                        </label>
                                    </div>,
                                    <div key={`${item}-input`} className="small-9 cell">
                                        <input type={inputType} id={item} name={item} placeholder={item} value={this.state?.[item]} onChange={this.onFormChangeUseState} />
                                    </div>,
                                ]
                            })}
                            <div className="small-12 cell">
                                <fieldset className="fieldset margin-top-0">
                                    <legend style={{ textAlign: 'center' }}>Groups</legend>
                                    {(this.state?.AllGroups ?? []).flatMap((g) => {
                                        return (
                                            <div className="display-inline-block">
                                                <input
                                                    key={`group-${g?.Id}-input`}
                                                    id={`group-${g?.Id}`}
                                                    name="Groups"
                                                    value={g?.Id}
                                                    type="checkbox"
                                                    checked={(this.state?.Groups ?? []).indexOf(g?.Id) > -1}
                                                    onChange={(evt) => this.onFormChangeUseState(evt, true)}
                                                />
                                                <label key={`group-${g?.Id}-label`} htmlFor={`group-${g?.Id}`}>
                                                    {g?.Name}
                                                </label>
                                            </div>
                                        )
                                    })}
                                </fieldset>
                            </div>
                            <div className="small-12 text-right">
                                <button type="submit" className="button">
                                    Save
                                </button>
                                <button className="button margin-left-1 secondary" onClick={this.onClose}>
                                    Cancel
                                </button>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        )
    },
})
