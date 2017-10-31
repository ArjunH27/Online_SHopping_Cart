$("#selectAll").on("click", function () {
    var ischecked = this.checked;
    $('#checkboxes').find("input:checkbox").each(function () {
        this.checked = ischecked;
    });
});
$("input[name='ids']").click(function () {
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
function ZoomImage(image) {

    document.getElementById("main").src = image;
    $("#popupdiv").dialog({
        width: 600,
        height: 600,
        modal: true,
        buttons: {
            Close: function () {
                $(this).dialog('close');
            }
        }
    });
}
