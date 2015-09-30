/** @jsx React.DOM */

var React = require("react");
var debug = require("debug")("TicketsView");

var TicketsStore = require('../stores/TicketsStore');
var AuthStore = require("../stores/AuthStore");
var NavLink = require("fluxible-router").NavLink;
var ErrorMessage = require("./ErrorMessage");

var deleteTicket = require('../actions/deleteTicket');
var navigateAction = require('fluxible-router').navigateAction;
var connectToStores = require('fluxible-addons-react').connectToStores;


var TicketRow = React.createClass({
	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },
	deleteClicked: function (event) {
		var data = { ticketId: this.props.ticket.id, expectedVersion: this.props.ticket.eTag };

		this.context.executeAction(deleteTicket, data);
		event.preventDefault();
    },
	render: function render() {
		var ticketId = this.props.ticket.id;
		var actions = [];
		actions.push(<NavLink routeName="editTicket" navParams={{id: ticketId}} key={'edit_' + ticketId} className="btn btn-link">
						edit
					 </NavLink>);

		actions.push(<a key={'delete_'+ ticketId} className="btn btn-link" onClick={this.deleteClicked}>delete</a>);

		return (
			<tr>
				<td>{this.props.ticket.title}</td>
				<td>{this.props.ticket.status}</td>
				<td>
					{actions}
				</td>
			</tr>
		);
	}
});

var TicketsView = React.createClass({

	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },

	render: function render() {

		var rows = [];
		for(i = 0; i < this.props.tickets.length; i++){
			var t = this.props.tickets[i];
			rows.push(<TicketRow  ticket={t} key={t.id} />);
		}

		return (
			<div>
				<ErrorMessage />
				<h3>Tickets</h3>
				<div className="row">
					<div className="col-md-6">
						<table className="table table-striped table-hover">
							<thead>
								<tr>
									<th>Title</th>
									<th>Status</th>
									<th></th>
								</tr>
							</thead>
							<tbody>
								{rows}
							</tbody>
						</table>
						<NavLink routeName="newTicket" className="btn btn-default">
							Add new Ticket
						</NavLink>
					</div>
					<div className="col-md-6">
					</div>
				</div>
		    </div>
		);
	}
});

TicketsView = connectToStores(TicketsView, [TicketsStore], function(context, props) {
	return {
		tickets: context.getStore(TicketsStore).getAllTickets()
	}
});

module.exports = TicketsView;