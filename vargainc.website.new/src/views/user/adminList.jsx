import Backbone from 'backbone'
import React from 'react'
import 'react.backbone'
import BaseView from 'views/base'
import Collection from 'collections/user'

var adminCollection = new Collection()

export default React.createBackboneClass({
    mixins: [BaseView],
    getDefaultProps: function () {
        return {
            collection: adminCollection,
            group: null,
            onSelect: null,
        }
    },
    getInitialState: function () {
        return {
            selected: null,
        }
    },
    componentWillMount: function () {
        if (this.props.group) {
            this.getCollection().fetchInGroup(this.props.group)
        }
    },
    onSelected: function (userId) {
        var user = this.getCollection().get(userId)
        this.props.onSelect && this.props.onSelect(user)
        this.setState({
            selected: user,
        })
    },
    onDbSelected: function (userId) {
        var user = this.getCollection().get(userId)
        this.props.onDbSelect && this.props.onDbSelect(user)
        this.setState({
            selected: user,
        })
    },
    render: function () {
        var list = this.getCollection(),
            self = this

        list = list ? list : []
        return (
            <ul className="vertical menu list-group">
                {list.map(function (model) {
                    //var active = this.state && this.state.selectedUserId && this.state.selectedUserId == model.get('Id') ? "active" : "";
                    var id = model.get('Id'),
                        selected = self.state.selected && self.state.selected == model,
                        activeClass = selected ? 'list-group-item active' : 'list-group-item'

                    return (
                        <li className={activeClass} key={id} onClick={self.onSelected.bind(self, id)} onClick={self.onDbSelected.bind(self, id)}>
                            {model.get('UserName')}
                        </li>
                    )
                })}
            </ul>
        )
    },
})
