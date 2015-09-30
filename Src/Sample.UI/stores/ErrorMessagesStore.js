"use strict";

var createStore = require("fluxible/addons/createStore");
var debug = require("debug")("ErrorMessagesStore");

var ErrorMessagesStore = createStore({
    storeName: "ErrorMessagesStore",
    
    handlers: {
        "event:CheckoutDocumentFailure": "whenErrorOccured",
        "event:DeleteDocumentFailure": "whenErrorOccured",
        "event:DeleteTicketFailure": "whenErrorOccured",
        "DATA_LOADED": "whenDataLoaded"
    },
    whenErrorOccured: function (error) {
        debug("error occured", error);
        this.message = error;        
        this.emitChange();
    },
    whenDataLoaded: function () {
        debug("data loaded");
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
