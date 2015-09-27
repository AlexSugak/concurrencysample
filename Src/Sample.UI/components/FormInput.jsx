/** @jsx React.DOM */

var React = require("react");
var Formsy = require('formsy-react');

var FormInput = React.createClass({
	mixins: [Formsy.Mixin],

	changeValue: function (event) {
		this.setValue(event.currentTarget.value);
	},

	renderInput: function(){
		if(this.props.inputType === 'textarea'){
			return (
				<textarea type="text" rows="5" className="form-control" onChange={this.changeValue} value={this.getValue()}/>
			);
		}

		return (
			<input type="text" className="form-control" onChange={this.changeValue} value={this.getValue()}/>
		);
	},
	render: function () {

		var className = this.showError() ? 'form-group has-error' : 'form-group';

		var errorMessage = this.getErrorMessage();
		return (
			<div className={className}>
				{this.renderInput()}
				<span>{errorMessage}</span>
			</div>
		);
	}
});

module.exports = FormInput;