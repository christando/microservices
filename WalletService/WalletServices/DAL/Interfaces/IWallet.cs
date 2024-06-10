using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletServices.DTO;
using WalletServices.Models;

namespace WalletServices.DAL.Interfaces
{
    public interface IWallet : ICrud<Wallet>
    {
        IEnumerable<Wallet> GetByUserId(int userId);
        IEnumerable<Wallet> GetByUsername(string username);
        IEnumerable<Wallet> TopUpSaldo(string PaymentWallet, string Username,decimal saldo);
        void UpdateSaldoAfterOrder(WalletUpdateSaldoDTO walletUpdateSaldoDTO);
    }
}