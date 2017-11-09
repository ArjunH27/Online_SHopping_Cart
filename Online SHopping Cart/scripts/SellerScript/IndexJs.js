$(function () {
    $("#datepicker").datepicker({ maxDate: new Date() });
    //    $("#datepicker").text($('#calpicker').val());
});

function myFunction() {
    var today = new Date();
    var dd = today.getDate();
    var mm = today.getMonth() + 1; //January is 0!
    var yyyy = today.getFullYear();

    if (dd < 10) {
        dd = '0' + dd
    }

    if (mm < 10) {
        mm = '0' + mm
    }

    today = mm + '/' + dd + '/' + yyyy;

    $('#pieimg').attr('src', '/Seller/Chart?date=' + today)
}

function TestOnTextChange(item) {



    var text = $('#datepicker').val();

    $('#pieimg').attr('src', '/Seller/Chart?date=' + text)


}