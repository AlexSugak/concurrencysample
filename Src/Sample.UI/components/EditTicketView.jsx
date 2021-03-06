/** @jsx React.DOM */

var React = require("react");
var Formsy = require('formsy-react');
var FormInput = require('./FormInput');
var ErrorMessage = require("./ErrorMessage");

var TicketsStore = require('../stores/TicketsStore');
var ConflictStore = require('../stores/TicketConflictStore');
var RouteStore = require("fluxible-router").RouteStore;

var connectToStores = require('fluxible-addons-react').connectToStores;
var editTicket = require('../actions/editTicket');
var resolveConflict = require('../actions/resolveTicketConflict');

var debug = require("debug")("EditTicketView");

var FormInputWithConflict = React.createClass({
	mixins: [Formsy.Mixin],

	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },

	changeValue: function (event) {
		this.setValue(event.currentTarget.value);
	},
	applyServerVersionClicked: function(event){
		this.setValue(this.props.serverValue);
		this.context.executeAction(resolveConflict, { field: this.props.name, newValue: this.props.serverValue, ticketId: this.props.ticketId});		
	},
	applyLocalVersionClicked: function(event){
		this.context.executeAction(resolveConflict, { field: this.props.name, newValue: this.getValue(), ticketId: this.props.ticketId});		
	},

	renderInput: function(){
		return (
			<input type="text" className="form-control" onChange={this.changeValue} value={this.getValue()}/>
		);
	},
	hasConflict: function(){
		return this.props.hasConflict && this.props.serverValue && this.props.serverValue !== this.getValue();
	},
	renderServerVersion: function() {
		if(this.hasConflict()) {
			return (
				<p className="form-control-static">
					{"Server version is:"} <mark>{this.props.serverValue}</mark>
					<span 
						onClick={this.applyServerVersionClicked} 
						className="icon-action glyphicon glyphicon-ok" 
						aria-hidden="true" />
					<span 
						onClick={this.applyLocalVersionClicked} 
						className="icon-action glyphicon glyphicon-remove" 
						aria-hidden="true" />
				</p> 
			);
		} else {
			return null;
		}
	},
	render: function () {
		var className = (this.showError() || this.hasConflict()) ? 'form-group has-error' : 'form-group';

		var errorMessage = this.getErrorMessage();
		return (
			<div className={className}>
				{this.renderInput()}
				{this.renderServerVersion()}
				<span>{errorMessage}</span>
			</div>
		);
	}
});

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
    	var expectedVersion = this.props.serverTicketVersion 
    							? this.props.serverTicketVersion.eTag 
    							: this.props.ticket.eTag; 
		var data = { 
			id: this.props.ticket.id,
			title: model.title, 
	        description: model.description,
	        severity: model.severity,
	        status: model.status,
	        assignedTo: model.assignedTo,
	        expectedVersion: expectedVersion
		};

		this.context.executeAction(editTicket, data);

		event.preventDefault();
    },
    getServerVersion: function(key){
    	debug("server version:", this.props.serverTicketVersion);
    	if(this.props.serverTicketVersion && this.props.serverTicketVersion[key]){
    		return this.props.serverTicketVersion[key];
    	}

    	return null;
    },

	render: function render() {
		var ticket = this.props.ticket;
		if(!ticket){
			return null;
		}

		var hasConflict = this.props.hasConflict;

		return (
			<div className="row">
				<div className="col-md-6">
					<ErrorMessage loadDataOnClose={false}/>
					<h3>Edit Ticket</h3>

					<Formsy.Form onValidSubmit={this.submit} onValid={this.enableButton} onInvalid={this.disableButton}>
						<label htmlFor="title">Title</label>
						<FormInputWithConflict ticketId={ticket.id} hasConflict={hasConflict} name="title" validationError="Title required" value={ticket.title} serverValue={this.getServerVersion("title")} required/>

						<label htmlFor="description">Description</label>
						<FormInputWithConflict ticketId={ticket.id} hasConflict={hasConflict} name="description" value={ticket.description} serverValue={this.getServerVersion("description")} />

						<label htmlFor="severity">Severity</label>
						<FormInputWithConflict ticketId={ticket.id} hasConflict={hasConflict} name="severity" validationError="Severity required" value={ticket.severity} serverValue={this.getServerVersion("severity")} required/>

						<label htmlFor="status">Status</label>
						<FormInputWithConflict ticketId={ticket.id} hasConflict={hasConflict} name="status" validationError="Status required" value={ticket.status} serverValue={this.getServerVersion("status")} required/>

						<label htmlFor="assignedTo">Assigned To</label>
						<FormInputWithConflict ticketId={ticket.id} hasConflict={hasConflict} name="assignedTo" validationError="Assigned to required" value={ticket.assignedTo} serverValue={this.getServerVersion("assignedTo")} />

						<button className="btn btn-primary" type="submit" disabled={!this.state.canSubmit || this.props.hasConflict}>Save</button>
					</Formsy.Form>
				</div>
		    </div>
		);
	}
});

EditTicketView = connectToStores(EditTicketView, [TicketsStore, ConflictStore, RouteStore], function(context, props) {
	var ticketsStore = context.getStore(TicketsStore);
	var conflictStore = context.getStore(ConflictStore);
	var routeStore = context.getStore(RouteStore);
	var ticketId = routeStore.getCurrentRoute().get('params').get('id');

	return {
		ticket: ticketsStore.getTicket(ticketId),
		hasConflict: conflictStore.hasConflict(ticketId),
		serverTicketVersion: conflictStore.getTicketServerVersion(ticketId)
	}
});

module.exports = EditTicketView;