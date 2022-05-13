import React from 'react'
import createReactClass from 'create-react-class'
import BaseView from 'views/base'
import axios from 'axios'

export default createReactClass({
    mixins: [BaseView],
    getInitialState: function () {
        return {
            error: false,
        }
    },
    onClose: function () {
        this.publish('showDialog')
        return false
    },
    onSubmit: function (evt) {
        evt && evt.preventDefault()
        evt && evt.stopPropagation()
        let password = (this.state?.password ?? '').trim()
        let confirmPassword = (this.state?.confirmPassword ?? '').trim()
        if (password.length == 0 || password != confirmPassword) {
            this.setState({ error: true })
        } else {
            axios.post('user/reset/password', { Id: this.props.userId, Password: password }).then((resp) => {
                if (resp.data.success == true) {
                    this.onClose()
                }
            })
        }
        return false
    },
    render: function () {
        return (
            <div>
                <button className="close-button" onClick={this.onClose}>
                    <span aria-hidden="true">×</span>
                </button>
                <form onSubmit={this.onSubmit} className="margin-top-1">
                    <div className="grid-container">
                        <h5 className="text-center">
                            Are you sure want reset <h4 className="display-inline">{this.props.username}</h4>
                            &apos;s Passwrod?
                        </h5>
                        <div className="grid-x grid-padding-x">
                            <div className="small-4 cell">
                                <label htmlFor="password" className="text-right middle">
                                    Password
                                </label>
                            </div>
                            <div className="small-8 cell">
                                <input type="password" id="password" name="password" placeholder="Password" onChange={this.onFormChangeUseState} required />
                            </div>
                            <div className="small-4 cell">
                                <label htmlFor="confirm-password" className="text-right middle">
                                    Confirm Password
                                </label>
                            </div>
                            <div className="small-8 cell">
                                <input type="password" id="confirm-password" name="confirmPassword" placeholder="Confirm Password" onChange={this.onFormChangeUseState} required />
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
