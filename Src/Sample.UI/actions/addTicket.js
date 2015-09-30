"use strict";

var AuthStore = require("../stores/AuthStore");
var RouteStore = require("fluxible-router").RouteStore;
var navigateAction = require('fluxible-router').navigateAction;

var debug = require("debug")("addTicket");

module.exports = function(context, payload, done) {

    var authStore = context.getStore(AuthStore);
    var userName = authStore.getCurrentUser();

    context.ticketsApi.addTicket(userName, payload, function(err, res) {
        if (err) {
            debug('error', err);
            context.dispatch("event:AddTicketFailure", err);
            done();
            return;
        }

        if (!res.ok) {
            debug('error', res);
            context.dispatch("event:AddTicketFailure", res.text);
            done();
            return;
        }

        debug('success');
        var ticket = res.body;
        context.dispatch("event:AddTicketSuccess", ticket);

        var routeStore = context.getStore(RouteStore);
        context.executeAction(
            navigateAction, {
                url: routeStore.makePath("tickets", {})
            },
            done);
    });
};