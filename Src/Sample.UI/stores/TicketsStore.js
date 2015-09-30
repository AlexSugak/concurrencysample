"use strict";

var createStore = require("fluxible/addons/createStore");
var debug = require("debug")("TicketsStore");

var TicketsStore = createStore({
    storeName: "TicketsStore",
    
    handlers: {
        "event:FetchAllTicketsSuccess": "whenTicketsFetched",
        "event:AddTicketSuccess": "whenTicketAdded",
        "event:DeleteTicketSuccess": "whenTicketDeleted",
        "event:EditTicketSuccess": "whenTicketEdited"
    },
    whenTicketsFetched: function (tickets) {
        debug("tickets fetched");
        this.tickets = tickets;
        this.emitChange();
    },
    whenTicketAdded: function (ticket) {
        debug("ticket added");
        this.tickets.push(ticket);
        this.emitChange();
    },
    whenTicketDeleted: function (ticketId) {
        debug("ticket deleted");
        this.tickets = this.tickets.filter(function (t) { return t.id !== ticketId; });
        this.emitChange();
    },
    whenTicketEdited: function (ticket) {
        debug("ticket edited");
        for (i = 0; i < this.tickets.length; i++) {
            if (this.tickets[i].id === ticket.id) {
                this.tickets[i] = ticket;
                break;
            }
        }
        this.emitChange();
    },
    initialize: function () {
        this.tickets = [];
    },
    dehydrate: function () {
        return {
            tickets: this.tickets
        };
    },
    rehydrate: function (state) {
        this.tickets = state.tickets;
    },

    // --- queries ---

    getAllTickets: function () {
        debug("returning all tickets", this.tickets);
        return this.tickets;
    },
    getTicket: function (ticketId) {
        for (var i = 0; i < this.tickets.length; i++) {
            if (this.tickets[i].id === ticketId) {
                debug("returning ticket:", this.tickets[i]);
                return this.tickets[i];
            }
        }

        debug("ticket not found:", ticketId);
        return null;
    }
});

module.exports = TicketsStore;