using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WalletServices.DTO
{
    public class WalletUpdateSaldoDTO
    {
        public string username { get; set; }
        public string password { get; set; }
        public decimal saldo { get; set; }
        public string paymentWallet { get; set; }
    }
}