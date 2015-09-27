/** @jsx React.DOM */

var React = require("react");
var debug = require("debug")("DocumentsView");

var DocumentsStore = require('../stores/DocumentsStore');
var AuthStore = require("../stores/AuthStore");
var NavLink = require("fluxible-router").NavLink;

var navigateAction = require('fluxible-router').navigateAction;
var deleteDocument = require('../actions/deleteDocument');
var checkoutDocument = require('../actions/checkoutDocument');
var checkinDocument = require('../actions/checkinDocument');
var connectToStores = require('fluxible-addons-react').connectToStores;

var DocumentRow = React.createClass({
	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },
	deleteClicked: function (event) {
		var data = { documentId: this.props.document.id};

		this.context.executeAction(deleteDocument, data);
		event.preventDefault();
    },
	checkoutClicked: function (event) {
		var data = { documentId: this.props.document.id};

		this.context.executeAction(checkoutDocument, data);
		event.preventDefault();
    },
	checkinClicked: function (event) {
		var data = { documentId: this.props.document.id};

		this.context.executeAction(checkinDocument, data);
		event.preventDefault();
    },
	render: function render() {
		var docId = this.props.document.id;
		var isCheckedOut = !(this.props.document.checkedOutBy === null);
		var checkedOutText = isCheckedOut ? this.props.document.checkedOutBy : 'N/A';
		var rowClass = isCheckedOut ? 'danger' : '';

		var actions = [];
		if(!isCheckedOut)
		{
			actions.push(<a key={'checkout_'+ docId} className="btn btn-link" onClick={this.checkoutClicked}>check out</a>);
		}
		if(isCheckedOut && this.props.userName === this.props.document.checkedOutBy)
		{
			actions.push(<a key={'checkin_'+ docId} className="btn btn-link" onClick={this.checkinClicked}>check in</a>);
		}
		if(isCheckedOut && this.props.userName === this.props.document.checkedOutBy)
		{
			actions.push(<NavLink routeName="editDocument" navParams={{id: docId}} key={'edit_' + docId} className="btn btn-link">
							edit
						 </NavLink>);
		}
		if(!isCheckedOut || this.props.userName === this.props.document.checkedOutBy)
		{
			actions.push(<a key={'delete_'+ docId} className="btn btn-link" onClick={this.deleteClicked}>delete</a>);
		}

		return (
			<tr className={rowClass}>
				<td>{this.props.document.title}</td>
				<td>{checkedOutText}</td>
				<td>
					{actions}
				</td>
			</tr>
		);
	}
});

var DocumentsView = React.createClass({

	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },

	render: function render() {
		var rows = [];
		for(i = 0; i < this.props.documents.length; i++){
			var doc = this.props.documents[i];
			rows.push(<DocumentRow  document={doc} userName={this.props.userName} key={doc.id} />);
		}

		return (
			<div>
				<h3>Documents</h3>
				<div className="row">
					<div className="col-md-6">
						<table className="table table-striped table-hover">
							<thead>
								<tr>
									<th>Title</th>
									<th>Checked Out To</th>
									<th></th>
								</tr>
							</thead>
							<tbody>
								{rows}
							</tbody>
						</table>
						<NavLink routeName="newDocument" className="btn btn-default">
							Add new document
						</NavLink>
					</div>
					<div className="col-md-6">
					</div>
				</div>
		    </div>
		);
	}
});

DocumentsView = connectToStores(DocumentsView, [DocumentsStore, AuthStore], function(context, props) {
	return {
		documents: context.getStore(DocumentsStore).getAllDocuments(),
		userName: context.getStore(AuthStore).getCurrentUser()
	}
});

module.exports = DocumentsView;