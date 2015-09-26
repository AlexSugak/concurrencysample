"use strict";

var nothing = function (context, payload, done) {
    done();
};

module.exports = {
    index: {
        path: "/",
        method: "GET",
        action: nothing,
        handler: require('../components/IndexView')
    },
    error: {
        path: "/error",
        method: "GET",
        action: nothing,
        handler: require('../components/ErrorView')
    }
};