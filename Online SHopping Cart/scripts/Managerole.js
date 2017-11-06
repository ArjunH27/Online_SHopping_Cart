$(document).ready(function () {
    $('.edit').hide();
    $(document).on("click",".edit-case", function () {
        var tr = $(this).parents('tr:first');
        //var RoleId = tr.find('#Id').text();
        var RoleName = tr.find('#Name').text();
        var RoleDescription = tr.find('#Description').text();
        //tr.find('#RoleId').val(RoleId);
        tr.find('#RoleName').val(RoleName);
        tr.find('#RoleDescription').val(RoleDescription);
        tr.find('.edit, .read').toggle();
    });
    $(document).on('click',".update-case",function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        var  RoleId = $(this).prop('id');
        //var RoleId = tr.find('#RoleId').val();
        var RoleName = tr.find('#RoleName').val();
        var RoleDescription = tr.find('#RoleDescription').val();
        if (RoleName == "") 
        {
            tr.find('#label1').html("Field cannot be empty");
            return false;
        }
        if(RoleDescription=="")
        {
            tr.find('#label2').html("Field cannot be empty");
            return false;
        }

        else {


            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/Admin/RoleEdit",
                data: JSON.stringify({ "RoleId": RoleId, "RoleName": RoleName, "RoleDescription": RoleDescription }),
                dataType: "json",
                success: function (data) {
                    tr.find('.edit, .read').toggle();
                    $('.edit').hide();
                    tr.find('#Name').text(data.RoleName);
                    tr.find('#Description').text(data.RoleDesc);
                    tr.find('#label1').html("");
                    tr.find('#label2').html("");
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
        tr.find('#label2').html("");
        tr.find('#label1').html("");
        tr.find('.edit, .read').toggle();
        $('.edit').hide();
    });
    $(document).on('click',".delete-case",function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        RoleId = $(this).prop('id');
        if (confirm('Are you sure you want to delete this?')) {
            $.ajax({
                type: 'POST',
                url: '/Admin/RoleDelete',
                data: { RoleId: RoleId },
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
        $current.addClass("thiss");

        $('.cat').each(function () {
            if ($(this).val() == $current.val() && $(this).attr('class') != $current.attr('class')) {
                alert('duplicate found!');
                $current.removeClass("thiss");
                //$current.addClass("edit");
                //$current.addClass("cat");
                $('.update-case').prop('disabled', true);
                location.reload(true);
                return false;
            }
            else {
                $('.update-case').prop('disabled', false);
                $current.addClass("edit");
                $current.addClass("cat");
                //$('.edit').hide();

            }
        });
        $current.removeClass("thiss");
    });

    //$(document).on('click', ".btn", function (e) {
    //    $('.message').show();

    //});
    //$(document).on('change', '.text', function () {
    //    $('.message').show();

    //});
});