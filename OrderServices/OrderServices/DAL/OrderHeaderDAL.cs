using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using OrderServices.DAL.Interfaces;
using OrderServices.Model;
using Dapper;

namespace OrderServices.DAL
{
    public class OrderHeaderDAL : IOrderHeaders
    {
        private readonly IConfiguration _config;

        public OrderHeaderDAL(IConfiguration config)
        {
            _config = config;
        }

        private string GetConnectionString()
        {
            return "Data Source=.\\SQLEXPRESS;Initial Catalog=OrderDb;Integrated Security=True";
        }
        public void Delete(int id)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"DELETE FROM OrderHeader WHERE OrderHeaderId = @OrderHeaderId";
                var param = new {OrderHeaderId = id};
                try{
                    conn.Execute(strsql, param);
                    
                }
                catch(SqlException sqlex){
                    throw new ArgumentException($"Error:  {sqlex.Message} - {sqlex.Number}");
                }
                catch(Exception ex){
                    throw new ArgumentException($"Error: {ex.Message}");
                }
            }
        }

        public IEnumerable<OrderHeader> GetAll()
        {
            List<OrderHeader> orderHeaders = new List<OrderHeader>();
            using(SqlConnection conn = new SqlConnection(GetConnectionString())){
                var strsql = @"SELECT * FROM OrderHeader order by OrderDate";
                SqlCommand cmd = new SqlCommand(strsql, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if(dr.HasRows){
                    while(dr.Read()){
                        OrderHeader orderHeader = new OrderHeader();
                        orderHeader.OrderHeaderId = Convert.ToInt32(dr["OrderHeaderId"]);
                        orderHeader.CustomerId= Convert.ToInt32(dr["CustomerId"]);
                        orderHeader.OrderDate = Convert.ToDateTime(dr["OrderDate"]);
                        orderHeader.Username = dr["Username"].ToString();
                        orderHeaders.Add(orderHeader);
                    }
                }
                dr.Close();
                cmd.Dispose();
                conn.Close();

            }
            return orderHeaders;
        }

        public OrderHeader GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"SELECT * FROM OrderHeader WHERE OrderHeaderId = @OrderHeaderId";
                var param = new { OrderHeaderId = id };
                var orderheader = conn.QueryFirstOrDefault<OrderHeader>(strsql, param);
                if(orderheader == null)
                {
                    throw new ArgumentException("Data tidak ditemukan");
                }
                return orderheader;
            }
        }

        public OrderHeader Insert(OrderHeader obj)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString())){
                var strsql = @"INSERT INTO OrderHeader(OrderDate, CustomerId, Username) VALUES(@OrderDate, @CustomerId, @Username);select @@IDENTITY;";
                var param = new { OrderDate = obj.OrderDate, CustomerId = obj.CustomerId, Username = obj.Username};
                try{
                    var id = conn.ExecuteScalar<int>(strsql, param);
                    obj.OrderHeaderId = id;
                    return obj;
                }
                catch(SqlException sqlex){
                    throw new ArgumentException($"Error:  {sqlex.Message} - {sqlex.Number}");
                }
                
            }
        }
        public void Update(OrderHeader obj)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString())){
                var strsql = @"UPDATE OrderHeader SET OrderDate = @OrderDate, CustomerId = @CustomerId, Username = @Username WHERE OrderHeaderId = @OrderHeaderId";
                SqlCommand cmd = new SqlCommand(strsql, conn);
                var param = new {
                    OrderHeaderId = obj.OrderHeaderId,
                    CustomerId = obj.CustomerId,
                    OrderDate = obj.OrderDate,
                    Username = obj.Username
                };
                try{
                    conn.Execute(strsql, param);
                    
                }
                catch(SqlException sqlex){
                    throw new ArgumentException($"Error:  {sqlex.Message} - {sqlex.Number}");
                }
                catch(Exception ex){
                    throw new ArgumentException($"Error: {ex.Message}");
                }
                finally{
                    cmd.Dispose();
                    conn.Close();
                }
            }
        }
        IEnumerable<OrderHeader> IOrderHeaders.GetByDate(string date)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString())){
                var strsql = @"SELECT * FROM OrderHeader WHERE OrderDate = @OrderDate";
                var param = new { OrderDate = date };
                var orderheader = conn.Query<OrderHeader>(strsql, param);
                return orderheader;
            }
        }
    }
}