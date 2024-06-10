using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using CustomerServices.DAL.interfaces;
using CustomerServices.models;
using Dapper;

namespace CustomerServices.DAL
{
    public class CustomerDAL : ICustomer
    {

        private readonly IConfiguration _config;
        public CustomerDAL(IConfiguration config)
        {
            _config = config;
        }
        private string GetConnectionString()
        {
            return "Data Source=.\\SQLEXPRESS;Initial Catalog=CustomersDb;Integrated Security=True";
        }
        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Customer> GetAll()
        {
            List<Customer> customers = new List<Customer>();
            using(SqlConnection conn = new SqlConnection(GetConnectionString())){
                var strsql = @"SELECT * FROM Customer order by FullName";
                SqlCommand cmd = new SqlCommand(strsql, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if(dr.HasRows){
                    while(dr.Read()){
                        Customer customer = new Customer();
                        customer.CustomerID = Convert.ToInt32(dr["CustomerID"]);
                        customer.FullName = dr["FullName"].ToString();
                        customer.Username = dr["Username"].ToString();
                        customer.Password = dr["Password"].ToString();
                        customers.Add(customer);
                    }
                }
                return customers;
            }
        }

        public Customer GetById(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Customer> GetByUsername(string username)
        {
            var strsql = @"SELECT * FROM Customer WHERE Username = @Username";
            using(SqlConnection conn = new SqlConnection(GetConnectionString())){
                var param = new {Username = username};
                return conn.Query<Customer>(strsql, param);
            }
        }

        public Customer Insert(Customer obj)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString())){
                var strsql = @"INSERT INTO Customer(FullName, Username, Password) VALUES(@FullName, @Username, @Password); SELECT CAST(SCOPE_IDENTITY() as int)";
                var param = new {FullName = obj.FullName, Username = obj.Username, Password = obj.Password};
                try{
                    conn.Open();
                    obj.CustomerID = conn.Query<int>(strsql, param).Single();
                }
                catch(SqlException sqlex){
                    throw new ArgumentException($"Error:  {sqlex.Message} - {sqlex.Number}");
                }
                catch(Exception ex){
                    throw new ArgumentException($"Error: {ex.Message}");
                }
                return obj;
            }
        }

        public void Update(Customer obj)
        {
            throw new NotImplementedException();
        }
    }
}