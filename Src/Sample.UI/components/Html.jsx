var React = require("react");

var Html = React.createClass({

	render: function render() {
		return (
			<html>
				<head>
					<meta httpEquiv="Content-Type" content="text/html; charset=utf-8"/>
					<link rel="stylesheet" href="/css/bootstrap.min.css" type="text/css" />
					<link rel="stylesheet" href="/css/custom.css" type="text/css" />
				</head>
				<body>
					<div id="app" dangerouslySetInnerHTML={{__html: this.props.markup}}></div>
					<footer className="footerinfo">
					</footer>
				</body>
				<script dangerouslySetInnerHTML={{__html: this.props.state}} type="text/javascript"></script>
				<script src="/public/js/client.js" defer type="text/javascript"></script>
			</html>
		);
	}
});


module.exports = Html;