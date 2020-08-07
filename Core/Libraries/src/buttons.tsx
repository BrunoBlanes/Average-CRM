import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { PrimaryButton, DefaultButton, Fabric } from '@fluentui/react';

export class Buttons {
	public static CreateButton(primary: boolean, content: string, parent: string) {
		const Button: React.FunctionComponent = () => {
			if (primary) {
				return (
					<PrimaryButton text={content} allowDisabledFocus disabled={false} checked={false} />
				);
			} else {
				return (
					<DefaultButton text={content} allowDisabledFocus disabled={false} checked={false} />
				);
			}
		};

		this.RenderButton(Button, parent);
	}

	private static RenderButton(Button: React.FunctionComponent, parent: string) {
		const ButtonWrapper = () => <Fabric><Button /></Fabric>;
		ReactDOM.render(<ButtonWrapper />, document.getElementById(parent));
	}
}