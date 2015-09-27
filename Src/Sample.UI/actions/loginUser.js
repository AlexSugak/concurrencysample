"use strict";

var debug = require("debug")("loginUser");
var loadData = require('../actions/loadData');

module.exports = function (context, payload, done) {
    
    //TODO: implement proper authentication
    
    if (!payload.userName || payload.userName === '') {
        context.dispatch("event:UserLogInFailed");
    }

    context.dispatch("event:UserLoggedIn", payload.userName);
    
    context.executeAction(loadData, {}, done);
};