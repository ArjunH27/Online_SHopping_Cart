$(document).ready(function () {
    $('.edit').hide();
    $(".edit-case").on("click", function () {
        var tr = $(this).parents('tr:first');
        var ServiceId = tr.find('#ServiceId').text();
        var ServiceName = tr.find('#ServiceName').text();
        var DeliveryCharge = tr.find('#DeliveryCharge').text();
        var ServiceDesc = tr.find('#ServiceDesc').text();
        tr.find('#txtServiceId').val(ServiceId);
        tr.find('#txtServiceName').val(ServiceName);
        tr.find('#txtDeliveryCharge').val(DeliveryCharge);
        tr.find('#txtServiceDesc').val(ServiceDesc);
        tr.find('.edit, .read').toggle();
    });
    $('.update-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        ServiceId = $(this).prop('id');
        var ServiceName = tr.find('#txtServiceName').val();
        var DeliveryCharge = tr.find('#txtDeliveryCharge').val();
        var ServiceDesc = tr.find('#txtServiceDesc').val();
        var LocationName = tr.find('#LocationName').text();
        var ProductName = tr.find('#ProductName').text();
        if (ServiceName == "") {
            tr.find('#label1').html("Field cannot be empty");
            return false;
        }
        if (DeliveryCharge == "") {
            tr.find('#label2').html("Field cannot be empty");
            return false;
        }
        if (ServiceDesc == "") {
            tr.find('#label3').html("Field cannot be empty");
            return false;
        }
        $.ajax({
            type: "POST",
            contentType: "application/json; charset=utf-8",
            url: "/Service/ServiceEdit",
            data: JSON.stringify({ "ServiceId": ServiceId, "ServiceName": ServiceName, "DeliveryCharge": DeliveryCharge, "ServiceDesc": ServiceDesc, "LocationName": LocationName, "ProductName": ProductName }),
            dataType: "json",
            success: function (data) {
                tr.find('.edit, .read').toggle();
                $('.edit').hide();
                tr.find('#ServiceName').text(data.ServiceName);
                tr.find('#DeliveryCharge').text(data.DeliveryCharge);
                tr.find('#ServiceDesc').text(data.ServiceDesc);
                tr.find('#LocationName').text(data.LocationName);
                tr.find('#ProductName').text(data.ProductName);
                tr.find('#label1').html("");
                tr.find('#label2').html("");
                tr.find('#label3').html("");
                window.location.href = response.Url;

            },
            error: function (err) {
                alert("error");
            }
        });
    });
    $('.cancel-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        var ServiceId = $(this).prop('ServiceId');
        tr.find('#label1').html("");
        tr.find('#label2').html("");
        tr.find('#label3').html("");
        tr.find('.edit, .read').toggle();
        $('.edit').hide();
    });
    $('.delete-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        var serviceId = $(this).prop('id');
        debugger;
        $.ajax({
            url: '/Service/ServiceDelete',
            type: 'POST',
            data: { serviceId: serviceId },
            dataType: "json",
            success: function (response) {
                alert('Delete success');
                window.location.href = response.Url;
            },
            error: function () {

                alert('Error occured during delete.');
            }
        });
    });
});