"use strict";

var createStore = require("fluxible/addons/createStore");
var debug = require("debug")("AuthStore");

var AuthStore = createStore({
    storeName: "AuthStore",
    
    handlers: {
        "event:UserLoggedIn": "whenUserLoggedIn"
    },
    whenUserLoggedIn: function (userName) {
        debug("user logged in");
        this.isLoggenIn = true;
        this.userName = userName;
        this.emitChange();
    },
    initialize: function () {
    },
    dehydrate: function () {
        return {
            isLoggenIn: this.isLoggenIn,
            userName: this.userName
        };
    },
    rehydrate: function (state) {
        this.isLoggenIn = state.isLoggenIn;
        this.userName = state.userName;
    },

    // --- queries ---
    isUserLoggedIn: function () {
        debug("returning if user logged in: ", this.isLoggenIn);
        return this.isLoggenIn;
    },
    getCurrentUser: function () {
        debug("returning currentUser: ", this.userName);
        return this.userName;
    }
});

module.exports = AuthStore;
