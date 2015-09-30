"use strict";

var FluxibleApp = require("fluxible");
var RouteStore = require("fluxible-router").RouteStore;
var routes = require("./configs/routes");

var documentsApiPlugin = require("./plugins/documentsApiPlugin.js");
var ticketsApiPlugin = require("./plugins/ticketsApiPlugin.js");

var app = new FluxibleApp({
    component: require("./components/AppPage.jsx")
});

app.plug(documentsApiPlugin);
app.plug(ticketsApiPlugin);

var AppRouteStore = RouteStore.withStaticRoutes(routes);
app.registerStore(AppRouteStore);
app.registerStore(require("./stores/SyncStore"));
app.registerStore(require("./stores/DocumentsStore"));
app.registerStore(require("./stores/TicketsStore"));
app.registerStore(require("./stores/AuthStore"));
app.registerStore(require("./stores/ErrorMessagesStore"));

module.exports = app;