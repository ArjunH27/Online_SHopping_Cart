

$(function () {
    $("#dialog").dialog({
        width: 600,
        height: 600,
        autoOpen: false,
        modal: true,
        title: "View Details"

    });
    $(".details").on("click", function () {
        var tr = $(this).parents('tr:first');
        var OrderId = tr.find('#OrderId').html();
        var UserId = tr.find('#UserId').html();
        $.ajax({
            type: "POST",
            url: "/Service/OrderDetails",
            data: JSON.stringify({ "OrderId": OrderId, "UserId": UserId }),
            contentType: "application/json; charset=utf-8",
            dataType: "html",
            success: function (response) {
                $('#dialog').html(response);

                $('#dialog').dialog('open');
            },
            failure: function (response) {
                alert(response.responseText);
            },
            error: function (response) {
                alert(response.responseText);
            }
        });
    });
});