using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.Model;

namespace OrderServices.DAL.Interfaces
{
    public interface ICustomers : ICrud<Customer>
    {
        IEnumerable<Customer> GetByName(string name);
    }    
}



