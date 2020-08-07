import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { PrimaryButton, DefaultButton, Fabric } from '@fluentui/react';
export class Buttons {
    static CreateButton(primary, content, parent) {
        const Button = () => {
            if (primary) {
                return (React.createElement(PrimaryButton, { text: content, allowDisabledFocus: true, disabled: false, checked: false }));
            }
            else {
                return (React.createElement(DefaultButton, { text: content, allowDisabledFocus: true, disabled: false, checked: false }));
            }
        };
        this.RenderButton(Button, parent);
    }
    static RenderButton(Button, parent) {
        const ButtonWrapper = () => React.createElement(Fabric, null,
            React.createElement(Button, null));
        ReactDOM.render(React.createElement(ButtonWrapper, null), document.getElementById(parent));
    }
}
//# sourceMappingURL=buttons.js.map