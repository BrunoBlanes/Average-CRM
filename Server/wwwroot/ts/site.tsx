// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.


//function FormatCPF() {
//	$("#cpf").mask("999.999.999-99");
//}

namespace JSInterop {
	class JsFunctions {
		public stopScroll(id: string): void {
			return document.getElementById(id).addEventListener('touchmove', function (e) {
				e.preventDefault();
			})
		}
	}

	export function Load(): void {
		window['jsInterop'] = new JsFunctions();
	}
}

JSInterop.Load();