using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WalletServices.DAL.Interfaces;
using WalletServices.Models;
using Dapper;
using System.Data.SqlClient;
using WalletServices.DTO;
using System.Text;

namespace WalletServices.DAL
{
    public class WalletDAL : IWallet
    {
        private readonly IConfiguration _config;
        public WalletDAL(IConfiguration config)
        {
            _config = config;
        }
        private string GetConnectionString()
        {
            
            return "Data Source=.\\SQLEXPRESS;Initial Catalog=WalletDb;Integrated Security=True";
        }
        public void Delete(int id)
        {
            using(var conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"DELETE FROM Wallet WHERE WalletId = @WalletId";
                var param = new {WalletId = id};
                conn.Execute(strsql, param);
            }
        }

        public IEnumerable<Wallet> GetAll()
        {
            using(var conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"SELECT * FROM Wallet order by WalletId";
                return conn.Query<Wallet>(strsql);
            }
        }

        public Wallet GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Wallet> GetByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        public Wallet Insert(Wallet obj)
        {
            using(var conn = new SqlConnection(GetConnectionString()))
            {
                string strpass = encryptpass(obj.Password);
                var strsql = @"INSERT INTO Wallet (Username, Password, FullName, Saldo, PaymentWallet) VALUES (@Username, @Password, @FullName, @Saldo, @PaymentWallet)";
                var param = new {Username = obj.Username, Password = strpass, FullName = obj.FullName, Saldo = obj.Saldo, PaymentWallet = obj.PaymentWallet};
                try {
                    conn.Execute(strsql, param);
                    return obj;
                }
                catch(SqlException sqlEx)
                {
                    throw new ArgumentException($"Error : {sqlEx.Message} - {sqlEx.Number}");
                }
                catch(Exception Ex)
                {
                    throw new ArgumentException($"Error : {Ex.Message}");
                }
            }
        }

        public void Update(Wallet obj)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Wallet> TopUpSaldo(string PaymentWallet, string Username,decimal saldo)
        {
            using(var conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"UPDATE Wallet SET Saldo = Saldo + @Saldo WHERE PaymentWallet = @PaymentWallet AND Username = @Username";
                var param = new {Saldo = saldo, PaymentWallet = PaymentWallet, Username = Username};
                try {
                    conn.Execute(strsql, param);
                    return GetAll();
                }
                catch(SqlException sqlEx)
                {
                    throw new ArgumentException($"Error : {sqlEx.Message} - {sqlEx.Number}");
                }
                catch(Exception Ex)
                {
                    throw new ArgumentException($"Error : {Ex.Message}");
                }
            }
        }

        public void UpdateSaldoAfterOrder(WalletUpdateSaldoDTO walletUpdateSaldoDTO)
        {
            var strsql = @"UPDATE Wallet SET Saldo = Saldo - @Saldo WHERE Username = @Username AND Password = @Password AND PaymentWallet = @PaymentWallet";
            using(SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var param = new {Saldo = walletUpdateSaldoDTO.saldo, Username = walletUpdateSaldoDTO.username, Password = walletUpdateSaldoDTO.password, PaymentWallet = walletUpdateSaldoDTO.paymentWallet};
                try
                {
                    conn.Execute(strsql, param);
                }catch(SqlException sqlEx)
                {
                    throw new ArgumentException($"Error : {sqlEx.Message} - {sqlEx.Number}");
                }
                catch(Exception Ex)
                {
                    throw new ArgumentException($"Error : {Ex.Message}");
                }
            }
        }

        public IEnumerable<Wallet> GetByUsername(string username)
        {
            using(var conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"SELECT * FROM Wallet WHERE Username = @Username";
                var param = new {Username = username};
                return conn.Query<Wallet>(strsql, param);
            }
        }
        public string encryptpass(string password)
        {
            string msg = "";
            byte[] encode = new byte[password.Length];
            encode = Encoding.UTF8.GetBytes(password);
            msg = Convert.ToBase64String(encode);
            return msg;
        }
    }
}