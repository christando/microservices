using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.Model
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = null!;

    }
}