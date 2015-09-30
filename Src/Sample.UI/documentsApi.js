"use strict";

var superagent = require('superagent');
var debug = require('debug')('DocumentsApi');

function DocumentsApi(options){
    options = options || {};
    var noop = function () { };

    this._getHost = options.getHost || noop;
};

DocumentsApi.prototype.getAllDocuments = function (userName, callback) {
    debug('getAllDocuments using DocumentsApi host: ' + this._getHost());
    superagent
        .get(this._getHost() + '/documents/')
        .accept('json')
        .set('Authorization', 'Bearer userName=' + userName)
        .end(function (err, res) {
            if (err) {
                debug('error', err);
            }
            callback(err, res);
        });
};

DocumentsApi.prototype.getDocumentById = function (userName, documentId, callback) {
    debug('getDocumentById using DocumentsApi host: ' + this._getHost());
    superagent
        .get(this._getHost() + '/documents/' + documentId)
        .accept('json')
        .set('Authorization', 'Bearer userName=' + userName)
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

DocumentsApi.prototype.addDocument = function (userName, document, callback) {
    debug('addDocument using DocumentsApi host: ' + this._getHost());
    superagent
        .post(this._getHost() + '/documents/')
        .set('Content-Type', 'application/json')
        .set('Authorization', 'Bearer userName=' + userName)
        .send(document)
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

DocumentsApi.prototype.deleteDocument = function (userName, documentId, callback) {
    debug('deleteDocument using DocumentsApi host: ' + this._getHost());
    superagent
        .del(this._getHost() + '/documents/' + documentId)
        .set('Authorization', 'Bearer userName=' + userName)
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

DocumentsApi.prototype.checkoutDocument = function (userName, documentId, callback) {
    debug('checkoutDocument using DocumentsApi host: ' + this._getHost());
    superagent
        .put(this._getHost() + '/documents/' + documentId + '/lock')
        .set('Authorization', 'Bearer userName=' + userName)
        .set('Content-Type', 'application/json')
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

DocumentsApi.prototype.checkinDocument = function (userName, documentId, callback) {
    debug('checkinDocument using DocumentsApi host: ' + this._getHost());
    superagent
        .del(this._getHost() + '/documents/' + documentId + '/lock')
        .set('Authorization', 'Bearer userName=' + userName)
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

DocumentsApi.prototype.editDocument = function (userName, documentId, document, callback) {
    debug('editDocument using DocumentsApi host: ' + this._getHost());
    superagent
        .put(this._getHost() + '/documents/' + documentId)
        .set('Content-Type', 'application/json')
        .set('Authorization', 'Bearer userName=' + userName)
        .send(document)
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

module.exports = DocumentsApi;