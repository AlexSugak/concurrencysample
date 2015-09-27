/** @jsx React.DOM */

var React = require("react");
var Formsy = require('formsy-react');

var FormInput = React.createClass({
	mixins: [Formsy.Mixin],

	changeValue: function (event) {
		this.setValue(event.currentTarget.value);
	},
	render: function () {

		var className = this.showError() ? 'form-group has-error' : 'form-group';

		var errorMessage = this.getErrorMessage();
		return (
			<div>
				<input type="text" className="form-control" onChange={this.changeValue} value={this.getValue()}/>
				<span>{errorMessage}</span>
			</div>
		);
	}
});

module.exports = FormInput;