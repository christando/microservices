using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using ShippingServices.DAL.Interfaces;
using ShippingServices.Models;

namespace ShippingServices.DAL
{
    public class ShippingsDAL : IShippings
    {
        private readonly IConfiguration _config;
        public ShippingsDAL(IConfiguration config)
        {
            _config = config;
        }
        private string GetConnectionString()
        {
            
            return "Data Source=.\\SQLEXPRESS;Initial Catalog=ShippingDb;Integrated Security=True";
        }
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Shippings> GetAll()
        {
            using (var connection = new SqlConnection(GetConnectionString())){
                var strsql = "SELECT * FROM Shipping order by ShippingID";
                return connection.Query<Shippings>(strsql);
            }
        }

        public IEnumerable<Shippings> GetById(int Id)
        {
            throw new NotImplementedException();
        }

        public Shippings Insert(Shippings obj)
        {
            using(var connection = new SqlConnection(GetConnectionString())){
                var strSql = "INSERT INTO Shipping (ShippingVendor, ShippingDate, ShippingStatus, OrderHeaderID, BeratBarang, BiayaShipping) VALUES (@ShippingVendor, @ShippingDate, @ShippingStatus, @OrderHeaderID, @BeratBarang, @BiayaShipping); SELECT CAST(SCOPE_IDENTITY() as int)";
                var param = new {ShippingVendor = obj.ShippingVendor, ShippingDate = obj.ShippingDate, ShippingStatus = obj.ShippingStatus, OrderHeaderID = obj.OrderHeaderID, BeratBarang = obj.BeratBarang, BiayaShipping = obj.BiayaShipping};
                try{
                    var id = connection.QuerySingle<int>(strSql, param);
                    obj.ShippingID = id;
                    return obj;
                }catch(SqlException sqlEx){
                    throw new Exception($"Error: {sqlEx.Message}");
                }catch(Exception ex){
                    throw new Exception($"Error: {ex.Message}");
                }
        }
        }
        public void Update(Shippings obj)
        {
            using(var connection = new SqlConnection(GetConnectionString())){
                var strSql = "UPDATE Shipping SET ShippingStatus = @ShippingStatus WHERE ShippingID = @ShippingID";
                var param = new {ShippingStatus = obj.ShippingStatus, ShippingID = obj.ShippingID};
                try{
                    connection.Execute(strSql, param);
                }catch(SqlException sqlEx){
                    throw new Exception($"Error: {sqlEx.Message}");
                }catch(Exception ex){
                    throw new Exception($"Error: {ex.Message}");
                }
            }
        }

        Shippings ICrud<Shippings>.GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}