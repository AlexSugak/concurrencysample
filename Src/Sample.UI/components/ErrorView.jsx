/** @jsx React.DOM */

var React = require("react");

var NavLink = require('fluxible-router').NavLink;

var connectToStores = require('fluxible-addons-react').connectToStores;

var ErrorView = React.createClass({
	render: function render() {
		return (
			<div className="message">
				<p> Oops, something wrong has happened! </p>
				<NavLink routeName="index">
				<div className="back"> Back to home </div>
				</NavLink>
			</div>
		);
	}
});

ErrorView = connectToStores(ErrorView, [], function(context, props) {
	return {
	}
});

module.exports = ErrorView;