"use strict";
var _a = window.Fabric,
	Fabric = _a.Fabric,
	createTheme = _a.createTheme,
	DefaultButton = _a.DefaultButton,
	PrimaryButton = _a.PrimaryButton,
	Customizations = _a.Customizations;

const myTheme = createTheme({
	palette: {
		themePrimary: '#ee0000',
		themeLighterAlt: '#fef4f4',
		themeLighter: '#fcd4d4',
		themeLight: '#faafaf',
		themeTertiary: '#f46262',
		themeSecondary: '#ef1d1d',
		themeDarkAlt: '#d50000',
		themeDark: '#b40000',
		themeDarker: '#850000',
		neutralLighterAlt: '#faf9f8',
		neutralLighter: '#f3f2f1',
		neutralLight: '#edebe9',
		neutralQuaternaryAlt: '#e1dfdd',
		neutralQuaternary: '#d0d0d0',
		neutralTertiaryAlt: '#c8c6c4',
		neutralTertiary: '#a19f9d',
		neutralSecondary: '#605e5c',
		neutralPrimaryAlt: '#3b3a39',
		neutralPrimary: '#323130',
		neutralDark: '#201f1e',
		black: '#000000',
		white: '#ffffff',
	}
});

window.loadFluentTheme = () => {
	Customizations.applySettings({ theme: myTheme });
};

window.renderDefaultButton = (content, disabled, allowDisabledFocus, checked, container) => {
	var Button = function () {
		return React.createElement(DefaultButton, {
			text: content,
			allowDisabledFocus: allowDisabledFocus,
			disabled: disabled,
			checked: checked
		});
	};

	RenderButton(Button, container);
};

window.renderPrimaryButton = (content, disabled, allowDisabledFocus, checked, container) => {
	var Button = function () {
		return React.createElement(PrimaryButton, {
			text: content,
			allowDisabledFocus: allowDisabledFocus,
			disabled: disabled,
			checked: checked
		});
	};

	RenderButton(Button, container);
};

var RenderButton = function (Button, container) {
	var ButtonWrapper = function () {
		return React.createElement(Fabric, null, React.createElement(Button, null));
	};
	ReactDOM.render(React.createElement(ButtonWrapper, null), document.getElementById(container));
};