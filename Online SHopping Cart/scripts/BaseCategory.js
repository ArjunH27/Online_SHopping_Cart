$(document).ready(function () {
    $('.edit').hide();
     
    $(document).on("click",".edit-case", function () {
        var tr = $(this).parents('tr:first');
        //var BaseCatId = tr.find('#BaseId').text();
        var BaseCatName = tr.find('#BaseName').text();
        var BaseCatDescription = tr.find('#BaseDescription').text();
      //  tr.find('#BaseCatId').val(BaseCatId);
        tr.find('#BaseCatName').val(BaseCatName);
        tr.find('#BaseCatDescription').val(BaseCatDescription);
        tr.find('.edit, .read').toggle();
    });
    $(document).on('click',".update-case",function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        var BaseCatId = $(this).prop('id');
        var BaseCatName = tr.find('#BaseCatName').val();
        var BaseCatDescription = tr.find('#BaseCatDescription').val();

        if (BaseCatName == "") {
            tr.find('#label1').html("Field cannot be empty");
            return false;
        }
        if (BaseCatDescription == "") {
            tr.find('#label2').html("Field cannot be empty");
            return false;
        }
        else {
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/Admin/BaseCategoryEdit",
                data: JSON.stringify({ "BaseCatId": BaseCatId, "BaseCatName": BaseCatName, "BaseCatDescription": BaseCatDescription }),
                dataType: "json",
                success: function (data) {
                    tr.find('.edit, .read').toggle();
                    $('.edit').hide();
                    tr.find('#BaseId').text(data.BaseCatId)
                    tr.find('#BaseName').text(data.BaseCatName);
                    tr.find('#BaseDescription').text(data.BaseCatDesc);
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
        tr.find('#label1').html("");
        tr.find('#label2').html("");
        tr.find('.edit, .read').toggle();
        $('.edit').hide();
    });
    $(document).on('click',".delete-case",function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        BaseCatId = $(this).prop('id');
        if (confirm('Are you sure you want to delete this?')) {
            $.ajax({
                type: 'POST',
                url: '/Admin/BaseCategoryDelete',
                data: { BaseCatId: BaseCatId },
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