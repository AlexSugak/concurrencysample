/** @jsx React.DOM */

var React = require("react");
var Formsy = require('formsy-react');
var FormInput = require('./FormInput');

var TicketsStore = require('../stores/TicketsStore');
var RouteStore = require("fluxible-router").RouteStore;

var connectToStores = require('fluxible-addons-react').connectToStores;
var editTicket = require('../actions/editTicket');

var debug = require("debug")("EditTicketView");

var EditTicketView = React.createClass({

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
			id: this.props.ticket.id,
			title: model.title, 
	        description: model.description,
	        severity: model.severity,
	        status: model.status,
	        assignedTo: model.assignedTo,
	        expectedVersion: this.props.ticket.eTag
		};

		this.context.executeAction(editTicket, data);

		event.preventDefault();
    },

	render: function render() {
		return (
			<div className="row">
				<div className="col-md-6">
					<h3>Edit Ticket</h3>

					<Formsy.Form onValidSubmit={this.submit} onValid={this.enableButton} onInvalid={this.disableButton}>
						<label htmlFor="title">Title</label>
						<FormInput name="title" validationError="Title required" value={this.props.ticket.title} required/>
						<label htmlFor="description">Description</label>
						<FormInput name="description" inputType="textarea" value={this.props.ticket.description} />
						<label htmlFor="severity">Severity</label>
						<FormInput name="severity" validationError="Severity required" value={this.props.ticket.severity} required/>
						<label htmlFor="status">Status</label>
						<FormInput name="status" validationError="Status required" value={this.props.ticket.status} required/>
						<label htmlFor="assignedTo">Assigned To</label>
						<FormInput name="assignedTo" validationError="Assigned to required" value={this.props.ticket.assignedTo}/>
						<button className="btn btn-primary" type="submit" disabled={!this.state.canSubmit}>Save</button>
					</Formsy.Form>
				</div>
		    </div>
		);
	}
});

EditTicketView = connectToStores(EditTicketView, [TicketsStore, RouteStore], function(context, props) {
	var ticketsStore = context.getStore(TicketsStore);
	var routeStore = context.getStore(RouteStore);
	var ticketId = routeStore.getCurrentRoute().get('params').get('id');

	return {
		ticket: ticketsStore.getTicket(ticketId)
	}
});

module.exports = EditTicketView;