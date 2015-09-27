"use strict";

var AuthStore = require("../stores/AuthStore");
var RouteStore = require("fluxible-router").RouteStore;
var navigateAction = require('fluxible-router').navigateAction;

var debug = require("debug")("checkoutDocument");

module.exports = function (context, payload, done) {

    var authStore = context.getStore(AuthStore);
    var userName = authStore.getCurrentUser();

    context.api.checkoutDocument(userName, payload.documentId, function (err, res) {
        if (err) {
            debug('error', err);
            context.dispatch("event:CheckoutDocumentFailure", err);
            done();
            return;
        }
        
        if (!res.ok) {
            debug('error', res);
            context.dispatch("event:CheckoutDocumentFailure", res.text);
            done();
            return;
        }
        
        debug('success');
        var lockInfo = { id : payload.documentId, checkedOutBy: userName };
        context.dispatch("event:CheckoutDocumentSuccess", lockInfo);
        done();
    });
};