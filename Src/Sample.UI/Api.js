"use strict";

var superagent = require('superagent');
var debug = require('debug')('Api');

function Api(options){
    options = options || {};
    var noop = function () { };

    this._getHost = options.getHost || noop;
};

Api.prototype.getAllDocuments = function (userName, callback) {
    debug('getAllDocuments using api host: ' + this._getHost());
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

Api.prototype.getDocumentById = function (userName, documentId, callback) {
    debug('getDocumentById using api host: ' + this._getHost());
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

Api.prototype.addDocument = function (userName, document, callback) {
    debug('addDocument using api host: ' + this._getHost());
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

Api.prototype.deleteDocument = function (userName, documentId, callback) {
    debug('addDocument using api host: ' + this._getHost());
    superagent
        .del(this._getHost() + '/documents/' + documentId)
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

module.exports = Api;