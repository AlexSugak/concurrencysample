/** @jsx React.DOM */

var React = require("react");
var debug = require("debug")("IndexView");

var navigateAction = require('fluxible-router').navigateAction;

var connectToStores = require('fluxible-addons-react').connectToStores;

var IndexView = React.createClass({

	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },

	render: function render() {
		return (
			<div>
			Hello
		    </div>
		);
	}
});

IndexView = connectToStores(IndexView, [], function(context, props) {
	return {
	}
});

module.exports = IndexView;