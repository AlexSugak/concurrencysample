"use strict";

var createStore = require("fluxible/addons/createStore");
var debug = require("debug")("TicketsStore");

var TicketsStore = createStore({
    storeName: "TicketsStore",
    
    handlers: {
        "event:FetchAllTicketsSuccess": "whenTicketsFetched",
        "event:AddTicketSuccess": "whenTicketAdded",
        "event:DeleteTicketSuccess": "whenTicketDeleted",
        "event:EditTicketSuccess": "whenTicketEdited",
        "event:EditTicketConcurrencyError": "whenConcurrentEditError"
    },
    whenTicketsFetched: function (tickets) {
        debug("tickets fetched");
        this.tickets = tickets;
        this.serverTickets = [];
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
        for (var i = 0; i < this.tickets.length; i++) {
            if (this.tickets[i].id === ticket.id) {
                this.tickets[i] = ticket;
                break;
            }
        }
        this.serverTickets = this.serverTickets.filter(function (t) { return t.id !== ticket.id; });
        this.emitChange();
    },
    whenConcurrentEditError: function (err) {
        debug("concurrent ticket edit detected, server version:", err.serverTicket);
        this.serverTickets = this.serverTickets.filter(function (t) { return t.id !== err.serverTicket.id; });
        this.serverTickets.push(err.serverTicket);
        this.emitChange();
    },
    initialize: function () {
        this.tickets = [];
        this.serverTickets = [];
    },
    dehydrate: function () {
        return {
            tickets: this.tickets,
            serverTickets: this.serverTickets
        };
    },
    rehydrate: function (state) {
        this.tickets = state.tickets;
        this.serverTickets = state.serverTickets;
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
    },
    getTicketServerVersion: function (ticketId) {
        for (var i = 0; i < this.serverTickets.length; i++) {
            if (this.serverTickets[i].id === ticketId) {
                debug("returning server ticket:", this.serverTickets[i]);
                return this.serverTickets[i];
            }
        }

        debug("server ticket version not found:", ticketId);
        return null;
    }
});

module.exports = TicketsStore;