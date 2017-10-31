using Online_SHopping_Cart.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace Online_SHopping_Cart
{
    public class ContentRepository
    {

        public Image_Table UploadImageInDataBase(HttpPostedFileBase file, Image_Table image)
        {
            image.BinaryImage = ConvertToBytes(file);

            return (image);


        }
        public byte[] ConvertToBytes(HttpPostedFileBase image)
        {
            byte[] imageBytes = null;
            BinaryReader reader = new BinaryReader(image.InputStream);
            imageBytes = reader.ReadBytes((int)image.ContentLength);
            return imageBytes;
        }
    }
}