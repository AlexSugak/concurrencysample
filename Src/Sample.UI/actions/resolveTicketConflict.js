"use strict";

var debug = require("debug")("resolveTicketConflict");

module.exports = function (context, payload, done) {

    context.dispatch("event:TicketConflictResolved", payload);

    done();
};