/** @jsx React.DOM */

var React = require("react");
var debug = require("debug")("DocumentsView");

var DocumentsStore = require('../stores/DocumentsStore');

var navigateAction = require('fluxible-router').navigateAction;
var connectToStores = require('fluxible-addons-react').connectToStores;

var DocumentsView = React.createClass({

	contextTypes: {
        executeAction: React.PropTypes.func.isRequired
    },

	render: function render() {
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
							</tbody>
						</table>
					</div>
					<div className="col-md-6">
					</div>
				</div>
		    </div>
		);
	}
});

DocumentsView = connectToStores(DocumentsView, [DocumentsStore], function(context, props) {
	return {
		documents: context.getStore(DocumentsStore).getAllDocuments()
	}
});

module.exports = DocumentsView;