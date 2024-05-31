using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrderServices.DTO
{
    public class OrderHeaderUpdate
    {
        public int CustomerId { get; set; }
        public DateTime OrderDate { get; set; }
        public string Username {get;set;}
    }
}