using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShippingServices.DTO;
using ShippingServices.Models;

namespace ShippingServices.Services
{
    public interface IWalletService
    {
        Task<IEnumerable<Wallet>> GetAllWallet();
        Task<Wallet> GetWalletByUsername(string username);
        Task UpdateSaldo(WalletUpdateSaldoDTO walletUpdateSaldoDTO);
    }
}