import React from 'react'
import axios from 'axios'

import Helper from 'views/base'

export default class EditNdAddress extends React.Component {
    constructor(props) {
        super(props)

        this.onChange = Helper.onFormChangeUseState.bind(this)

        this.onSave = this.onSave.bind(this)
    }

    onSave(e) {
        e.preventDefault()
        e.stopPropagation()
        axios
            .post('nd/address', {
                street: this.state.street,
                zipCode: this.state.zipCode,
                geoFence: this.state.geoFence,
                desc: this.state.desc,
                point: [this.props.geom.lng, this.props.geom.lat],
            })
            .then((resp) => {
                if (resp.data.success) {
                    Helper.publish('ndRefresh')
                }
            })
            .finally(() => {
                Helper.closeDialog()
            })
    }

    render() {
        var showError = this.state && this.state.error ? true : false
        var errorMessage = showError ? this.state.error : ''
        let id = this.props?.id
        let street = this.props?.street
        let zipCode = this.props?.zipCode
        let desc = this.props?.desc
        let geoFence = this.props?.geoFence ?? 500
        return (
            <form onSubmit={this.onSave}>
                <h3>{id ? 'Edit Nd Address' : 'Create New Nd Address'}</h3>
                <div data-abide-error className="alert callout" style={{ display: showError ? 'block' : 'none' }}>
                    <p>
                        <i className="fa fa-exclamation-circle"></i>&nbsp;{errorMessage}
                    </p>
                </div>
                <div className="row">
                    <div className="small-12 columns">
                        <label>
                            Street
                            <input onChange={this.onChange} name="street" type="text" defaultValue={street} required />
                            <span className="form-error">it is required.</span>
                        </label>
                    </div>
                    <div className="small-12 columns">
                        <label>
                            Zip Code
                            <input onChange={this.onChange} name="zipCode" type="text" defaultValue={zipCode} required />
                        </label>
                    </div>
                    <div className="small-12 columns">
                        <label>
                            GeoFence
                            <input onChange={this.onChange} name="geoFence" type="text" defaultValue={geoFence} required min="0" step="1" />
                        </label>
                    </div>
                    <div className="small-12 columns">
                        <label>
                            Description
                            <input onChange={this.onChange} name="desc" type="text" defaultValue={desc} required />
                        </label>
                    </div>
                </div>
                <div className="small-12 columns">
                    <div className="button-group float-right">
                        <button type="submit" className="success button">
                            Save
                        </button>
                        <a
                            className="button"
                            onClick={() => {
                                Helper.closeDialog()
                            }}
                        >
                            Cancel
                        </a>
                    </div>
                </div>
                <button
                    onClick={() => {
                        Helper.closeDialog()
                    }}
                    className="close-button"
                    data-close
                    aria-label="Close reveal"
                    type="button"
                >
                    <span aria-hidden="true">&times;</span>
                </button>
            </form>
        )
    }
}
