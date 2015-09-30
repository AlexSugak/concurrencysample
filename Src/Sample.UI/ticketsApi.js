"use strict";

var superagent = require('superagent');
var debug = require('debug')('TicketsApi');

function TicketsApi(options){
    options = options || {};
    var noop = function () { };

    this._getHost = options.getHost || noop;
};

TicketsApi.prototype.getAllTickets = function (userName, callback) {
    debug('getAllTickets using api host: ' + this._getHost());
    superagent
        .get(this._getHost() + '/tickets/')
        .accept('json')
        .set('Authorization', 'Bearer userName=' + userName)
        .end(function (err, res) {
            if (err) {
                debug('error', err);
            }
            callback(err, res);
        });
};

TicketsApi.prototype.getTicketById = function (userName, ticketId, callback) {
    debug('getTicketById using api host: ' + this._getHost());
    superagent
        .get(this._getHost() + '/tickets/' + ticketId)
        .accept('json')
        .set('Authorization', 'Bearer userName=' + userName)
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

TicketsApi.prototype.addTicket = function (userName, ticket, callback) {
    debug('addTicket using api host: ' + this._getHost());
    superagent
        .post(this._getHost() + '/tickets/')
        .set('Content-Type', 'application/json')
        .set('Authorization', 'Bearer userName=' + userName)
        .send(ticket)
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

TicketsApi.prototype.deleteTicket = function (userName, expectedVersion, ticketId, callback) {
    debug('deleteDocument using api host: ' + this._getHost());
    superagent
        .del(this._getHost() + '/tickets/' + ticketId)
        .set('Authorization', 'Bearer userName=' + userName)
        .set('If-Match', expectedVersion)
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

TicketsApi.prototype.editTicket = function (userName, expectedVersion, ticketId, ticket, callback) {
    debug('editTicket using TicketsApi host: ' + this._getHost());
    superagent
        .put(this._getHost() + '/tickets/' + ticketId)
        .set('Content-Type', 'application/json')
        .set('Authorization', 'Bearer userName=' + userName)
        .set('If-Match', expectedVersion)
        .send(ticket)
        .end(function (err, res) {
        if (err) {
            debug('error', err);
        }
        callback(err, res);
    });
};

module.exports = TicketsApi;