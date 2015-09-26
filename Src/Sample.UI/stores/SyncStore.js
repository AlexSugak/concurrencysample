"use strict";

var createStore = require("fluxible/addons/createStore");
var debug = require("debug")("SyncStore");

var SyncStore = createStore({
	storeName: "SyncStore",

	handlers: {
		"DATA_LOADED": "whenDataLoaded"
	},
	whenDataLoaded: function() {
		debug("data loaded");
		this.dataLoaded = true;
	},
	initialize: function() {
		this.dataLoaded = false;
	},
	wasDataLoaded: function() {
		return this.dataLoaded;
	},
	dehydrate: function() {
		return {
			dataLoaded: this.dataLoaded
		};
	},
	rehydrate: function(state) {
		this.dataLoaded = state.dataLoaded;
	}
});

module.exports = SyncStore;