using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using OrderServices.Model;
using OrderServices.DAL.Interfaces;
using Dapper;


namespace OrderServices.DAL
{
    public class CustomerDAL : ICustomers
    {
        private readonly IConfiguration _config;

        public CustomerDAL(IConfiguration config)
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
                var strsql = @"DELETE FROM Customers WHERE CustomerId = @CustomerId";
                var param = new {CustomerId = id};
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

        public IEnumerable<Customer> GetAll()
        {
            List<Customer> customers = new List<Customer>();
            using(SqlConnection conn = new SqlConnection(GetConnectionString())){
                var strsql = @"SELECT * FROM Customers order by CustomerName";
                SqlCommand cmd = new SqlCommand(strsql, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if(dr.HasRows){
                    while(dr.Read()){
                        Customer customer = new Customer();
                        customer.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                        customer.CustomerName = dr["CustomerName"].ToString();
                        customers.Add(customer);
                    }
                }
                dr.Close();
                cmd.Dispose();
                conn.Close();
            }
            return customers;
        }

        public Customer Insert(Customer obj)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"INSERT INTO Customers (CustomerName) VALUES(@CustomerName); SELECT @@IDENTITY;";
                try{
                    var NewId = conn.ExecuteScalar<int>(strsql, 
                    new {CustomerName = obj.CustomerName});
                    obj.CustomerId = NewId;
                    return obj;
                }catch(SqlException sqlex){
                    throw new ArgumentException($"Error:  {sqlex.Message} - {sqlex.Number}");
                }
            }
        }

        public Customer GetById(int id)
        {
            using (SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"SELECT * FROM Customers WHERE CustomerId = @CustomerId";
                SqlCommand cmd = new SqlCommand(strsql, conn);
                cmd.Parameters.AddWithValue("@CustomerId", id);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                Customer customer = new Customer();
                if(dr.HasRows){
                    while(dr.Read()){
                        customer.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                        customer.CustomerName = dr["CustomerName"].ToString();
                    }
                }
                dr.Close();
                cmd.Dispose();
                conn.Close();
                return customer;
            }
        }

        

        public void Update(Customer obj)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strsql = @"UPDATE Customers SET CustomerName = @CustomerName WHERE CustomerId = @CustomerId";
                SqlCommand cmd = new SqlCommand(strsql, conn);
                cmd.Parameters.AddWithValue("@CustomerName", obj.CustomerName);
                cmd.Parameters.AddWithValue("@CustomerId", obj.CustomerId);

                try{
                    conn.Open();
                    var results = cmd.ExecuteNonQuery();
                    if(results != 1){
                        throw new ArgumentException("Data gagal diupdate");
                    }
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

        public IEnumerable<Customer> GetByName(string name)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString())){
                var strsql = @"SELECT * FROM Customers WHERE CustomerName LIKE @CustomerName";
                SqlCommand cmd = new SqlCommand(strsql, conn);
                cmd.Parameters.AddWithValue("@CustomerName", "%" + name + "%");
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                List<Customer> customers = new List<Customer>();
                if(dr.HasRows){
                    while(dr.Read()){
                        Customer customer = new Customer();
                        customer.CustomerId = Convert.ToInt32(dr["CustomerId"]);
                        customer.CustomerName = dr["CustomerName"].ToString();
                        customers.Add(customer);
                    }
                }
                dr.Close();
                cmd.Dispose();
                conn.Close();
                return customers;
            }
        }

        
    }
}