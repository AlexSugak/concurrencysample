/** @jsx React.DOM */

var React = require("react");
var ErrorMessagesStore = require("../stores/ErrorMessagesStore");
var loadData = require("../actions/loadData");
var clearError = require("../actions/clearError");

var connectToStores = require('fluxible-addons-react').connectToStores;

var ErrorMessage = React.createClass({
	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },

	removeErrorClicked: function(event){
		if(this.props.loadDataOnClose){
			this.context.executeAction(loadData, {});
		}

		this.context.executeAction(clearError, {});

		event.preventDefault();
	},

	render: function () {
		if(this.props.errorMessage) {
			return (
				<div className="alert alert-danger" role="alert">
					<strong>Error!</strong>{' ' + this.props.errorMessage}
					<span 
						onClick={this.removeErrorClicked} 
						className="icon-action glyphicon glyphicon-remove pull-right" 
						aria-hidden="true" />
				</div>
			);
		}

		return null;
	}
});

ErrorMessage = connectToStores(ErrorMessage, [ErrorMessagesStore], function(context, props) {
	return {
		errorMessage: context.getStore(ErrorMessagesStore).getErrorMessage()
	}
});

module.exports = ErrorMessage;