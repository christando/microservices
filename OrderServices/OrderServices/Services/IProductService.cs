using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using OrderServices.Model;
using OrderServices.DTO;


namespace OrderServices.Services{
public interface IProductService
{
    Task<IEnumerable<Product>> GetAllProducts();
    Task<Product> GetProductById(int id);
    Task UpdateProductStock(ProductUpdateStockDTO productUpdateStockDTO);
}
}
