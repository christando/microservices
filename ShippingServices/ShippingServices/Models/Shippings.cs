using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShippingServices.Models
{
    public class Shippings
    {
        public int ShippingID { get; set; }
        public string ShippingVendor { get; set; }
        public DateTime ShippingDate { get; set; }
        public string ShippingStatus { get; set; }
        public int OrderHeaderID { get; set; }
        public decimal BeratBarang { get; set; }
        public decimal BiayaShipping { get; set; }
        public string Username { get; set; }
        public string password { get; set; }
        public string PaymentWallet { get; set; }
    }
}