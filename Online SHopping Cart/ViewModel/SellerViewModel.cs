using Online_SHopping_Cart.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Online_SHopping_Cart.ViewModel
{
    public class SellerViewModel
    {
        public int ProductId { get; set; }
        public int ProductCatId { get; set; }
        [Required]
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDesc { get; set; }
        public int ProductStock { get; set; }
        [Required]
        public byte[] BinaryImage { get; set; }
        public int ImageId { get; set; }
        public string ProductCatName { get; set; }
        public string BaseCatName { get; set; }
        public int OrderId { get; set; }
        public string ServiceName { get; set; }
        public string UserName { get; set; }
        public Nullable<int> Quantity { get; set; }
        public IList<BaseCategory_Table> BaseProducts { get; set; }
        public IList<Image_Table> image { get; set; }


        public List<SelectListItem> categorylist = new List<SelectListItem>();
    }
}