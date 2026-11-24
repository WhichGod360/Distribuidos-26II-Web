"use strict";
document.querySelectorAll(".password-fields").forEach(group => {
    const input = group.querySelector("[data-password]");
    const help = group.querySelector("[data-password-help]");
    if (!input || !help) return;
    input.addEventListener("input", () => {
        const valid = input.value.length >= 8 && /\p{Lu}/u.test(input.value) && /\p{Nd}/u.test(input.value);
        help.classList.toggle("requirements-met", valid);
        help.textContent = valid ? "Cumple los requisitos de contraseña." : "Mínimo 8 caracteres, una mayúscula y un número.";
    });
});
document.querySelectorAll("[data-auto-refresh]").forEach(group => {
    const checkbox = group.querySelector("input[type=checkbox]");
    const button = group.querySelector("[data-refresh-button]");
    if (!checkbox || !button) return;
    window.setInterval(() => {
        const editing = document.activeElement && document.activeElement.matches("textarea, select, input:not([type=checkbox]):not([type=radio]):not([type=submit]):not([type=button]):not([type=hidden])");
        if (checkbox.checked && !document.hidden && !editing && !button.disabled) button.click();
    }, Number(group.dataset.autoRefresh) * 1000);
});
