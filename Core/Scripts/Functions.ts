// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

export class Functions {
	public static StopScroll(id: string): void {
		return document.getElementById(id).addEventListener('touchmove', function (e: TouchEvent) {
			e.preventDefault();
		});
	}

	public static FormatCPF(event: KeyboardEvent) {
		if (event.target instanceof HTMLElement) {
			$(event.target).mask("000.000.000-00");
		}
	}
}