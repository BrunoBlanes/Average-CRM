declare const React: any;
declare const Fabric: any;
declare const ReactDOM: any;

namespace JSInterop {
	var Nav = Fabric.Nav,
		Fabric = Fabric.Fabric,
		createTheme = Fabric.createTheme,
		DefaultButton = Fabric.DefaultButton,
		PrimaryButton = Fabric.PrimaryButton,
		Customizations = Fabric.Customizations;

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

	class JsFunctions {

		// Apply my default theme
		public loadFluentTheme(): void {
			Customizations.applySettings({ theme: myTheme });
		}

		// Creates a default fluent ui button
		public renderDefaultButton(content: string, disabled: boolean, checked: boolean, container: string): void {
			const Button: any = () => {
				return(
					<DefaultButton text={content} allowDisabledFocus disabled = {disabled} checked = {checked} />
				);
			}

			this.renderButton(Button, container);
		}

		// Creates a primary fluent ui button
		public renderPrimaryButton(content: string, disabled: boolean, checked: boolean, container: string): void {
			const Button: any = () => {
				return (
					<PrimaryButton text={content} allowDisabledFocus disabled={disabled} checked={checked} />
				);
			};

			this.renderButton(Button, container);
		}

		// Render the button using fluent ui
		private renderButton(Button: any, container: string): void {
			const ButtonWrapper: any = () => <Fabric><Button /></Fabric>;
			ReactDOM.render(<ButtonWrapper />, document.getElementById(container))
		}

		// NAVIGATION PANE
		public renderNavigationPane(title: string, children, container): void {
			let navItems = [];
			var count = 1;
			for (var c in children) {
				if (children.hasOwnProperty(c)) {
					var item = { name: c, url: children[c], key: 'key' + count };
					navItems.push(item);
					count++;
				}
			};

			let navigationItems = [{
				name: title,
				links: navItems
			}];

			let navigationPage = function () {
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
		}
	}

	export function Load(): void {
		window['jsInterop'] = new JsFunctions();
	}
}

JSInterop.Load();