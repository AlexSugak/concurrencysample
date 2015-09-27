"use strict";

var AuthStore = require("../stores/AuthStore");
var RouteStore = require("fluxible-router").RouteStore;
var navigateAction = require('fluxible-router').navigateAction;

var debug = require("debug")("addDocument");

module.exports = function (context, payload, done) {

    var authStore = context.getStore(AuthStore);
    var userName = authStore.getCurrentUser();

    context.api.addDocument(userName, payload, function (err, res) {
        if (err) {
            debug('error', err);
            context.dispatch("event:AddDocumentFailure", err);
            done();
            return;
        }
        
        if (!res.ok) {
            debug('error', res);
            context.dispatch("event:AddDocumentFailure", res.text);
            done();
            return;
        }
        
        debug('success');
        var document = res.body;
        context.dispatch("event:AddDocumentSuccess", document);
        
        var routeStore = context.getStore(RouteStore);
        context.executeAction(
            navigateAction, 
            { url: routeStore.makePath("documents", {}) }, 
            done);
    });
};