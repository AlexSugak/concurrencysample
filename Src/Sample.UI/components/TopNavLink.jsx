/** @jsx React.DOM */

var React = require("react");
var NavLink = require("fluxible-router").NavLink;

var TopNavLink = React.createClass({
	propTypes: {
		route: React.PropTypes.string.isRequired,
		currentRoute: React.PropTypes.string.isRequired,
		title: React.PropTypes.string.isRequired,
		context: React.PropTypes.object.isRequired
	},

	render: function render() {
		var isActive = (this.props.route == this.props.currentRoute)
		return (
			<li className={isActive ? 'active' : ''}>
				<NavLink routeName={this.props.route} context={this.props.context}>
					{this.props.title}
				</NavLink>
			</li>
		);
	}
});


module.exports = TopNavLink;