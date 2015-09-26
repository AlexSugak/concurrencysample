/** @jsx React.DOM */

var React = require("react");

var StoreMixin = require("fluxible").StoreMixin;
var handleHistory = require('fluxible-router').handleHistory;
var provideContext = require('fluxible-addons-react').provideContext;
var debug = require("debug")("app");

var AppPage = React.createClass({
	displayName : "AppPage",

	render: function render() {
		var Handler = this.props.currentRoute.get('handler');

		return (
			<div className="container">
				<div>
					<Handler />
				</div>
			</div>
		);
	}
});

// wrap with history handler
AppPage = handleHistory(AppPage);

// and wrap that with context
AppPage = provideContext(AppPage);

module.exports = AppPage;