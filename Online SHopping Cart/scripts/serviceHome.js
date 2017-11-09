$(document).ready(function () {
    $('.jumbotron').hide();
    $('#c1').show();

    $('.list-group .list-group-item').click(function () {

        if ($(this).index() == 0) {

            $('#c1').slideToggle(1000);
            if (!$(this).hasClass('active')) {


            }
            $(".jumbotron:nth-child(1)").show();

            $('#c2,#c3').slideUp(1000);
        }
        else if ($(this).index() == 1) {
            $('#c2').slideToggle(1000);

            $(".jumbotron:nth-child(2)").show();
            $('#c1,#c3').slideUp(1000);
        }
        else if ($(this).index() == 2) {

            $(".jumbotron:nth-child(3)").show();
            $('#c1,#c2').slideUp(1000);
        }

        $('.list-group .list-group-item.active').removeClass('active');
        $(this).addClass('active');


    });


});
