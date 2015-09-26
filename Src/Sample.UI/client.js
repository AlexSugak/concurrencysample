"use strict";

var React = require("react");
var debug = require("debug");
var bootstrapDebug = debug("Client");
var app = require("./app");
var createElementWithContext = require('fluxible-addons-react').createElementWithContext;
var dehydratedState = window.App; // Sent from the server

window.React = React; // For chrome dev tool support
debug.enable("*");

bootstrapDebug("rehydrating app");
app.rehydrate(dehydratedState, function (err, context) {
    if (err) {
        throw err;
    }
    window.context = context;
    var mountNode = document.getElementById("app");
    
    bootstrapDebug("React Rendering");
    React.render(createElementWithContext(context), mountNode, function () {
        bootstrapDebug("React Rendered");
    });
});