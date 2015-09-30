"use strict";

var createStore = require("fluxible/addons/createStore");
var debug = require("debug")("ErrorMessagesStore");

var ErrorMessagesStore = createStore({
    storeName: "ErrorMessagesStore",
    
    handlers: {
        "event:CheckoutDocumentFailure": "whenErrorOccured",
        "event:DeleteDocumentFailure": "whenErrorOccured",
        "event:DeleteTicketFailure": "whenErrorOccured",
        "event:EditTicketConcurrencyError": "whenOptimisticConcurrencyErrorOccured",
        "event:EditTicketSuccess": "whenErrorCleared",
        "event:ErrorCleared": "whenErrorCleared",
        "DATA_LOADED": "whenDataLoaded"
    },
    whenErrorOccured: function (error) {
        debug("error occured", error);
        this.message = error;        
        this.emitChange();
    },
    whenOptimisticConcurrencyErrorOccured: function (error) {
        debug("optimistic concurrency error occured", error);
        this.message = error.message;        
        this.emitChange();
    },
    whenDataLoaded: function () {
        debug("data loaded");
        this.message = null;
        this.emitChange();
    },
    whenErrorCleared: function () {
        debug("error cleared");
        this.message = null;
        this.emitChange();
    },
    initialize: function () {
        this.message = null;
    },
    dehydrate: function () {
        return {
            message: this.message,
        };
    },
    rehydrate: function (state) {
        this.message = state.message;
    },

    // --- queries ---
    getErrorMessage: function () {
        debug("returning error message: ", this.message);
        return this.message;
    }
});

module.exports = ErrorMessagesStore;
