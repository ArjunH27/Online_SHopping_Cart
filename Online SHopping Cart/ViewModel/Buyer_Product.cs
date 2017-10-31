using Online_SHopping_Cart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_SHopping_Cart.ViewModel
{
    public class Buyer_Product
    {
        public List<Product_Table> plist { get; set; }

        public List<Image_Table> imglist { get; set; }

        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDesc { get; set; }
        public byte[] BinaryImage { get; set; }
    }
}