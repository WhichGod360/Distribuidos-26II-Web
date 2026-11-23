(function () {
    "use strict";
    var formulario = document.getElementById("simularPago");
    if (!formulario) return;

    function confirmarAlAbrir() {
        if (document.visibilityState !== "visible") return;
        document.removeEventListener("visibilitychange", confirmarAlAbrir);
        formulario.submit();
    }

    document.addEventListener("visibilitychange", confirmarAlAbrir);
    confirmarAlAbrir();
}());
