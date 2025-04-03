// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
const spinner = $("#spinner-overlay")

// stop bootstrap dropdown menus from closing when their body is clicked
$('.dropdown-menu').on('click', function (e) {
    e.stopPropagation();
})
