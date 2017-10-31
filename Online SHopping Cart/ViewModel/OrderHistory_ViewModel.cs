using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Online_SHopping_Cart.ViewModel
{
    public class OrderHistory_ViewModel
    {

        public byte[] BinaryImage { get; set; }
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public string OrderDelivryAddress { get; set; }
        public System.DateTime OrderDeliveryDate { get; set; }
        public decimal Amount { get; set; }
        public string ServiceName { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string CustomerName { get; set; }
    }
}