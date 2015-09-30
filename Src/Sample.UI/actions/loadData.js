"use strict";

var SyncStore = require("../stores/SyncStore");
var AuthStore = require("../stores/AuthStore");
var debug = require("debug")("loadData");

module.exports = function(context, payload, done) {
    
    var authStore = context.getStore(AuthStore);
    var userName = authStore.getCurrentUser();

    context.documentsApi.getAllDocuments(userName, function (err, res) {
        if (err) {
            debug('error', err);
            context.dispatch("event:FetchAllDocumentsFailure", err);
            done();
            return;
        }
        
        if (!res.ok) {
            debug('error', res);
            context.dispatch("event:FetchAllDocumentsFailure", res.body);
            done();
            return;
        }
        
        var documents = res.body.documents;
        context.dispatch("event:FetchAllDocumentsSuccess", documents);

        context.ticketsApi.getAllTickets(userName, function (err, res) {
            if (err) {
                debug('error', err);
                context.dispatch("event:FetchAllTicketsFailure", err);
                done();
                return;
            }
            
            if (!res.ok) {
                debug('error', res);
                context.dispatch("event:FetchAllTicketsFailure", res.body);
                done();
                return;
            }

            debug('success');
            var tickets = res.body.tickets;
            context.dispatch("event:FetchAllTicketsSuccess", tickets);

            context.dispatch("DATA_LOADED");
            done();
            return;
        });
    });
};