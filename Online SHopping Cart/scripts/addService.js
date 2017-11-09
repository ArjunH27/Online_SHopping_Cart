$(document).ready(function () {
    $('#ddlbcat').change(function () {
        $.ajax({
            type: "post",
            url: "/Service/GetProductCategory",
            data: { baseCategoryId: $('#ddlbcat').val() },
            datatype: "json",

            success: function (data) {
                var category = "<select id='ddlpcat'>";
                category = category + '<option value="">--Select--</option>';
                for (var i = 0; i < data.length; i++) {
                    category = category + '<option value=' + data[i].Value + '>' + data[i].Text + '</option>';
                }
                category = category + '</select>';
                $('#ddlpcat').html(category);
            }
        });
    });
});
function GetProductId() {
    var productCatId = $("#ddlpcat").val();
    $.ajax({
        url: '/Service/Products_PartialView',
        type: 'Get',
        data: { productCatId: productCatId },
        success: function (result) {
            $("#partialPlaceHolder").html(result);
        },
        error: function () {
            alert("failed");
        }
    });
}