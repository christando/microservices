using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using OrderServices.DTO;
using OrderServices.Model;
using System.Text;

namespace OrderServices.Services
{
    public class WalletService : IWalletService
    {
        private readonly HttpClient _httpClient;

        public WalletService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7003");
        }
        async Task<IEnumerable<Wallet>> IWalletService.GetAllWallet()
        {
            var response = await _httpClient.GetAsync("/API/Wallet");
            if(response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var wallet = JsonSerializer.Deserialize<IEnumerable<Wallet>>(results);
                return wallet;
            }
            else
            {
                throw new ArgumentException("Cannot Get wallet - HttpStatusCode: " + response.StatusCode);
            }
        }

        async Task<Wallet> IWalletService.GetWalletByUsername(string username)
        {
            var response = await _httpClient.GetAsync($"/API/Wallet/GetByUsername/{username}");
            if (response.IsSuccessStatusCode)
            {
                var results = await response.Content.ReadAsStringAsync();
                var wallet = JsonSerializer.Deserialize<List<Wallet>>(results);
                if(wallet == null)
                {
                    throw new ArgumentException("Cannot get found");
                }
                return wallet.First();
            }
            else
            {
                throw new ArgumentException("Cannot Get wallet - HttpStatusCode: " + response.StatusCode);
            }
        }

        public async Task UpdateSaldo(WalletUpdateSaldoDTO walletUpdateSaldoDTO)
        {
            var json = JsonSerializer.Serialize(walletUpdateSaldoDTO);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync("/API/wallet/UpdateSaldoAfterOrder", data);
            
            if(!response.IsSuccessStatusCode)
            {
                throw new ArgumentException($"Cannot Update saldo - HttpStatusCode: {response.StatusCode}") ;
            }
        }
    }
}