import { FASTDesignSystemProvider, createColorPalette } from "@microsoft/fast-components";
import { parseColorHexRGB } from "@microsoft/fast-colors";


const neutral = "#323130";
const accent = "#DF0000";

export class Fluent {
	public static ApplyColorPalette(): void {
		let provider: FASTDesignSystemProvider = document.querySelector("fluent-design-system-provider");

		// Create and apply the accent color palette
		let palette = createColorPalette(parseColorHexRGB(accent));
		provider.accentPalette = palette;
		provider.accentBaseColor = accent;

		// Create and apply the neutral color palette
		palette = createColorPalette(parseColorHexRGB(neutral));
		provider.neutralPalette = palette;
	}
}