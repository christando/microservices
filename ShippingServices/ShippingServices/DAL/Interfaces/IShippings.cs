using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingServices.Models;

namespace ShippingServices.DAL.Interfaces
{
    public interface IShippings : ICrud<Shippings>
    {
        IEnumerable<Shippings> GetById(int Id);
    }
}