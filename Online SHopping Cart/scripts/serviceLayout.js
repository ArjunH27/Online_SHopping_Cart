$('ul.nav li.dropdown').hover(function () {
    $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeIn(500);
}, function () {
    $(this).find('.dropdown-menu').stop(true, true).delay(200).fadeOut(500);
});
function openNav() {
    document.getElementById("mySidenav").style.width = "250px";
    document.getElementById("mainview").style.marginLeft = "250px";
    document.body.style.backgroundColor = "rgba(0,0,0,0.4)";
}
function closeNav() {
    document.getElementById("mySidenav").style.width = "0";
    document.getElementById("mainview").style.marginLeft = "0";
    document.body.style.backgroundColor = "#e0e0e0";
}
$(".notif-class").click(function (e) {

    e.preventDefault();
    $.ajax({

        url: "/Service/DisableNotification", // comma here instead of semicolon
        success: function () {

        }

    });

});