/** @jsx React.DOM */

var React = require("react");

var NavLink = require('fluxible-router').NavLink;

var connectToStores = require('fluxible-addons-react').connectToStores;

var ErrorView = React.createClass({
	render: function render() {
		return (
			<div className="alert alert-danger" role="alert">
				<strong>Something went wrong!</strong>
			</div>
		);
	}
});

ErrorView = connectToStores(ErrorView, [], function(context, props) {
	return {
	}
});

module.exports = ErrorView;