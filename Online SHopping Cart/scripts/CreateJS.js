$(document).ready(function () {
    $('#ddlbcat').change(function () {
        $.ajax({
            type: "post",
            url: "/Seller/GetProductCat",
            data: { id: $('#ddlbcat').val() },
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
 

    function delimage(det,file){
     
            if (file.name == det)
            {
                //var input = $('input#[type=file]');

               // file.replaceWith(file.val('').clone(true));

            }
        

    }




    function previewImages() {

        var preview = document.querySelector('#preview');

        if (this.files) {
            [].forEach.call(this.files, readAndPreview);
        }

        function readAndPreview(file) {

            // Make sure `file.name` matches our extensions criteria
            if (!/\.(jpe?g|png|gif)$/i.test(file.name)) {
                return alert(file.name + " is not an image");
            } // else...

            var reader = new FileReader();
           
            reader.addEventListener("load", function () {
             
                var image = new Image();
                image.height = 50;
                image.title = file.name;
                image.src = this.result;
                var span = document.createElement('span');
                span.setAttribute('class', 'pip');
                var remove = document.createElement('span');
                remove.setAttribute('class', 'remove_img_preview');
                span.appendChild(image);
                span.appendChild(remove);
                preview.appendChild(span);
            }, false);
            
            reader.readAsDataURL(file);
            $('#preview').on('click', '.remove_img_preview', function () {
                var det = $(this).parent(".pip ").find("img").attr('title');
                //console.log(det);
                delimage(det,file);
               $(this).parent(".pip").remove();


            });
        }
    
    }
    
   
 
  


    document.querySelector('#ImageData').addEventListener("change", previewImages, false);
});


