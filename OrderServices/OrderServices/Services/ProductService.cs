using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using OrderServices.Model;
using OrderServices.DTO;
using System.Text;

namespace OrderServices.Services
{
    public class ProductService : IProductService
    {
        private readonly HttpClient _httpClient;
        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7001");
        }

        public async Task UpdateProductStock(ProductUpdateStockDTO productUpdateStockDTO)
        {
            var Json = JsonSerializer.Serialize(productUpdateStockDTO);
            var data = new StringContent(Json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("/API/product/UpdateStock", data);
            if(!response.IsSuccessStatusCode)
            {
                throw new ArgumentException("Cannot Update product - HttpStatusCode: " + response.StatusCode);
            }
        }

        async Task<IEnumerable<Product>> IProductService.GetAllProducts()
        {
            var response = await _httpClient.GetAsync("/api/products");
            if(response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<IEnumerable<Product>>(results);
                return product;
            }
            else
            {
                throw new ArgumentException("Cannot Get product - HttpStatusCode: " + response.StatusCode);
            }
        }

        async Task<Product> IProductService.GetProductById(int id)
        {
            var response = await _httpClient.GetAsync($"API/product/GetById/{id}");
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<Product>(results);
                if(product == null)
                {
                    throw new ArgumentException("Cannot get found");
                }
                return product;
            }
            else
            {
                throw new ArgumentException("Cannot Get product - HttpStatusCode: " + response.StatusCode);
            }
        }
    }
}


