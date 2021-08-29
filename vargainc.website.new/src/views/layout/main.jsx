import Backbone from 'backbone'
import React from 'react'
import 'react.backbone'
import $ from 'jquery'
import classNames from 'classnames'
import BaseView from 'views/base'
import MenuView from 'views/layout/menu'
import UserView from 'views/layout/user'
import LoadingView from 'views/layout/loading'
import { isString, has, extend } from 'lodash'

export default React.createBackboneClass({
    mixins: [BaseView, React.BackboneMixin('user', 'change:FullName')],
    getInitialState: function () {
        return {
            mainView: null,
            mainParams: null,
            dialogView: null,
            dialogParams: null,
            dialogSize: 'small',
            dialogCustomClass: '',
            dialogModalView: null,
            dialogModalParams: null,
            dialogModalSize: 'small',
            dialogModalCustomClass: '',
            loading: false,
            pageTitle: 'TIMM System',
            showMenu: null,
            showSearch: null,
            showUser: null,
            fullTextSearchTimeout: null,
        }
    },
    componentDidMount: function () {
        var self = this
        /**
         * set main view
         * @param  {React} view
         * @param  {Backbone.Collection} or Backbone.Model} params
         * @param  {showMenu: {boolean}
         */
        this.subscribe('loadView', function (data) {
            // self.setState({})
            var options = data.options
            self.setState({
                pageTitle: has(options, 'pageTitle') ? options.pageTitle : 'TIMM System',
                showMenu: has(options, 'showMenu') ? options.showMenu : true,
                showSearch: has(options, 'showSearch') ? options.showSearch : true,
                showUser: has(options, 'showUser') ? options.showUser : true,
                mainView: data.view,
                mainParams: data.params,
            })
        })
        /**
         * show a dialog
         * @param  {React} view
         * @param  {Backbone.Collection} or Backbone.Model} params
         * @param  {size: {String} size Foundation Reveal Size Value: tiny, small, large, full} options
         */
        this.subscribe('showDialog', function (data) {
            if (data) {
                self.setState({
                    dialogView: data.view,
                    dialogParams: data.params,
                })
                var options = data.options
                self.setState({
                    dialogSize: has(options, 'size') ? options.size : 'small',
                    dialogCustomClass: has(options, 'customClass') ? options.customClass : '',
                })
            } else {
                self.setState({
                    dialogView: null,
                    dialogParams: null,
                    dialogSize: null,
                    dialogCustomClass: null,
                })
            }
        })

        /**
         * show a dialog
         * @param  {React} view
         * @param  {Backbone.Collection} or Backbone.Model} params
         * @param  {size: {String} size Foundation Reveal Size Value: tiny, small, large, full} options
         */
        this.subscribe('showDialogModal', function (data) {
            if (data) {
                self.setState({
                    dialogModalView: data.view,
                    dialogModalParams: data.params,
                })
                var options = data.options
                self.setState({
                    dialogModalSize: has(options, 'size') ? options.size : 'small',
                    dialogModalCustomClass: has(options, 'customClass') ? options.customClass : '',
                })
            } else {
                self.setState({
                    dialogModalView: null,
                    dialogModalParams: null,
                    dialogModalSize: null,
                    dialogModalCustomClass: null,
                })
            }
        })

        /**
         * loading control
         */
        var loadingCount = 0,
            loadingDelay = 500,
            loadingTimeout = null
        this.subscribe('showLoading', function () {
            loadingCount++
            window.clearTimeout(loadingTimeout)
            loadingTimeout = window.setTimeout(function () {
                self.setState({
                    loading: true,
                })
            }, loadingDelay)
        })
        this.subscribe('hideLoading', function () {
            loadingCount--
            window.setTimeout(function () {
                if (loadingCount <= 0) {
                    window.clearTimeout(loadingTimeout)
                    self.setState({
                        loading: false,
                    })
                    loadingCount = 0
                }
            }, 300)
        })
        /**
         * fix main view size
         */
        $(window).on('resize', function () {
            $('.off-canvas-wrapper-inner').height($(window).height())
            self.publish('Global.Window.Resize', {
                width: $(window).width(),
                height: $(window).height(),
            })
        })
        $(window).trigger('resize')

        $(window).on('click', function () {
            self.publish('Global.Window.Click')
        })
    },
    componentDidUpdate: function () {
        var haveOpenModal = false
        if (this.state.dialogView && Foundation) {
            $('.dialog').foundation()
            $('.dialog').foundation('open')
            haveOpenModal = true
        } else {
            $('.dialog').foundation()
            $('.dialog').foundation('close')
        }
        if (this.state.dialogModalView && Foundation) {
            $('.dialogModal').foundation()
            $('.dialogModal').foundation('open')
            haveOpenModal = true
        } else {
            $('.dialogModal').foundation()
            $('.dialogModal').foundation('close')
        }

        if (!haveOpenModal) {
            $('.reveal-overlay').css({ display: 'none' })
        }
        $('html').removeClass('is-reveal-open')
        $('iframe').height($(window).height())
    },
    fullTextSearch: function (e) {
        var self = this
        clearTimeout(this.state.fullTextSearchTimeout)
        var searchKey = e.currentTarget.value
        this.state.fullTextSearchTimeout = setTimeout(function () {
            self.publish({
                channel: 'View',
                topic: 'search',
                data: searchKey,
            })
        }, 500)
    },
    menuSwitch: function () {
        this.refs.sideMenu && this.refs.sideMenu.switch()
    },
    /**
     * build main view
     */
    getMainView: function () {
        if (this.state.mainView) {
            if (React.isValidElement(this.state.mainView)) {
                return this.state.mainView
            } else {
                var MainView = React.createFactory(this.state.mainView)
                return MainView(this.state.mainParams)
            }
        }
        return null
    },
    onCloseDialog: function () {
        this.publish('showDialog')
    },
    onCloseDialogModal: function () {
        this.publish('showDialogModal')
    },
    /**
     * build dialog view
     */
    getDialogView: function (view, viewParams, closeHandle) {
        if (view) {
            if (isString(view)) {
                var content = view
                return (
                    <div className="row">
                        <div className="small-12 columns">
                            <p>&nbsp;</p>
                            <h5>{content}</h5>
                            <p>&nbsp;</p>
                        </div>
                        <div className="small-12 columns">
                            <div className="button-group float-right">
                                <a href="javascript:;" className="button tiny" onClick={closeHandle}>
                                    Okay
                                </a>
                            </div>
                        </div>
                        <button onClick={closeHandle} className="close-button" data-close aria-label="Close reveal" type="button">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                )
            } else if (React.isValidElement(view)) {
                return view
            } else {
                var DialogView = React.createFactory(view),
                    params = extend(viewParams, {
                        ref: 'DialogView',
                    })
                return DialogView(params)
            }
        }
        return null
    },
    render: function () {
        var model = this.getModel()
        var mainView = this.getMainView()
        var dialogView = this.getDialogView(this.state.dialogView, this.state.dialogParams, this.onCloseDialog)
        var dialogClass = classNames(`reveal dialog ${this.state.dialogSize} ${this.state.dialogCustomClass}`, {
            hide: dialogView == null,
        })
        var dialogModalView = this.getDialogView(this.state.dialogModalView, this.state.dialogModalParams, this.onCloseDialogModal)
        var dialogModalClass = classNames(`reveal dialogModal ${this.state.dialogModalSize} ${this.state.dialogModalCustomClass}`, {
            hide: dialogModalView == null,
        })

        if (this.state.showMenu === true) {
            var mainMenuClassName = 'left-menu'
            var menu = <MenuView ref="sideMenu" />
        } else {
            var mainMenuClassName = ''
            var menu = null
        }
        if (this.state.showSearch === true) {
            var search = (
                <span className="title-bar-center">
                    <div className="topSearchBar hide-for-small-only">
                        <input type="text" placeholder="Search" onChange={this.fullTextSearch} />
                    </div>
                </span>
            )
        } else {
            var search = null
        }
        if (this.state.showUser === true) {
            var user = <UserView model={this.props.user} />
        } else {
            var user = null
        }
        var loadingDisplay = this.state.loading ? 'block' : 'none'
        return (
            <div>
                {menu}
                <div className={'main off-canvas-content ' + mainMenuClassName} data-off-canvas-content>
                    <div className="title-bar">
                        <div className="title-bar-left">
                            <button aria-expanded="true" className="menu-icon" type="button" onClick={this.menuSwitch}></button>
                            <span className="title-text">{this.state.pageTitle}</span>
                        </div>
                        {user}
                        {search}
                    </div>
                    {mainView}
                    <div id="google-map-wrapper">
                        <div id="google-map" className="google-map"></div>
                    </div>
                </div>
                <div key="dialog" className={dialogClass} data-reveal data-options="closeOnClick: false; closeOnEsc: false;">
                    {dialogView}
                </div>

                <div key="dialogModal" className={dialogModalClass} data-reveal data-options="closeOnClick: false; closeOnEsc: false;">
                    {dialogModalView}
                </div>

                <div className="overlayer" style={{ display: loadingDisplay }}>
                    <LoadingView />
                </div>
            </div>
        )
    },
})
