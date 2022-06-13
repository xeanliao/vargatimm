import axios from 'axios'
import React from 'react'
import Topic from 'postal'
import ClassNames from 'classnames'

export default class Auditors extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            auditors: [],
            selectedAuditorId: '',
            notAssignBags: [],
            filterNotAssignBag: '',
            selectedNotAssignBag: null,
            assignedBags: [],
            filterAssignedBag: '',
            selectedAssignedBag: null,
        }

        this.onAuditorChange = this.onAuditorChange.bind(this)
        this.onFilterNotAssignBag = this.onFilterNotAssignBag.bind(this)
        this.onFilterAssignedBag = this.onFilterAssignedBag.bind(this)
        this.onNotAssignBagClick = this.onNotAssignBagClick.bind(this)
        this.onAssignedBagClick = this.onAssignedBagClick.bind(this)
        this.onAssign = this.onAssign.bind(this)
        this.onUnassign = this.onUnassign.bind(this)
        this.onSave = this.onSave.bind(this)
        this.onReset = this.onReset.bind(this)
        this.onClose = this.onClose.bind(this)
    }
    componentDidMount() {
        axios.get(`/user/list/all`).then((resp) => {
            this.setState({ auditors: resp.data })
        })
        this.loadNotAssignBag()
    }
    loadNotAssignBag() {
        axios.get('/bag/user/none').then((resp) => {
            this.setState({ notAssignBags: resp.data })
        })
    }
    onAuditorChange(evt) {
        let auditorId = evt.currentTarget.value
        if (auditorId) {
            axios.get(`/bag/user/${auditorId}`).then((resp) => {
                this.setState({ selectedAuditorId: auditorId, assignedBags: resp.data, selectedAssignedBag: null })
            })
        } else {
            this.setState({ selectedAuditorId: '', assignedBags: [], selectedAssignedBag: null })
        }
    }
    onFilterNotAssignBag(evt) {
        this.setState({ filterNotAssignBag: evt.currentTarget.value })
    }
    onFilterAssignedBag(evt) {
        this.setState({ filterAssignedBag: evt.currentTarget.value })
    }
    onNotAssignBagClick(bagId) {
        if (!this.state.selectedAuditorId) {
            return false
        }
        this.setState({ selectedNotAssignBag: bagId == this.state.selectedNotAssignBag ? null : bagId })
    }
    onAssignedBagClick(bagId) {
        if (!this.state.selectedAuditorId) {
            return false
        }
        this.setState({ selectedAssignedBag: bagId == this.state.selectedAssignedBag ? null : bagId })
    }
    onAssign() {
        if (this.state.selectedNotAssignBag) {
            let selectedBag = this.state.notAssignBags.filter((i) => i == this.state.selectedNotAssignBag)?.[0]
            let notAssignBag = this.state.notAssignBags.filter((i) => i != this.state.selectedNotAssignBag)
            let assignedBag = this.state.assignedBags
            assignedBag.push(selectedBag)
            this.setState({ selectedNotAssignBag: null, notAssignBags: notAssignBag, assignedBags: assignedBag })
        }
    }
    onUnassign() {
        if (this.state.selectedAssignedBag) {
            let selectedBag = this.state.assignedBags.filter((i) => i == this.state.selectedAssignedBag)?.[0]
            let assignedBag = this.state.assignedBags.filter((i) => i != this.state.selectedAssignedBag)
            let notAssignBag = this.state.notAssignBags
            notAssignBag.push(selectedBag)
            this.setState({ selectedAssignedBag: null, notAssignBags: notAssignBag, assignedBags: assignedBag })
        }
    }
    onSave() {
        if (!this.state.selectedAuditorId) {
            return false
        }
        axios.post(`/bag/user/${this.state.selectedAuditorId}`, this.state.assignedBags).then(() => {
            this.onReset()
        })
    }
    onReset() {
        Promise.all([axios.get('/bag/user/none'), axios.get(`/bag/user/${this.state.selectedAuditorId}`)]).then((resp) => {
            this.setState({ selectedNotAssignBag: null, notAssignBags: resp[0].data, selectedAssignedBag: null, assignedBags: resp[1].data })
        })
    }
    onClose() {
        Topic.publish({
            channel: 'View',
            topic: 'showDialog',
            data: null,
        })
    }
    render() {
        let notAssignBags = this.state.notAssignBags.filter((bag) => {
            if (this.state.filterNotAssignBag) {
                return bag.toString().indexOf(this.state.filterNotAssignBag) > -1
            }
            return true
        })
        let assignedBags = this.state.assignedBags.filter((bag) => {
            if (this.state.filterAssignedBag) {
                return bag.toString().indexOf(this.state.filterAssignedBag) > -1
            }
            return true
        })
        return (
            <div>
                <h4 className="text-center">Assign Bag To Auditor</h4>
                <button className="close-button" type="button" onClick={this.onClose}>
                    <span>&times;</span>
                </button>
                <div className="grid-x grid-padding-x">
                    <div className="cell small-12">
                        <select onChange={this.onAuditorChange} value={this.state.selectedAuditorId}>
                            <option>Please select Auditor</option>
                            {this.state.auditors.map((i) => {
                                return (
                                    <option key={i.Id} value={i.Id}>
                                        {i.FullName}
                                    </option>
                                )
                            })}
                        </select>
                    </div>
                    <div className="cell small-5">
                        <input type="search" placeholder="filter Bag" value={this.state.filterNotAssignBag} onChange={this.onFilterNotAssignBag}></input>
                    </div>
                    <div className="cell small-2"></div>
                    <div className="cell small-5">
                        <input type="search" placeholder="filter Bag" value={this.state.filterAssignedBag} onChange={this.onFilterAssignedBag}></input>
                    </div>
                    <div className="cell small-5">
                        <ul className="vertical menu overflow-y-scroll" style={{ height: '480px' }}>
                            {notAssignBags.map((bag) => {
                                let className = ClassNames({ 'is-active': bag == this.state.selectedNotAssignBag })
                                return (
                                    <li
                                        className={className}
                                        key={bag}
                                        onClick={() => {
                                            this.onNotAssignBagClick(bag)
                                        }}
                                    >
                                        <a>{bag}</a>
                                    </li>
                                )
                            })}
                        </ul>
                    </div>
                    <div className="cell small-2 flex-container flex-dir-column align-middle align-center">
                        <button className="button" onClick={this.onAssign} disabled={!this.state.selectedNotAssignBag}>
                            {'>'}
                        </button>
                        <button className="button" onClick={this.onUnassign} disabled={!this.state.selectedAssignedBag}>
                            {'<'}
                        </button>
                    </div>
                    <div className="cell small-5">
                        <ul className="vertical menu overflow-y-scroll" style={{ height: '480px' }}>
                            {assignedBags.map((bag) => {
                                let className = ClassNames({ 'is-active': bag == this.state.selectedAssignedBag })
                                return (
                                    <li
                                        className={className}
                                        key={bag}
                                        onClick={() => {
                                            this.onAssignedBagClick(bag)
                                        }}
                                    >
                                        <a>{bag}</a>
                                    </li>
                                )
                            })}
                        </ul>
                    </div>
                    <div className="cell small-12 flex-container align-center align-middle padding-top-2">
                        <button className="button margin-right-1" disabled={!this.state.selectedAuditorId} onClick={this.onSave}>
                            Save
                        </button>
                        <button className="button secondary margin-left-1" disabled={!this.state.selectedAuditorId} onClick={this.onReset}>
                            Reset
                        </button>
                    </div>
                </div>
            </div>
        )
    }
}
