"use strict";

var AuthStore = require("../stores/AuthStore");
var RouteStore = require("fluxible-router").RouteStore;
var navigateAction = require('fluxible-router').navigateAction;

var debug = require("debug")("deleteTicket");

module.exports = function (context, payload, done) {

    var authStore = context.getStore(AuthStore);
    var userName = authStore.getCurrentUser();

    context.ticketsApi.deleteTicket(userName, payload.expectedVersion, payload.ticketId, function (err, res) {
        if (err) {
            debug('error', err);
            context.dispatch("event:DeleteTicketFailure", err);
            done();
            return;
        }
        
        if (!res.ok) {
            debug('error', res);
            context.dispatch("event:DeleteTicketFailure", res.text);
            done();
            return;
        }
        
        debug('success');
        context.dispatch("event:DeleteTicketSuccess", payload.ticketId);
        done();
    });
};