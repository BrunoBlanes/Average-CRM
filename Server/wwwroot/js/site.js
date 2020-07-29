// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
//function FormatCPF() {
//	$("#cpf").mask("999.999.999-99");
//}
var JSInterop;
(function (JSInterop) {
    class JsFunctions {
        stopScroll(id) {
            return document.getElementById(id).addEventListener('touchmove', function (e) {
                e.preventDefault();
            });
        }
    }
    function Load() {
        window['jsInterop'] = new JsFunctions();
    }
    JSInterop.Load = Load;
})(JSInterop || (JSInterop = {}));
JSInterop.Load();
//# sourceMappingURL=site.js.map