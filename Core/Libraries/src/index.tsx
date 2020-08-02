import { Buttons } from './buttons';
import { Functions } from './functions';

export function CreateButton(primary: boolean, content: string, parent: string): void {
	Buttons.CreateButton(primary, content, parent);
}

export function StopScroll(id: string): void {
	Functions.StopScroll(id);
}