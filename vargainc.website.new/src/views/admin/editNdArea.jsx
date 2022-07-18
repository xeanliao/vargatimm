import React from 'react'
import axios from 'axios'

import Helper from 'views/base'

export default class EditNdArea extends React.Component {
    constructor(props) {
        super(props)

        this.onChange = Helper.onFormChangeUseState.bind(this)

        this.onSave = this.onSave.bind(this)
    }

    onSave(e) {
        e.preventDefault()
        e.stopPropagation()
        // let fixedGeojson = rewind(this.props.geom)
        axios
            .post('nd/area', {
                name: this.state.name,
                total: this.state.total,
                desc: this.state.desc,
                geom: this.props.geom.features?.[0]?.geometry?.coordinates?.[0],
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
        let name = this.props?.name
        let desc = this.props?.desc
        let total = this.props?.total
        return (
            <form onSubmit={this.onSave}>
                <h3>{id ? 'Edit Custom Area' : 'Create New Custom Area'}</h3>
                <div data-abide-error className="alert callout" style={{ display: showError ? 'block' : 'none' }}>
                    <p>
                        <i className="fa fa-exclamation-circle"></i>&nbsp;{errorMessage}
                    </p>
                </div>
                <div className="row">
                    <div className="small-12 columns">
                        <label>
                            Name
                            <input onChange={this.onChange} name="name" type="text" defaultValue={name} required />
                            <span className="form-error">it is required.</span>
                        </label>
                    </div>
                    <div className="small-12 columns">
                        <label>
                            Total
                            <input onChange={this.onChange} name="total" type="number" defaultValue={total} required min="0" step="1" />
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
