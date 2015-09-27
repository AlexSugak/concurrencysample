"use strict";

var AuthStore = require("../stores/AuthStore");
var RouteStore = require("fluxible-router").RouteStore;
var navigateAction = require('fluxible-router').navigateAction;

var debug = require("debug")("editDocument");

module.exports = function (context, payload, done) {

    var authStore = context.getStore(AuthStore);
    var userName = authStore.getCurrentUser();
    
    var documentData = { title: payload.title, content: payload.content };

    context.api.editDocument(userName, payload.id, documentData, function (err, res) {
        if (err) {
            debug('error', err);
            context.dispatch("event:EditDocumentFailure", err);
            done();
            return;
        }
        
        if (!res.ok) {
            debug('error', res);
            context.dispatch("event:EditDocumentFailure", res.text);
            done();
            return;
        }
        
        debug('success');
        var document = res.body;
        context.dispatch("event:EditDocumentSuccess", document);
        
        var routeStore = context.getStore(RouteStore);
        context.executeAction(
            navigateAction, 
            { url: routeStore.makePath("documents", {}) }, 
            done);
    });
};