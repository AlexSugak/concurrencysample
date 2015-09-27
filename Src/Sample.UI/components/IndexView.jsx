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
				<h3>Welcome to the concurrency control samples app</h3>
				<p>
					Please use top bar to navigate to Documents or Tickets management pages.
				</p>
				<p>
					Here, <mark>Documents</mark> demostrate <mark>Pessimistic</mark> concurrency control.
					Users are required to <mark>check out</mark> (i.e. lock) the document before making any changes to it.
					If the document is locked by a user, no other user can edit it.
				</p>
				<p>
					<mark>Tickets</mark> demostrate <mark>Optimistic</mark> concurrency control.
					Users can edit any ticket. However, the changes made to ticket will only apply if no other user has made changes not seen by the first user.
					If this is not true, user will have to resolve the <mark>Conflict</mark> by choosing which changes (his or server's) to apply.
				</p>
		    </div>
		);
	}
});

IndexView = connectToStores(IndexView, [], function(context, props) {
	return {
	}
});

module.exports = IndexView;