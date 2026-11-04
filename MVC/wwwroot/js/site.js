// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener("change", event => {
    const input = event.target;
    if (!(input instanceof HTMLInputElement) || !input.matches(".file-picker-input")) return;

    const status = document.getElementById(input.dataset.statusTarget);
    if (!status) return;

    const files = [...input.files];
    if (files.length === 0) {
        status.textContent = input.multiple ? "No files selected" : "No file selected";
        return;
    }

    status.textContent = files.length === 1
        ? files[0].name
        : `${files.length} files selected`;
});