using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CatalogServices.DAL.Interfaces;
using CatalogServices.Models;
using System.Data.SqlClient;

namespace CatalogServices.DAL
{
    
    public class CategoryDAL : ICategory
    {
        private readonly IConfiguration _config;

        public CategoryDAL(IConfiguration config){
            _config = config;
        }
        private string GetConnectionString()
        {
            // return _config.GetConnectionString("DefaultConnection");
            return "Data Source=.\\SQLEXPRESS;Initial Catalog=CatalogDb;Integrated Security=True";
            // return "Data Source=.\\SQLEXPRESS;Initial Catalog=CatalogDb;Integrated Security=True;TrustServerCertificate=True";
        }
        public void Delete(int id)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"DELETE FROM Categories WHERE CategoryID = @CategoryID";
                SqlCommand cmd = new SqlCommand(strSql, conn);
                cmd.Parameters.AddWithValue("@CategoryID", id);
                try
                {
                    conn.Open();
                    var results = cmd.ExecuteNonQuery();
                    if(results!= 1)
                    {
                        throw new ArgumentException("Data Gagal dihapus");
                    }
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                }
            }
        }

        public IEnumerable<Category> GetAll()
        {
            // throw new NotImplementedException();
            List<Category> categories = new List<Category>();
            using(SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT * FROM Categories order by CategoryName";
                SqlCommand cmd = new SqlCommand(strSql, conn);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if(dr.HasRows){
                    while(dr.Read())
                    {
                        Category category = new Category();
                        category.CategoryID = Convert.ToInt32(dr["CategoryID"]);
                        category.CategoryName = dr["CategoryName"].ToString();
                        categories.Add(category);
                    }
                }
                dr.Close();
                cmd.Dispose();
                conn.Close();

                return categories;
            }
        }

        public Category GetById(int id)
        {
            Category category = new Category();
            using(SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT * FROM Categories WHERE CategoryID = @CategoryID";
                SqlCommand cmd = new SqlCommand(strSql, conn);
                cmd.Parameters.AddWithValue("@CategoryID", id);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                if(dr.HasRows){
                    dr.Read();
                    category.CategoryID = Convert.ToInt32(dr["CategoryID"]);
                    category.CategoryName = dr["CategoryName"].ToString();
                }
                dr.Close();
                cmd.Dispose();
                conn.Close();

                return category;
            }
        }

        public IEnumerable<Category> GetByName(string name)
        {
            List<Category> categories = new List<Category>();
            using(SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"SELECT * FROM Categories WHERE CategoryName = @CategoryName";
                SqlCommand cmd = new SqlCommand(strSql, conn);
                cmd.Parameters.AddWithValue("@CategoryName", name);
                conn.Open();
                SqlDataReader dr = cmd.ExecuteReader();
                while(dr.Read())
                {
                    Category category = new Category();
                    category.CategoryID = Convert.ToInt32(dr["CategoryID"]);
                    category.CategoryName = dr["CategoryName"].ToString();
                    categories.Add(category);
                }
                    dr.Close();
                    cmd.Dispose();
                    conn.Close();
            }
                    return categories;
        }

        public void Insert(Category obj)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"INSERT INTO Categories (CategoryName) VALUES (@CategoryName)";
                SqlCommand cmd = new SqlCommand(strSql, conn);
                cmd.Parameters.AddWithValue("@CategoryName", obj.CategoryName);

                try
                {
                    conn.Open();
                    var results = cmd.ExecuteNonQuery();
                    if(results!= 1)
                    {
                        throw new ArgumentException("Data Gagal ditambahkan");
                    }
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                }
            }
        }

        public void Update(Category obj)
        {
            using(SqlConnection conn = new SqlConnection(GetConnectionString()))
            {
                var strSql = @"UPDATE Categories SET CategoryName = @CategoryName WHERE CategoryID = @CategoryID";
                SqlCommand cmd = new SqlCommand(strSql, conn);
                cmd.Parameters.AddWithValue("@CategoryName", obj.CategoryName);
                cmd.Parameters.AddWithValue("@CategoryID", obj.CategoryID);

                try
                {
                    conn.Open();
                    var results = cmd.ExecuteNonQuery();
                    if(results!= 1)
                    {
                        throw new ArgumentException("Data Gagal diupdate");
                    }
                }
                catch (SqlException sqlEx)
                {
                    throw new ArgumentException($"Error: {sqlEx.Message} - {sqlEx.Number}");
                }
                catch (Exception ex)
                {
                    throw new ArgumentException($"Error: {ex.Message}");
                }
                finally
                {
                    cmd.Dispose();
                    conn.Close();
                }
            }
        }
    }
}