/** @jsx React.DOM */

var React = require("react");
var Formsy = require('formsy-react');
var FormInput = require('./FormInput');

var connectToStores = require('fluxible-addons-react').connectToStores;
var addDocument = require('../actions/addDocument');

var debug = require("debug")("NewDocumentView");

var NewDocumentView = React.createClass({

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
		var data = { title: model.title, content: model.content};

		this.context.executeAction(addDocument, data);

		event.preventDefault();
    },

	render: function render() {
		return (
			<div className="row">
				<div className="col-md-6">
					<h3>New Document</h3>

					<Formsy.Form onValidSubmit={this.submit} onValid={this.enableButton} onInvalid={this.disableButton}>
						<label htmlFor="title">Title</label>
						<FormInput name="title" validationError="Title required" required/>
						<label htmlFor="content">Content</label>
						<FormInput name="content" inputType="textarea" validationError="Content required" required/>
						<button className="btn btn-primary" type="submit" disabled={!this.state.canSubmit}>Save</button>
					</Formsy.Form>
				</div>
		    </div>
		);
	}
});

NewDocumentView = connectToStores(NewDocumentView, [], function(context, props) {
	return {
	}
});

module.exports = NewDocumentView;