/** @jsx React.DOM */

var React = require("react");
var debug = require("debug")("DocumentsView");

var navigateAction = require('fluxible-router').navigateAction;
var connectToStores = require('fluxible-addons-react').connectToStores;

var DocumentsView = React.createClass({

	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },

	render: function render() {
		return (
			<div>
			Documents
		    </div>
		);
	}
});

DocumentsView = connectToStores(DocumentsView, [], function(context, props) {
	return {
	}
});

module.exports = DocumentsView;