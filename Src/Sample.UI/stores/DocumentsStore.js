"use strict";

var createStore = require("fluxible/addons/createStore");
var debug = require("debug")("DocumentsStore");

var DocumentsStore = createStore({
    storeName: "DocumentsStore",
    
    handlers: {
        "event:FetchAllDocumentsSuccess": "whenDocumentsFetched",
        "event:AddDocumentSuccess": "whenDocumentAdded",
        "event:DeleteDocumentSuccess": "whenDocumentDeleted",
        "event:CheckoutDocumentSuccess": "whenDocumentCheckedOut",
        "event:CheckinDocumentSuccess": "whenDocumentCheckedIn"
    },
    whenDocumentsFetched: function (documents) {
        debug("documents fetched");
        this.documents = documents;
        this.emitChange();
    },
    whenDocumentAdded: function (document) {
        debug("document added");
        this.documents.push(document);
        this.emitChange();
    },
    whenDocumentDeleted: function (documentId) {
        debug("document deleted");
        this.documents = this.documents.filter(function (doc) { return doc.id !== documentId; });
        this.emitChange();
    },
    whenDocumentCheckedOut: function (lockInfo) {
        debug("document checked out");
        for (i = 0; i < this.documents.length; i++) {
            if (this.documents[i].id === lockInfo.id) {
                this.documents[i].checkedOutBy = lockInfo.checkedOutBy;
            }
        }
        this.emitChange();
    },
    whenDocumentCheckedIn: function (documentId) {
        debug("document checked in");
        for (i = 0; i < this.documents.length; i++) {
            if (this.documents[i].id === documentId) {
                this.documents[i].checkedOutBy = null;
            }
        }
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
