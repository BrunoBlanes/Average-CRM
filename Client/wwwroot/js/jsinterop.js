"use strict";
var _a = window.Fabric,
	Nav = _a.Nav,
	Fabric = _a.Fabric,
	Separator = _a.Separator,
	createTheme = _a.createTheme,
	DefaultButton = _a.DefaultButton,
	PrimaryButton = _a.PrimaryButton,
	Customizations = _a.Customizations,
	initializeIcons = _a.initializeIcons;

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


// THEMES
window.loadFluentTheme = () => {
	Customizations.applySettings({ theme: myTheme });
};


// BUTTONS
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


// NAVIGATION PANE
window.renderNavigationPane = (title, children, container) => {
	var navItems = [];
	var count = 1;
	for (var c in children) {
		if (children.hasOwnProperty(c)) {
			var item = { name: c, url: children[c], key: 'key' + count };
			navItems.push(item);
			count++;
		}
	};

	var navigationItems = [
	{
		name: title,
		links: navItems
	}];

	var NavigationPage = function () {
		return React.createElement(Nav, {
			onRenderGroupHeader: RenderGroupHeader,
			ariaLabel: title,
			groups: navigationItems,
			initialSelectedKey: 'key1'
		});
	};

	function RenderGroupHeader(group) {
		return React.createElement("h3", null, group.name);
	}

	var NavigationPaneWrapper = function () {
		return React.createElement(Fabric, null, React.createElement(NavigationPage, null));
	};

	ReactDOM.render(React.createElement(NavigationPaneWrapper, null), document.getElementById(container));
}

// SEPARATOR
window.renderSeparator = (container) => {
	var SeparatorItem = function () {
		return React.createElement(Separator, {
			vertical: true
		});
	};

	var SeparatorItemWrapper = function () {
		return React.createElement(Fabric, null, React.createElement(SeparatorItem, null));
	};

	ReactDOM.render(React.createElement(SeparatorItemWrapper, null), document.getElementById(container));
}