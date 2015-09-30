"use strict";

var debug = require("debug")("loginUser");
var loadData = require('../actions/clearError');

module.exports = function (context, payload, done) {
    
    context.dispatch("event:ErrorCleared");

	done();    
};