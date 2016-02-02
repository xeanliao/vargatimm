define(['moment', 'backbone', 'react', 'pubsub', 'collections/user', 'react.backbone'], function (moment, Backbone, React, Topic, Collection) {
	var adminCollection = new Collection();

	return React.createBackboneClass({
		getDefaultProps: function () {
			return {
				collection: adminCollection
			};
		},
		componentWillMount: function () {
			this.getCollection().fetchInGroup();
		},
		onSelected: function (e) {
			console.log('user selection on selected');
			var collection = this.getCollection();
			var user = this.getModel().attributes;
			this.props && this.props.onSelect && this.props.onSelect(user);
			this.setState({ selected: user });
		},
		render: function () {
			var list = this.getCollection();
			list = list ? list : [];
			return React.createElement(
				'ul',
				{ className: 'vertical menu list-group' },
				list.map(function (model) {
					//var active = this.state && this.state.selectedUserId && this.state.selectedUserId == model.get('Id') ? "active" : "";
					return React.createElement(
						'li',
						{ className: 'list-group-item', key: model.get('Id'), onClick: this.onSelected },
						model.get('UserName')
					);
				})
			);
		}
	});
});
