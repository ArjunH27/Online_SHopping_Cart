using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Online_SHopping_Cart.ViewModel
{
    public class Book
    {
        public int UserId { get; set; }
        public int Locationid { get; set; }
        public string ServiceName { get; set; }
        public decimal DeliveryCharge { get; set; }
        public int ServiceId { get; set; }
        public int ProductId { get; set; }
        public int ProductCatid { get; set; }
        public int SellerId { get; set; }
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string ProductDesc { get; set; }
        public int ProductStock { get; set; }
        public byte[] BinaryImage { get; set; }
        public string OrderDelivryAddress { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public Nullable<decimal> TotalAmount { get; set; }
        public string UserName { get; set; }
    }
}