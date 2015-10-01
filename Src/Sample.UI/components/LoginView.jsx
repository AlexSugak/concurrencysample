/** @jsx React.DOM */

var React = require("react");
var Formsy = require('formsy-react');
var FormInput = require('./FormInput');
var debug = require("debug")("LoginView");

var loginUserAction = require('../actions/loginUser');
var navigateAction = require('fluxible-router').navigateAction;
var connectToStores = require('fluxible-addons-react').connectToStores;

var LoginView = React.createClass({

	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },

	getInitialState: function() {
		return { canSubmit: false };
	},

	enableButton: function () {
		this.setState({
			canSubmit: true
		});
    },
    disableButton: function () {
		this.setState({
			canSubmit: false
		});
    },
    submit: function (model) {
		var data = { userName: model.login};

		this.context.executeAction(loginUserAction, data);

		event.preventDefault();
    },

	render: function render() {
		return (
			<div>
				<Formsy.Form className="form-signin" onValidSubmit={this.submit} onValid={this.enableButton} onInvalid={this.disableButton}>
					<h2 className="form-signin-header">Please sign in</h2>
					<FormInput name="login" validationError="Login must contain [a-Z] letters only" validations="isAlpha" required/>
					<div className="checkbox">
					</div>
					<button className="btn btn-lg btn-primary btn-block" type="submit" disabled={!this.state.canSubmit}>Sign in</button>
				</Formsy.Form>
		    </div>
		);
	}
});

LoginView = connectToStores(LoginView, [], function(context, props) {
	return {
	}
});

module.exports = LoginView;