$(document).ready(function () {
    $(".treeview li>ul").css('display', 'none'); // Hide all 2-level ul
    $(".collapsible").click(function (e) {
        e.preventDefault();
        $(this).toggleClass("collapse expand");
        $(this).closest('li').children('ul').slideToggle();
    });
});