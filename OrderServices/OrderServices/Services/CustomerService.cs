using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.Model;
using System.Text.Json;
using System.Text;

namespace OrderServices.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly HttpClient _httpClient;

        public CustomerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7005");
        }

        public async Task<IEnumerable<Customers>> GetAllCustomer()
        {
            var response = await _httpClient.GetAsync("/API/Customer");
            if(response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var customer = JsonSerializer.Deserialize<IEnumerable<Customers>>(results);
                return customer;
            }
            else
            {
                throw new ArgumentException("Cannot Get customer - HttpStatusCode: " + response.StatusCode);
            }
        }

        public async Task<Customers> GetCustomerByUsername(string username)
        {
            var response = await _httpClient.GetAsync($"/API/Customer/GetByUsername/{username}");
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var customer = JsonSerializer.Deserialize<List<Customers>>(results);
                if(customer == null)
                {
                    throw new ArgumentException("Cannot get found");
                }
                return customer.First();
            }
            else
            {
                throw new ArgumentException("Cannot Get customer - HttpStatusCode: " + response.StatusCode);
            }
        }
    }
}