﻿"use strict";

var nothing = function (context, payload, done) {
    done();
};

module.exports = {
    index: {
        path: "/",
        method: "GET",
        action: nothing,
        handler: require('../components/IndexView')
    },
    documents: {
        path: "/documents",
        method: "GET",
        action: nothing,
        handler: require('../components/DocumentsView')
    },
    newDocument: {
        path: "/documents/new",
        method: "GET",
        action: nothing,
        handler: require('../components/NewDocumentView')
    },
    editDocument: {
        path: "/documents/:id",
        method: "GET",
        action: nothing,
        handler: require('../components/EditDocumentView')
    },
    tickets: {
        path: "/tickets",
        method: "GET",
        action: nothing,
        handler: require('../components/TicketsView')
    },
    newTicket: {
        path: "/tickets/new",
        method: "GET",
        action: nothing,
        handler: require('../components/NewTicketView')
    },
    editTicket: {
        path: "/tickets/:id",
        method: "GET",
        action: nothing,
        handler: require('../components/EditTicketView')
    },
    error: {
        path: "/error",
        method: "GET",
        action: nothing,
        handler: require('../components/ErrorView')
    }
};