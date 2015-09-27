/** @jsx React.DOM */

var React = require("react");
var TopNavLink = require("./TopNavLink");

var connectToStores = require('fluxible-addons-react').connectToStores;
var AuthStore = require("../stores/AuthStore");

var TopNav = React.createClass({
	propTypes: {
		projectName: React.PropTypes.string.isRequired,
		route: React.PropTypes.string.isRequired,
	},

	render: function render() {
		var projectName = this.props.projectName;
		var route = this.props.route;
		return (
			<nav className="navbar navbar-default">
				<div className="container-fluid">
				  <div className="navbar-header">
					<button type="button" className="navbar-toggle collapsed" data-toggle="collapse" data-target="#navbar" aria-expanded="false" aria-controls="navbar">
					  <span className="sr-only">Toggle navigation</span>
					  <span className="icon-bar"></span>
					  <span className="icon-bar"></span>
					  <span className="icon-bar"></span>
					</button>
					<a className="navbar-brand" href="#">{projectName}</a>
				  </div>
				  <div id="navbar" className="navbar-collapse collapse">
					<ul className="nav navbar-nav">
						<TopNavLink title="Home" route="index" currentRoute={route} />
						<TopNavLink title="Documents" route="documents" currentRoute={route} />
						<TopNavLink title="Tickets" route="tickets" currentRoute={route} />
					</ul>
					<ul className="nav navbar-nav navbar-right">
						<li>
							<p className="navbar-text">
								Welcome, {this.props.userName}
							</p>
						</li>
					</ul>
				  </div>
				</div>
			  </nav>
		);
	}
});

TopNav = connectToStores(TopNav, [AuthStore], function(context, props) {
	return {
		userName: context.getStore(AuthStore).getCurrentUser()
	}
});

module.exports = TopNav;