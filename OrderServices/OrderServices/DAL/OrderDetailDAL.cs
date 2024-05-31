using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using OrderServices.Model;

namespace OrderServices.DAL
{
    public class OrderDetailDAL : IOrderDetails
    {

        private readonly IConfiguration _config;
        public OrderDetailDAL(IConfiguration config)
        {
            _config = config;
        }
        private string GetConnectionString()
        {
            return "Data Source=.\\SQLEXPRESS;Initial Catalog=OrderDb;Integrated Security=True";
        }
        public void Delete(int id)
        {
            using(var conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"DELETE FROM OrderDetails WHERE OrderDetailId = @OrderDetailId";
                var param = new {OrderDetailId = id};
                try{
                    conn.Execute(strsql, param);
                }
                catch(SqlException sqlex){
                    throw new ArgumentException($"Error:  {sqlex.Message} - {sqlex.Number}");
                }
            }
        }

        public IEnumerable<OrderDetail> GetAll()
        {
            using(var conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"SELECT * FROM OrderDetails order by OrderHeaderId";
                return conn.Query<OrderDetail>(strsql);
            }
        }

        public OrderDetail GetById(int id)
        {
            using(var conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"SELECT * FROM OrderDetails WHERE OrderDetailId = @OrderDetailId";
                var param = new {OrderDetailId = id};
                return conn.QueryFirstOrDefault<OrderDetail>(strsql, param);
            }
        }

        public IEnumerable<OrderDetail> GetByOrderId(int orderId)
        {
            throw new NotImplementedException();
        }

        public OrderDetail Insert(OrderDetail obj)
        {
            using(var conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"INSERT INTO OrderDetails (OrderHeaderId, ProductId, Quantity, Price, Username) VALUES (@OrderHeaderId, @ProductId, @Quantity, @Price, @Username);
                select @@IDENTITY;";
                var param = new {OrderHeaderId = obj.OrderHeaderId, ProductId = obj.ProductId, Quantity = obj.Quantity, Price = obj.Price, Username = obj.Username};
                try{
                    var id = conn.ExecuteScalar<int>(strsql, param);
                    obj.OrderDetailId = id;
                    return obj;
                }
                catch(SqlException sqlex){
                    throw new ArgumentException($"Error:  {sqlex.Message} - {sqlex.Number}");
                }
            }
        }

        public void Update(OrderDetail obj)
        {
            using(var conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"UPDATE OrderDetails SET OrderHeaderId = @OrderHeaderId, ProductId = @ProductId, Quantity = @Quantity, Price = @Price WHERE OrderDetailId = @OrderDetailId";
                var param = new {OrderHeaderId = obj.OrderHeaderId, ProductId = obj.ProductId, Quantity = obj.Quantity, Price = obj.Price, OrderDetailId = obj.OrderDetailId};
                try{
                    conn.Execute(strsql, param);
                }
                catch(SqlException sqlex){
                    throw new ArgumentException($"Error:  {sqlex.Message} - {sqlex.Number}");
                }
            }
        }

        public IEnumerable<OrderDetail> GetByName(string name)
        {
            throw new NotImplementedException();
        }
    }
}