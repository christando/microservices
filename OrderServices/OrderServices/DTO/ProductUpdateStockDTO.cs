using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.DTO
{
    public class ProductUpdateStockDTO
    {
        public int productID { get; set; }
        public int quantity { get; set; }
    }
}