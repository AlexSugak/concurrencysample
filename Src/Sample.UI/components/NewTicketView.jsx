/** @jsx React.DOM */

var React = require("react");
var Formsy = require('formsy-react');
var FormInput = require('./FormInput');

var connectToStores = require('fluxible-addons-react').connectToStores;
var addTicket = require('../actions/addTicket');

var debug = require("debug")("NewTicketView");

var NewTicketView = React.createClass({

	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },

	getInitialState: function() {
		return { canSubmit: false };
	},
	enableButton: function () {
		this.setState({
			canSubmit: true
		});
    },
    disableButton: function () {
		this.setState({
			canSubmit: false
		});
    },
    submit: function (model) {
		var data = { 
			title: model.title, 
	        description: model.description,
	        severity: model.severity,
	        status: model.status,
	        assignedTo: model.assignedTo
		};

		this.context.executeAction(addTicket, data);

		event.preventDefault();
    },

	render: function render() {
		return (
			<div className="row">
				<div className="col-md-6">
					<h3>New Ticket</h3>

					<Formsy.Form onValidSubmit={this.submit} onValid={this.enableButton} onInvalid={this.disableButton}>
						<label htmlFor="title">Title</label>
						<FormInput name="title" validationError="Title required" required/>
						<label htmlFor="description">Description</label>
						<FormInput name="description" inputType="textarea" />
						<label htmlFor="severity">Severity</label>
						<FormInput name="severity" validationError="Severity required" required/>
						<label htmlFor="status">Status</label>
						<FormInput name="status" validationError="Status required" required/>
						<label htmlFor="assignedTo">Assigned To</label>
						<FormInput name="assignedTo" validationError="Assigned to required"/>
						<button className="btn btn-primary" type="submit" disabled={!this.state.canSubmit}>Save</button>
					</Formsy.Form>
				</div>
		    </div>
		);
	}
});

NewTicketView = connectToStores(NewTicketView, [], function(context, props) {
	return {
	}
});

module.exports = NewTicketView;