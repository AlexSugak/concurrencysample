"use strict";

var createStore = require("fluxible/addons/createStore");
var debug = require("debug")("DocumentsStore");

var DocumentsStore = createStore({
    storeName: "DocumentsStore",
    
    handlers: {
        "event:FetchAllDocumentsSuccess": "whenDocumentsFetched"
    },
    whenDocumentsFetched: function (documents) {
        debug("documents fetched");
        this.documents = documents;
        this.emitChange();
    },
    initialize: function () {
        this.documents = [];
    },
    dehydrate: function () {
        return {
            documents: this.documents
        };
    },
    rehydrate: function (state) {
        this.documents = state.documents;
    },

    // --- queries ---

    getAllDocuments: function () {
        debug("returning all documents", this.documents);
        return this.documents;
    }
});

module.exports = DocumentsStore;
