$(document).ready(function () {
    $('.edit').hide();
    $(document).on("click", ".edit-case", function () {
        var tr = $(this).parents('tr:first');
        // var ProductCatId = tr.find('#CatId').text();
        var ProductCatName = tr.find('#CatName').text();
        var ProductCatDescription = tr.find('#CatDescription').text();
        var BaseCat = tr.find("#BaseName").text();
        //tr.find('#ProductCatId').val(ProductCatId);
        tr.find('#ProductCatName').val(ProductCatName);
        tr.find('#ProductCatDescription').val(ProductCatDescription);
        tr.find('.edit, .read').toggle();
       tr.find('#BaseCategoryName option:selected').text(BaseCat);
    
        //tr.find('#BaseCategoryName option:selected').val(id);
        //var BaseCatid = tr.find('#BaseCategoryName').val();
//tr.find('#BaseCategoryName option:contains(' + BaseCat + ')').prop('selected', true);
   

       
      
    });
    $(document).on('click',".update-case",function (e) {
        e.preventDefault();
        var tr = $(this).parents('tr:first');
        var ProductCatId = $(this).prop('id');
        var ProductCatName = tr.find('#ProductCatName').val();
        var ProductCatDescription = tr.find('#ProductCatDescription').val();
        //if (tr.find('#BaseCategoryName').change()) {
        //    var BaseCat = tr.find('#BaseCategoryName option:selected').text();
        //    var BaseCatid = tr.find('#BaseCategoryName option:selected').val();
        //    return false
        //}
        //else {
        //    var BaseCat = tr.find("#BaseName").text();
        //    var BaseCatid = tr.find("#baseid").val();
        //}
        var BaseCat = tr.find("#BaseName").text();
        
        if (ProductCatName == "") {
            tr.find('#label1').html("Field cannot be empty");
            return false;
        }
        if (ProductCatDescription == "") {
            tr.find('#label2').html("Field cannot be empty");
            return false;
        }

           
        else {
            if (tr.find('#BaseCategoryName option:selected').text() == BaseCat) {
                var BaseCat = tr.find("#BaseName").text();
                var BaseCatid = tr.find("#baseid").text();
                
            }
            else {
                var BaseCat = tr.find('#BaseCategoryName option:selected').text();
                var BaseCatid = tr.find('#BaseCategoryName option:selected').val();
                
            }
            
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: "/Admin/ProductCategoryEdit",
                    data: JSON.stringify({ "ProductCatId": ProductCatId, "ProductCatName": ProductCatName, "ProductCatDescription": ProductCatDescription, BaseCatid: BaseCatid }),
                    dataType: "json",
                    success: function (data) {
                        tr.find('.edit, .read').toggle();
                        $('.edit').hide();
                        tr.find('#CatName').text(data.ProductCatName);
                        tr.find('#CatDescription').text(data.ProductCatDesc);
                        tr.find('#BaseName').text(BaseCat);
                        tr.find('#label1').html("");
                        tr.find('#label2').html("");
                        // window.location.href = data.Url;
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
        ProductCatId = $(this).prop('id');
        if (confirm('Are you sure you want to delete this?')) {
            $.ajax({
                type: 'POST',
                url: '/Admin/ProductCategoryDelete',
                data: { ProductCatId: ProductCatId },
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
    $(document).on('change','.cat',function () {

        var $current = $(this);
        $(this).addClass('thiss');

        $('.cat').each(function () {
            if ($(this).val() == $current.val() && $(this).attr('class') != $current.attr('class')) {
                alert('duplicate found!');
                $current.removeClass("thiss");
                $('.update-case').prop('disabled', true);
                location.reload(true);
                return false
            }
            else {
                $('.update-case').prop('disabled', false);

                //$('.edit').hide();
            }



        });
        $current.removeClass("thiss");
    });
});