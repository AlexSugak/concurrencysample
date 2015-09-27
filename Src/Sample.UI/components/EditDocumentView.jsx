/** @jsx React.DOM */

var React = require("react");
var Formsy = require('formsy-react');
var FormInput = require('./FormInput');

var DocumentsStore = require('../stores/DocumentsStore');
var RouteStore = require("fluxible-router").RouteStore;

var connectToStores = require('fluxible-addons-react').connectToStores;
var editDocument = require('../actions/editDocument');

var debug = require("debug")("EditDocumentView");

var EditDocumentView = React.createClass({

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
		var data = { id: this.props.document.id, title: model.title, content: model.content};

		this.context.executeAction(editDocument, data);

		event.preventDefault();
    },

	render: function render() {
		return (
			<div className="row">
				<div className="col-md-6">
					<h3>Edit Document</h3>

					<Formsy.Form onValidSubmit={this.submit} onValid={this.enableButton} onInvalid={this.disableButton}>
						<label htmlFor="title">Title</label>
						<FormInput name="title" validationError="Title required" value={this.props.document.title} required/>
						<label htmlFor="content">Content</label>
						<FormInput name="content" inputType="textarea" validationError="Content required" value={this.props.document.content} required/>
						<button className="btn btn-primary" type="submit" disabled={!this.state.canSubmit}>Save</button>
					</Formsy.Form>
				</div>
		    </div>
		);
	}
});

EditDocumentView = connectToStores(EditDocumentView, [DocumentsStore, RouteStore], function(context, props) {
	var documentsStore = context.getStore(DocumentsStore);
	var routeStore = context.getStore(RouteStore);
	var docId = routeStore.getCurrentRoute().get('params').get('id');

	return {
		document: documentsStore.getDocument(docId)
	}
});

module.exports = EditDocumentView;