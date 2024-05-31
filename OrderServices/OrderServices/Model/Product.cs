using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.Model
{
    public class Product
    {
        public int productID {get;set;}
        public string name {get;set;}
        public string categoryName {get;set;}
        public decimal price {get;set;}
        public int quantity {get;set;}
    }
}