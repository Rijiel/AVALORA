$(function () {
    if ($(window).width() < 992) {
        $('#admin-dropdown').addClass('dropup');
        $('#admin-dropdown').removeClass('dropdown');
        $('#login-dropdown').removeClass('dropdown');
        $('#login-dropdown-menu').removeClass('dropdown-menu dropdown-menu-end');
    }
});


$(window).on('resize', function () {
    if ($(window).width() < 992) {
        $('#admin-dropdown').addClass('dropup');
        $('#admin-dropdown').removeClass('dropdown');

        $('#login-dropdown').removeClass('dropdown');
        $('#login-dropdown-menu').removeClass('dropdown-menu dropdown-menu-end');
    } else {
        $('#admin-dropdown').removeClass('dropup');
        $('#admin-dropdown').addClass('dropdown');
        $('#login-dropdown').addClass('dropdown');
        $('#login-dropdown-menu').addClass('dropdown-menu dropdown-menu-end');
    }
});