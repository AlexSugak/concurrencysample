"use strict";

var createStore = require("fluxible/addons/createStore");
var debug = require("debug")("TicketConflictStore");

var TicketConflictStore = createStore({
    storeName: "TicketConflictStore",
    
    handlers: {
        "event:EditTicketSuccess": "whenTicketEdited",
        "event:EditTicketConcurrencyError": "whenConcurrentEditError",
        "event:TicketConflictResolved": "whenConflictResolved"
    },
    whenTicketEdited: function (ticket) {
        debug("ticket edited");
        this.conflicts[ticket.id] = null;
        this.emitChange();
    },
    whenConcurrentEditError: function (err) {
        debug("concurrent ticket edit detected, server version:", err.serverTicket);
        this.conflicts[err.ticket.id] = { 
            serverTicket: err.serverTicket, 
            conflicts: this.getConflicts(err.ticket, err.serverTicket) 
        };
        this.emitChange();
    },
    whenConflictResolved: function (e) {
        debug("conflict resolved:", e);
        this.conflicts[e.ticketId].conflicts = this.conflicts[e.ticketId].conflicts.filter(function (c) { return c.field !== e.field; });
        this.emitChange();
    },
    getConflicts: function(localTicket, serverTicket){
        var conflicts = [];
        if(localTicket.title !== serverTicket.title){
            conflicts.push({field: 'title', server: serverTicket.title});
        }
        if(localTicket.description !== serverTicket.description){
            conflicts.push({field: 'description', server: serverTicket.description});
        }
        if(localTicket.assignedTo !== serverTicket.assignedTo){
            conflicts.push({field: 'assignedTo', server: serverTicket.assignedTo});
        }
        if(localTicket.severity !== serverTicket.severity){
            conflicts.push({field: 'severity', server: serverTicket.severity});
        }
        if(localTicket.status !== serverTicket.status){
            conflicts.push({field: 'status', server: serverTicket.status});
        }

        return conflicts;
    },
    initialize: function () {
        this.conflicts = [];
    },
    dehydrate: function () {
        return {
            conflicts: this.conflicts
        };
    },
    rehydrate: function (state) {
        this.conflicts = state.conflicts;
    },

    // --- queries ---

    hasConflict: function (ticketId) {
        var isConflict = (this.conflicts[ticketId] && this.conflicts[ticketId].conflicts.length > 0);
        debug("returning if ticket has conflicts:", isConflict);
        return isConflict
    },
    getTicketServerVersion: function (ticketId) {
        if(!this.conflicts[ticketId]) {
            debug("ticket has no conflivt:", ticketId);
            return null;
        }
        var serverVersion = this.conflicts[ticketId].serverTicket;
        debug("returning server ticket:", serverVersion);
        return serverVersion;
    }
});

module.exports = TicketConflictStore;