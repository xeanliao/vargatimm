import axios from 'axios'
import React from 'react'
import { color } from 'd3-color'
import * as Helper from 'views/base'

export default class DMapEdit extends React.Component {
    constructor(props) {
        super(props)

        this.onChange = Helper.onFormChange.bind(this)
        this.closeDialog = Helper.closeDialog.bind(this)
        this.publish = Helper.publish.bind(this)

        this.onSave = this.onSave.bind(this)
        this.onClose = this.onClose.bind(this)
    }

    onSave(e) {
        e.preventDefault()
        e.stopPropagation()
        let model = this.props.model
        let formData = Object.fromEntries(new FormData(e.target))
        let postData = Object.assign({}, model, formData)

        let dMapColor = color(postData.ColorString)
        postData.ColorR = dMapColor.r
        postData.ColorG = dMapColor.g
        postData.ColorB = dMapColor.b

        axios
            .post(`campaign/${model.CampaignId}/${postData.SubMapId}/dmap/edit`, postData)
            .then(() => {
                this.closeDialog()
                Helper.publish('DMap.Refresh')
                return Promise.resolve()
            })
            .catch(() => {
                this.setState({
                    error: 'opps! something wrong. please contact us!',
                })
            })
    }

    onClose() {
        this.closeDialog()
    }

    render() {
        var model = this.props.model
        var title = model.Id ? 'Edit Distribution Map' : 'New Distribution Map'
        var showError = this.state && this.state.error ? true : false
        var errorMessage = showError ? this.state.error : ''
        return (
            <form onSubmit={this.onSave}>
                <h3>{title}</h3>
                <div data-abide-error className="alert callout" style={{ display: showError ? 'block' : 'none' }}>
                    <p>
                        <i className="fa fa-exclamation-circle"></i>&nbsp;{errorMessage}
                    </p>
                </div>
                <div className="row">
                    <div className="small-2 columns">
                        <label>
                            Color
                            <input type="color" className="padding-0" onChange={this.onChange} name="ColorString" defaultValue={`#${model.ColorString}`} required />
                            <span className="form-error">it is required.</span>
                        </label>
                    </div>
                    <div className="small-10 columns">
                        <label>
                            Name
                            <input onChange={this.onChange} name="Name" type="text" defaultValue={model.Name} required />
                            <span className="form-error">it is required.</span>
                        </label>
                    </div>
                </div>
                <div className="row">
                    <div className="small-12 columns">
                        <label>
                            Submap
                            <select name="SubMapId" disabled={model.SubMapId} required>
                                {this.props.model.SubMaps.map((s) => {
                                    return (
                                        <option key={s.Id} value={s.Id} selected={s.Id == model.SubMapId}>
                                            {s.Name}
                                        </option>
                                    )
                                })}
                            </select>
                            <span className="form-error">it is required.</span>
                        </label>
                    </div>
                </div>
                <div className="small-12 columns">
                    <div className="button-group float-right">
                        <button type="submit" className="success button">
                            Save
                        </button>
                        <a href="javascript:;" className="button" onClick={this.onClose}>
                            Cancel
                        </a>
                    </div>
                </div>
                <button onClick={this.onClose} className="close-button" data-close aria-label="Close reveal" type="button">
                    <span aria-hidden="true">&times;</span>
                </button>
            </form>
        )
    }
}