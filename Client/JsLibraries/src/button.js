import * as React from 'react';
import ReactDOM from 'react-dom';
import { DefaultButton, PrimaryButton, Stack, IStackTokens, Fabric } from 'office-ui-fabric-react';

export function createButton() {
	"use strict";
	var stackTokens = { childrenGap: 40 };
	var ButtonDefaultExample = function (props) {
		var disabled = props.disabled, checked = props.checked;
		return React.createElement(Stack, { horizontal: true, tokens: stackTokens },
			React.createElement(DefaultButton, { text: "Ok", onClick: _alertClicked, allowDisabledFocus: true, disabled: disabled, checked: checked }),
			React.createElement(PrimaryButton, { text: "Primary", onClick: _alertClicked, allowDisabledFocus: true, disabled: disabled, checked: checked }));
	};
	function _alertClicked() {
		alert("Clicked");
	}
	var ButtonDefaultExampleWrapper = function () {
		return React.createElement(Fabric, null,
			React.createElement(ButtonDefaultExample, null));
	};
	ReactDOM.render(React.createElement(ButtonDefaultExampleWrapper, null), document.getElementById("content"));
}