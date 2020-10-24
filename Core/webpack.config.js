const path = require("path");

module.exports = [
	{
		entry: "./Scripts/Index.js",
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
			path: path.resolve(__dirname, "../Server/wwwroot/script"),
			filename: "site.min.js",
			library: "JsFunctions",
			libraryTarget: "window"
		}
	}
];