// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
jQuery(function () {
    var $ = jQuery;
    $("*[data-toggle='collapse']").on("click", function () {
        var $this = $(this);
        var id = $this.attr("href").replace("#", "");
        var collapsable = document.getElementById(id);
        $(collapsable).collapse("toggle");
    });
});
