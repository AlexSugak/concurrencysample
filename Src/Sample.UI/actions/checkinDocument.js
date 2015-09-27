"use strict";

var AuthStore = require("../stores/AuthStore");
var RouteStore = require("fluxible-router").RouteStore;
var navigateAction = require('fluxible-router').navigateAction;

var debug = require("debug")("checkinDocument");

module.exports = function (context, payload, done) {

    var authStore = context.getStore(AuthStore);
    var userName = authStore.getCurrentUser();

    context.api.checkinDocument(userName, payload.documentId, function (err, res) {
        if (err) {
            debug('error', err);
            context.dispatch("event:CheckinDocumentFailure", err);
            done();
            return;
        }
        
        if (!res.ok) {
            debug('error', res);
            context.dispatch("event:CheckinDocumentFailure", res.text);
            done();
            return;
        }
        
        debug('success');
        context.dispatch("event:CheckinDocumentSuccess", payload.documentId);
        done();
    });
};