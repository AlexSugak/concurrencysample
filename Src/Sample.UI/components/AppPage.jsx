/** @jsx React.DOM */

var React = require("react");

var StoreMixin = require("fluxible").StoreMixin;
var handleHistory = require('fluxible-router').handleHistory;
var provideContext = require('fluxible-addons-react').provideContext;
var connectToStores = require('fluxible-addons-react').connectToStores;

var TopNav = require("./TopNav");
var LoginView = require("./LoginView");
var AuthStore = require("../stores/AuthStore");

var debug = require("debug")("app");

var AppPage = React.createClass({
	displayName: "AppPage",

	render: function render() {
		var Handler = this.props.currentRoute.get('handler');
		var currentRoute = this.props.currentRoute.get('name');

		if (!this.props.isUserLoggedIn) {
			return (
				<div className="container">
					<LoginView />
				</div>
			);
		}

		return (
			<div className="container">
				<TopNav projectName="Concurrency Samples" route={currentRoute}/>
				<div>
					<Handler />
				</div>
			</div>
		);
	}
});

// wrap with history handler
AppPage = handleHistory(AppPage);

// and wrap that with context
AppPage = provideContext(AppPage);

AppPage = connectToStores(AppPage, [AuthStore], function(context, props) {
	return {
		isUserLoggedIn: context.getStore(AuthStore).isUserLoggedIn()
	}
});

module.exports = AppPage;