"use strict";

var AuthStore = require("../stores/AuthStore");
var RouteStore = require("fluxible-router").RouteStore;
var navigateAction = require('fluxible-router').navigateAction;

var debug = require("debug")("editTicket");

module.exports = function (context, payload, done) {

    var authStore = context.getStore(AuthStore);
    var userName = authStore.getCurrentUser();
    
    var ticketData = { 
        title: payload.title, 
        description: payload.description,
        severity: payload.severity,
        status: payload.status,
        assignedTo: payload.assignedTo
    };

    context.ticketsApi.editTicket(userName, payload.expectedVersion, payload.id, ticketData, function (err, res) {
        if(res.status === 412){
            debug('concurrency error', res);

            context.ticketsApi.getTicketById(userName, payload.id, function(errGet, resGet){
                var err = {message: "Ticket was edited by another user. Please resolve conflicts if any.", serverTicket: resGet.body};
                context.dispatch("event:EditTicketConcurrencyError", err);
                done();
                return;    
            });

            return;
        }

        if (err) {
            debug('error', err);
            context.dispatch("event:EditTicketFailure", err);
            done();
            return;
        }
        
        if (!res.ok) {
            debug('error', res);
            context.dispatch("event:EditTicketFailure", res.text);
            done();
            return;
        }
        
        debug('success');
        var ticket = res.body;
        context.dispatch("event:EditTicketSuccess", ticket);
        
        var routeStore = context.getStore(RouteStore);
        context.executeAction(
            navigateAction, 
            { url: routeStore.makePath("tickets", {}) }, 
            done);
    });
};