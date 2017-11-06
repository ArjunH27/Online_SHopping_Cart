




$(function () {
    $('.edit').hide();
    $(document).on('click', '.edit-case', function () {
        var tr = $(this).parents('tr:first');
        tr.find('.edit, .read').toggle();
    });
    $(document).on('click', '.update-case', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        //var gender = tr.find('#Gender').val();
        //var cid= $('#ddlbcat').val();
        //var pid = $('#ddlpcat').val()
       // var name = tr.find('#ProductCatName').val();
        var name = tr.find('#ProductName').val();
        var price = tr.find('#ProductPrice').val();
        var desc = tr.find('#ProductDesc').val();
        var stock = tr.find('#ProductStock').val();


        if (ProductName == "") {
            tr.find('#label1').html("Field cannot be empty");
            return false;
        }
        if (ProductPrice == "") {
            tr.find('#label2').html("Field cannot be empty");
            return false;
        }
        if (ProductDesc == "") {
            tr.find('#label3').html("Field cannot be empty");
            return false;
        }
        if (ProductStock == "") {
            tr.find('#label4').html("Field cannot be empty");
            return false;
        }
        else {
            $.ajax({

                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: "/Seller/Edit",
                data: JSON.stringify({ "id": id, "name": name, "price": price, "features": desc, "stock": stock }),

                success: function (data) {
                    tr.find('.edit, .read').toggle();
                    $('.edit').hide();

                    tr.find('#ProductCatName').text(data.pt.ProductCatName);
                    tr.find('#ProductName').text(data.pt.ProductName);
                    tr.find('#ProductPrice').text(data.pt.ProductPrice);
                    tr.find('#ProductDesc').text(data.pt.ProductDesc);
                    tr.find('#ProductStock').text(data.pt.ProductStock);
                    window.location.href = "/Seller/Display";
                },


            });
            window.location.reload(true);
        }
    });
    $('.cancel-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        var id = $(this).prop('id');
        tr.find('.edit, .read').toggle();
        tr.find('#label1').html("");
        tr.find('#label2').html("");
        tr.find('#label3').html("");
        tr.find('#label4').html("");
        $('.edit').hide();
    });
    $('.delete-case').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "/Seller/Delete/" + id,
            dataType: "json",
            success: function (data) {
                alert('Delete success');
               
            },
            error: function () {
                alert('Error occured during delete.');
            }

        });
        window.location.reload(true);
    });


    $('.deleteimage').on('click', function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        id = $(this).prop('id');
        $.ajax({
            type: 'POST',
            contentType: "application/json; charset=utf-8",
            url: "/Seller/DeleteImage/" + id,
            dataType: "json",
            success: function (data) {
                alert('Delete success');
                
            },
            error: function () {
                alert('Error occured during delete.');
            }

        });
  
    });


    //$('.popup').on('click', function (e) {
    //   // demoP = document.getElementById("demo");
    //    e.preventDefault();

    //    id = $(this).prop('id');
    //    $.ajax({
    //        type: 'POST',
    //        contentType: "application/json; charset=utf-8",
    //        url: "/Seller/imagedisplay/" + id,
            


    //        dataType: "html",
    //        success: function (response) {
    //            $('#dialog').html(response);
    //            $('#dialog').dialog('open');
    //        },
    //        failure: function (response) {
    //            alert(response.responseText);
    //        },
    //        error: function (response) {
    //            alert(response.responseText);
    //        }
        
    //     });

    //});
    $(function () {
        $("#dialog").dialog({
            autoOpen: false,
            modal: true,
            height:400,
            width: 400,
            resizable: false,
            dialogClass: "MyClass",
            Cancel: function() {
                $( this ).dialog( "close" );
            },
            close: function(event, ui) {
                location.reload();
            },
            show: {
                effect: "slide",
                duration: 1500
             
            },
            hide: {
                effect: "fade",
                duration: 1000
            }
       
     
            
        });
       
    
          //$(".ui-dialog").removeClass("ui-corner-all");
           // $("#ui-dialog-title-dialog").hide();
           // $(".ui-dialog-titlebar").removeClass('ui-widget-header');
                   
            //$(".ui-dialog-titlebar").hide();



        $(".details").click(function () {
            id = $(this).prop('id');
            $.ajax({
                type: "POST",
                url: "/Seller/imagedisplay",
                data: '{id: "' + id + '" }',
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

    
});



//document.getElementById("uploadBtn").onclick = function () {
//    document.getElementById("uploadFile").value = this.value;
//};

