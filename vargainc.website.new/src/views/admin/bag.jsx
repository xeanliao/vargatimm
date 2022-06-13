import axios from 'axios'
import React from 'react'
import Topic from 'postal'
import ClassNames from 'classnames'

export default class Bag extends React.Component {
    constructor(props) {
        super(props)

        this.state = {
            bags: [],
            selectedBagId: '',
            filterNotInBag: '',
            selectedNotInBagGtuId: null,
            filterGtuInBag: '',
            selectedGtuInBagGtuId: null,
            notInBag: [],
            gtuInBag: [],
        }

        this.onBagChange = this.onBagChange.bind(this)
        this.loadNotInBagGtu = this.loadNotInBagGtu.bind(this)
        this.onFilterNotInBag = this.onFilterNotInBag.bind(this)
        this.onFilterGtuInBag = this.onFilterGtuInBag.bind(this)
        this.onNotInBagClick = this.onNotInBagClick.bind(this)
        this.onGtuInBagClick = this.onGtuInBagClick.bind(this)
        this.onAssign = this.onAssign.bind(this)
        this.onUnassign = this.onUnassign.bind(this)
        this.onSave = this.onSave.bind(this)
        this.onReset = this.onReset.bind(this)
        this.onClose = this.onClose.bind(this)
    }
    componentDidMount() {
        axios.get(`/bag/all`).then((resp) => {
            this.setState({ bags: resp.data })
        })
        this.loadNotInBagGtu()
    }
    loadNotInBagGtu() {
        axios.get('/gtu/bag/none').then((resp) => {
            this.setState({ notInBag: resp.data })
        })
    }
    onBagChange(evt) {
        let bagId = evt.currentTarget.value
        if (bagId) {
            axios.get(`/gtu/bag/${bagId}`).then((resp) => {
                this.setState({ selectedBagId: bagId, gtuInBag: resp.data, selectedGtuInBagGtuId: null })
            })
        } else {
            this.setState({ selectedBagId: '', gtuInBag: [], selectedGtuInBagGtuId: null })
        }
    }
    onFilterNotInBag(evt) {
        this.setState({ filterNotInBag: evt.currentTarget.value })
    }
    onFilterGtuInBag(evt) {
        this.setState({ filterGtuInBag: evt.currentTarget.value })
    }
    onNotInBagClick(gtuId) {
        if (!this.state.selectedBagId) {
            return false
        }
        this.setState({ selectedNotInBagGtuId: this.state.selectedNotInBagGtuId == gtuId ? null : gtuId })
    }
    onGtuInBagClick(gtuId) {
        if (!this.state.selectedBagId) {
            return false
        }
        this.setState({ selectedGtuInBagGtuId: this.state.selectedGtuInBagGtuId == gtuId ? null : gtuId })
    }
    onAssign() {
        if (this.state.selectedNotInBagGtuId) {
            let selectedGtu = this.state.notInBag.filter((i) => i.Id == this.state.selectedNotInBagGtuId)?.[0]
            let notInBag = this.state.notInBag.filter((i) => i.Id != this.state.selectedNotInBagGtuId)
            let gtuInBag = this.state.gtuInBag
            gtuInBag.push(selectedGtu)
            this.setState({ selectedNotInBagGtuId: null, notInBag: notInBag, gtuInBag: gtuInBag })
        }
    }
    onUnassign() {
        if (this.state.selectedGtuInBagGtuId) {
            let selectedGtu = this.state.gtuInBag.filter((i) => i.Id == this.state.selectedGtuInBagGtuId)?.[0]
            let gtuInBag = this.state.gtuInBag.filter((i) => i.Id != this.state.selectedGtuInBagGtuId)
            let notInBag = this.state.notInBag
            notInBag.push(selectedGtu)
            this.setState({ selectedGtuInBagGtuId: null, notInBag: notInBag, gtuInBag: gtuInBag })
        }
    }
    onSave() {
        if (!this.state.selectedBagId) {
            return false
        }
        axios
            .post(
                `/gtu/bag/${this.state.selectedBagId}`,
                this.state.gtuInBag.map((i) => i.Id)
            )
            .then(() => {
                this.onReset()
            })
    }
    onReset() {
        Promise.all([axios.get('/gtu/bag/none'), axios.get(`/gtu/bag/${this.state.selectedBagId}`)]).then((resp) => {
            this.setState({ selectedNotInBagGtuId: null, notInBag: resp[0].data, selectedGtuInBagGtuId: null, gtuInBag: resp[1].data })
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
        let notInBag = this.state.notInBag.filter((gtu) => {
            if (this.state.filterNotInBag) {
                return gtu.ShortUniqueID.toLowerCase().indexOf(this.state.filterNotInBag.toLowerCase()) > -1
            }
            return true
        })
        let gtuInBag = this.state.gtuInBag.filter((gtu) => {
            if (this.state.filterGtuInBag) {
                return gtu.ShortUniqueID.toLowerCase().indexOf(this.state.filterGtuInBag.toLowerCase()) > -1
            }
            return true
        })
        return (
            <div>
                <h4 className="text-center">Assign GTU To Bag</h4>
                <button className="close-button" type="button" onClick={this.onClose}>
                    <span>&times;</span>
                </button>
                <div className="grid-x grid-padding-x">
                    <div className="cell small-12">
                        <select onChange={this.onBagChange} value={this.state.selectedBagId}>
                            <option>Please select Bag</option>
                            {this.state.bags.map((i) => {
                                return (
                                    <option key={i.Id} value={i.Id}>
                                        {i.Id}
                                    </option>
                                )
                            })}
                        </select>
                    </div>
                    <div className="cell small-5">
                        <input type="search" placeholder="filter GTU" value={this.state.filterNotInBag} onChange={this.onFilterNotInBag}></input>
                    </div>
                    <div className="cell small-2"></div>
                    <div className="cell small-5">
                        <input type="search" placeholder="filter GTU" value={this.state.filterGtuInBag} onChange={this.onFilterGtuInBag}></input>
                    </div>
                    <div className="cell small-5">
                        <ul className="vertical menu overflow-y-scroll" style={{ height: '480px' }}>
                            {notInBag.map((gtu) => {
                                let className = ClassNames({ 'is-active': gtu.Id == this.state.selectedNotInBagGtuId })
                                return (
                                    <li
                                        className={className}
                                        key={gtu.Id}
                                        onClick={() => {
                                            this.onNotInBagClick(gtu.Id)
                                        }}
                                    >
                                        <a>{gtu.ShortUniqueID}</a>
                                    </li>
                                )
                            })}
                        </ul>
                    </div>
                    <div className="cell small-2 flex-container flex-dir-column align-middle align-center">
                        <button className="button" onClick={this.onAssign} disabled={!this.state.selectedBagId}>
                            {'>'}
                        </button>
                        <button className="button" onClick={this.onUnassign} disabled={!this.state.selectedBagId}>
                            {'<'}
                        </button>
                    </div>
                    <div className="cell small-5">
                        <ul className="vertical menu overflow-y-scroll" style={{ height: '480px' }}>
                            {gtuInBag.map((gtu) => {
                                let className = ClassNames({ 'is-active': gtu.Id == this.state.selectedGtuInBagGtuId })
                                return (
                                    <li
                                        className={className}
                                        key={gtu.Id}
                                        onClick={() => {
                                            this.onGtuInBagClick(gtu.Id)
                                        }}
                                    >
                                        <a>{gtu.ShortUniqueID}</a>
                                    </li>
                                )
                            })}
                        </ul>
                    </div>
                    <div className="cell small-12 flex-container align-center align-middle padding-top-2">
                        <button className="button margin-right-1" disabled={!this.state.selectedBagId} onClick={this.onSave}>
                            Save
                        </button>
                        <button className="button secondary margin-left-1" disabled={!this.state.selectedBagId} onClick={this.onReset}>
                            Reset
                        </button>
                    </div>
                </div>
            </div>
        )
    }
}
