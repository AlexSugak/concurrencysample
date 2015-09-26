"use strict";

var SyncStore = require("../stores/SyncStore");
var debug = require("debug")("loadData");

module.exports = function(context, payload, done) {
	var store = context.getStore(SyncStore);
	if (store.wasDataLoaded()) {
		debug("data was already loaded");
		done();
		return;
	}
    
    context.dispatch("DATA_LOADED");
	done();
	return;
};