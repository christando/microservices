using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CustomerServices.models;

namespace CustomerServices.DAL.interfaces
{
    public interface ICustomer : ICrud<Customer>
    {
        IEnumerable<Customer> GetByUsername(string username);
    }
}