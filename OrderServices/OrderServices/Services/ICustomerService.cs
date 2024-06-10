using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.Model;

namespace OrderServices.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customers>> GetAllCustomer();
        Task<Customers> GetCustomerByUsername(string username);
    }
}