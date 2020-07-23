var JSInterop;
(function (JSInterop) {
    var Nav = Fabric.Nav, Fabric = Fabric.Fabric, createTheme = Fabric.createTheme, DefaultButton = Fabric.DefaultButton, PrimaryButton = Fabric.PrimaryButton, Customizations = Fabric.Customizations;
    var myTheme = createTheme({
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
    var JsFunctions = /** @class */ (function () {
        function JsFunctions() {
        }
        // Apply my default theme
        JsFunctions.prototype.loadFluentTheme = function () {
            Customizations.applySettings({ theme: myTheme });
        };
        // Creates a default fluent ui button
        JsFunctions.prototype.renderDefaultButton = function (content, disabled, checked, container) {
            var Button = function () {
                return (React.createElement(DefaultButton, { text: content, allowDisabledFocus: true, disabled: disabled, checked: checked }));
            };
            this.renderButton(Button, container);
        };
        // Creates a primary fluent ui button
        JsFunctions.prototype.renderPrimaryButton = function (content, disabled, checked, container) {
            var Button = function () {
                return (React.createElement(PrimaryButton, { text: content, allowDisabledFocus: true, disabled: disabled, checked: checked }));
            };
            this.renderButton(Button, container);
        };
        // Render the button using fluent ui
        JsFunctions.prototype.renderButton = function (Button, container) {
            var ButtonWrapper = function () { return React.createElement(Fabric, null,
                React.createElement(Button, null)); };
            ReactDOM.render(React.createElement(ButtonWrapper, null), document.getElementById(container));
        };
        // NAVIGATION PANE
        JsFunctions.prototype.renderNavigationPane = function (title, children, container) {
            var navItems = [];
            var count = 1;
            for (var c in children) {
                if (children.hasOwnProperty(c)) {
                    var item = { name: c, url: children[c], key: 'key' + count };
                    navItems.push(item);
                    count++;
                }
            }
            ;
            var navigationItems = [{
                    name: title,
                    links: navItems
                }];
            var navigationPage = function () {
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
                return React.createElement(Fabric, null, React.createElement(navigationPage, null));
            };
            ReactDOM.render(React.createElement(NavigationPaneWrapper, null), document.getElementById(container));
        };
        return JsFunctions;
    }());
    function Load() {
        window['jsInterop'] = new JsFunctions();
    }
    JSInterop.Load = Load;
})(JSInterop || (JSInterop = {}));
JSInterop.Load();
//# sourceMappingURL=jsInterop.js.map