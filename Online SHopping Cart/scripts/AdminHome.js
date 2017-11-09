$(document).ready(function () {

    var left = $('.slide').offset().left;  // Get the calculated left position

    $(".slide").css({ left: left })  // Set the left to its calculated position
                 .animate({ "left": "0px" }, "slow");

});