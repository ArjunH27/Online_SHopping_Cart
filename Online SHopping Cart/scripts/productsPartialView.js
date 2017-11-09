$("#selectAll").on("click", function () {
    var ischecked = this.checked;
    $('#checkboxes').find("input:checkbox").each(function () {
        this.checked = ischecked;
    });
});
$("input[name='productIds']").click(function () {
    var totalRows = $("#checkboxes").length;
    var checked = $("#checkboxes input :checkbox:checked").length;

    if (checked == totalRows) {
        $("#checkboxes").find("input:checkbox").each(function () {
            this.checked = true;
        });

    }
    else {
        $("#selectAll").removeAttr("checked");
    }
});

function ZoomImage(imageId) {
    $("#popupdiv").dialog({
        width: 500,
        height: 500,
        autoOpen: false,
        modal: true,
        title: "Product View"

    });
    $.ajax({
        type: "POST",
        url: "/Service/ImageDisplay",
        data: '{imageId: "' + imageId + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        success: function (response) {
            $('#popupdiv').html(response);

            $('#popupdiv').dialog('open');

        },
        failure: function (response) {
            alert(response.responseText);
        },
        error: function (response) {
            alert(response.responseText);
        }

    });


}
