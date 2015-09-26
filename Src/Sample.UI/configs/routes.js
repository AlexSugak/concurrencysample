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
    tickets: {
        path: "/tickets",
        method: "GET",
        action: nothing,
        handler: require('../components/TicketsView')
    },
    error: {
        path: "/error",
        method: "GET",
        action: nothing,
        handler: require('../components/ErrorView')
    }
};