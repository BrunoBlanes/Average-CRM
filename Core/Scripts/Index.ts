import { Functions } from './Functions';
import { Fluent } from './Fluent';

export function StopScroll(id: string): void {
	Functions.StopScroll(id);
}

export function FormatCPF(event: KeyboardEvent): void {
	Functions.FormatCPF(event);
}

export function ApplyColorPalette(): void {
	Fluent.ApplyColorPalette();
}