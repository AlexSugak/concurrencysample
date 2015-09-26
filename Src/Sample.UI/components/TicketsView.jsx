/** @jsx React.DOM */

var React = require("react");
var debug = require("debug")("TicketsView");

var navigateAction = require('fluxible-router').navigateAction;
var connectToStores = require('fluxible-addons-react').connectToStores;

var TicketsView = React.createClass({

	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },

	render: function render() {
		return (
			<div>
			Tickets
		    </div>
		);
	}
});

TicketsView = connectToStores(TicketsView, [], function(context, props) {
	return {
	}
});

module.exports = TicketsView;