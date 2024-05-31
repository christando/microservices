using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices.Models;

namespace CatalogServices.DAL.Interfaces
{
    public interface ICategory : ICrud<Category>
    {
        IEnumerable<Category> GetByName(string name);
    }
}