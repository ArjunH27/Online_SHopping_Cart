$(document).ready(function () {
    $('.edit').hide();
    $(document).on("click",".edit-case", function () {
        var tr = $(this).parents('tr:first');
        //var LocationId = tr.find('#LId').text();
        var LocationName = tr.find('#LName').text();
        var LocationPIN = tr.find('#PIN').text();
        var LocationDescription = tr.find('#LDescription').text();
       // tr.find('#LocationId').val(LocationId);
        tr.find('#LocationName').val(LocationName);
        tr.find('#LocationPIN').val(LocationPIN);
        tr.find('#LocationDescription').val(LocationDescription);
        tr.find('.edit, .read').toggle();
    });
    $(document).on('click',".update-case",function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        var LocationId = $(this).prop('id');
        //var RoleId = tr.find('#RoleId').val();
        var LocationName = tr.find('#LocationName').val();
        var LocationPIN = tr.find('#LocationPIN').val();
        var LocationDescription = tr.find('#LocationDescription').val();
        if (LocationName == "") {
            tr.find('#label1').html("Field cannot be empty");
            return false;
        }
        if (LocationPIN == "") {
            tr.find('#label2').html("Field cannot be empty");
            return false;
        }
        if (LocationDescription == "") {
            tr.find('#label3').html("Field cannot be empty");
            return false;
        }
        else {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/Admin/LocationEdit",
                data: JSON.stringify({ "LocationId": LocationId, "LocationName": LocationName, "LocationPIN": LocationPIN, "LocationDescription": LocationDescription }),
                dataType: "json",
                success: function (data) {
                    tr.find('.edit, .read').toggle();
                    $('.edit').hide();
                    tr.find('#LName').text(data.LocationName);
                    tr.find('#PIN').text(data.LocationPIN);
                    tr.find('#LDescription').text(data.LocationDesc);
                    tr.find('#label1').html("");
                    tr.find('#label2').html("");
                    tr.find('#label3').html("");
                    //window.location = data.Url;
                },
                error: function (err) {
                    alert("error");
                }
            });
        }
    });
    $(document).on('click', ".cancel-case",function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        var id = $(this).prop('id');
        tr.find('#label1').html("");
        tr.find('#label2').html("");
        tr.find('#label3').html("");
        tr.find('.edit, .read').toggle();
        $('.edit').hide();
    });
    $(document).on('click',".delete-case",function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        LocationId = $(this).prop('id');
        if (confirm('Are you sure you want to delete this?')) {
            $.ajax({
                type: 'POST',
                url: '/Admin/LocationDelete',
                data: { LocationId: LocationId },
                dataType: "json",
                success: function (data) {
                    window.location.href = data.Url;
                },
                error: function () {
                    alert('Error occured during delete.');
                }
            });
        }
    });
    $(document).on('change', '.cat', function () {

        var $current = $(this);
        $(this).addClass('thiss');

        $('.cat').each(function () {
            if ($(this).val() == $current.val() && $(this).attr('class') != $current.attr('class')) {
                alert('duplicate found!');
                $current.removeClass("thiss");
                $('.update-case').prop('disabled', true);
                location.reload(true);
                return false;
            }
            else {
                $('.update-case').prop('disabled', false);
               
                //$('.edit').hide();

            }
 });
        $current.removeClass("thiss");
    });
});