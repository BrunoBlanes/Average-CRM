const path = require("path");

module.exports = [
	{
		module: {
			rules: [
				{
					test: /\.(js|jsx)$/,
					exclude: /node_modules/,
					use: {
						loader: "babel-loader"
					}
				}
			]
		},
		output: {
			path: path.resolve(__dirname, "../../Client/wwwroot/js"),
			filename: "jsInterop.js",
			library: "JsFunctions",
			libraryTarget: "window"
		}
	},
	{
		module: {
			rules: [
				{
					test: /\.(js|jsx)$/,
					exclude: /node_modules/,
					use: {
						loader: "babel-loader"
					}
				}
			]
		},
		output: {
			path: path.resolve(__dirname, "../../Server/wwwroot/js"),
			filename: "jsInterop.js",
			library: "JsFunctions",
			libraryTarget: "window"
		}
	}
];