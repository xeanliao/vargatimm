import React from 'react';

export default React.createClass({
	getDefaultProps: function () {
		return {
			page: null,
			query: null
		};
	},
	render: function () {
		var address = this.props.page ? this.props.page : "about:blank;";
		if (this.props.page && this.props.query) {
			address += "?" + this.props.query;
		}
		return (
			<iframe src={address}></iframe>
		);
	}
});