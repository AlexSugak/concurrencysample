"use strict";

var SyncStore = require("../stores/SyncStore");
var AuthStore = require("../stores/AuthStore");
var debug = require("debug")("loadData");

module.exports = function(context, payload, done) {
	var store = context.getStore(SyncStore);
	if (store.wasDataLoaded()) {
		debug("data was already loaded");
		done();
		return;
    }
    
    var authStore = context.getStore(AuthStore);
    var userName = authStore.getCurrentUser();

    context.api.getAllDocuments(userName, function (err, res) {
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
        
        debug('success');
        var documents = res.body.documents;
        context.dispatch("event:FetchAllDocumentsSuccess", documents);

        context.dispatch("DATA_LOADED");
        done();
        return;
    });
};