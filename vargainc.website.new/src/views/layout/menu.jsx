import React from 'react'
import BaseView from 'views/base'
import $ from 'jquery'
import classNames from 'classnames'

export var MenuItem = React.createClass({
    mixins: [BaseView],
    getInitialState: function () {
        return {
            active: false,
        }
    },
    getDefaultProps: function () {
        return {
            address: null,
            icon: null,
            name: null,
        }
    },
    render: function () {
        return (
            <li>
                <a className={this.state.active ? 'active' : ''} href={'#' + this.props.address}>
                    <i className={this.props.icon}></i>
                    <span className="menu-text">&nbsp;{this.props.name}</span>
                </a>
            </li>
        )
    },
})

export default React.createClass({
    mixins: [BaseView],
    getInitialState: function () {
        return {
            open: false,
        }
    },
    getDefaultProps: function () {
        return {
            menu: [
                {
                    key: 'campaign',
                    icon: 'fa fa-trophy',
                    address: 'campaign',
                    name: 'Campaign',
                },
                {
                    key: 'distribution',
                    icon: 'fa fa-map',
                    address: 'distribution',
                    name: 'Distribution Maps',
                },
                {
                    key: 'monitor',
                    icon: 'fa fa-truck',
                    address: 'monitor',
                    name: 'GPS Monitor',
                },
                {
                    key: 'report',
                    icon: 'fa fa-file-pdf-o',
                    address: 'report',
                    name: 'Reports',
                },
            ],
        }
    },
    closeMenu: function () {
        this.setState({
            open: false,
        })
    },
    switch: function () {
        this.setState({
            open: !this.state.open,
        })
    },
    render: function () {
        let menuClass = classNames({
            sidebar: true,
            'off-canvas': true,
            'position-left': true,
            'is-open': this.state.open,
        })
        return (
            <div className={menuClass} data-off-canvas data-position="left">
                <ul className="menu vertical" onClick={this.closeMenu}>
                    {this.props.menu.map(function (item) {
                        return <MenuItem key={item.key} {...item} />
                    })}
                    <li>
                        <a href="#admin">
                            <i className="fa fa-gear"></i>
                            <span className="menu-text">&nbsp; Administration</span>
                        </a>
                        <ul className="submenu menu vertical">
                            <li>
                                <a href="#frame/Users.aspx">
                                    <span className="menu-text">User Management</span>
                                </a>
                            </li>
                            <li>
                                <a href="#frame/NonDeliverables.aspx">
                                    <span className="menu-text">Non-Deliverables</span>
                                </a>
                            </li>
                            <li>
                                <a href="#frame/GtuAdmin.aspx?AssignNameToGTUFlag=true">
                                    <span className="menu-text">GTU Management</span>
                                </a>
                            </li>
                            <li>
                                <a href="#frame/AvailableGTUList.aspx">
                                    <span className="menu-text">GTU Available List</span>
                                </a>
                            </li>
                            <li>
                                <a href="#frame/AdminGtuToBag.aspx">
                                    <span className="menu-text">GTU bag Management </span>
                                </a>
                            </li>
                            <li>
                                <a href="#frame/AdminGtuBagToAuditor.aspx">
                                    <span className="menu-text">Assign GTU-Bags to Auditors</span>
                                </a>
                            </li>
                            <li>
                                <a href="#frame/AdminDistributorCompany.aspx">
                                    <span className="menu-text">Distributor Management</span>
                                </a>
                            </li>
                        </ul>
                    </li>
                </ul>
            </div>
        )
    },
})
