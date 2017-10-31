using Online_SHopping_Cart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_SHopping_Cart.ViewModel
{
    public class Service_ViewModel
    {

        public List<Location_Table> locationList { get; set; }
        public List<BaseCategory_Table> baseProductList { get; set; }
        public int selectedLocation { get; set; }
        public string serviceName { get; set; }
        public decimal deliveryCharge { get; set; }
        public string serviceDescription { get; set; }
        public int productCatId { get; set; }

    }
}