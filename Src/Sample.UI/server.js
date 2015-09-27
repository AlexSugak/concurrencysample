require('node-jsx').install({
    extension: ".jsx"
});

var express = require("express");
var path = require("path");
var react = require("react");
var createElementWithContext = require('fluxible-addons-react').createElementWithContext;

var bodyParser = require("body-parser");
var navigateAction = require('fluxible-router').navigateAction;

var HtmlComponent = react.createFactory(require("./components/Html.jsx"));

var expressState = require("express-state");
var app = require("./app");
var config = require('./configs/config.js');
var debug = require("debug")("server");
var server = express();

expressState.extend(server);

server.use("/public", express.static(path.join(__dirname, "build")));
server.use("/css", express.static(path.join(__dirname, "css")));
server.use("/css", express.static(path.join(__dirname, "node_modules/bootstrap/dist/css")));
server.use("/images", express.static(path.join(__dirname, "img")));

server.use(bodyParser.json());

function handleError(err, next) {
    if (err.status && err.status === 404) {
        next();
    } else {
        next(err);
    }
}

server.use(function (req, res, next) {
    var context = app.createContext({
        config: config
    });
    
    var ac = context.getActionContext();
    
    debug("Executing navigate action");
    
    ac.executeAction(navigateAction, {
        url: req.url
    }, function (err) {
        if (err) {
            handleError(err, next);
            return;
        }
        debug("Exposing context state");
        res.expose(app.dehydrate(context), "App");
        
        debug("Rendering Application component into html");
        
        var element = createElementWithContext(context);
        var html = react.renderToStaticMarkup(HtmlComponent({
            state: res.locals.state,
            context: context.getComponentContext(),
            markup: react.renderToString(element)
        }));
        
        debug("Sending markup");
        res.write('<!DOCTYPE html>' + html);
        res.end();
    });
});


var port = process.env.port || 1337;
server.listen(port);
console.log("Listening on port " + port);
