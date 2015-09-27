"use strict";

var FluxibleApp = require("fluxible");
var RouteStore = require("fluxible-router").RouteStore;
var routes = require("./configs/routes");

var apiPlugin = require("./plugins/apiPlugin.js");

var app = new FluxibleApp({
    component: require("./components/AppPage.jsx")
});

app.plug(apiPlugin);

var AppRouteStore = RouteStore.withStaticRoutes(routes);
app.registerStore(AppRouteStore);
app.registerStore(require("./stores/SyncStore"));
app.registerStore(require("./stores/DocumentsStore"));
app.registerStore(require("./stores/AuthStore"));
app.registerStore(require("./stores/ErrorMessagesStore"));

module.exports = app;