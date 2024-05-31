using System.Data.SqlClient;
using CatalogServices.Models;
using Dapper;

namespace CatalogServices;

public class ProductDapper : IProduct
{
    
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
            var strSql = @"DELETE FROM Products WHERE ProductID = @ProductID";
            var param = new {ProductID = id};
            try
            {
                conn.Execute(strSql, param);
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

    public IEnumerable<Product> GetAll()
    {
        using(SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            var strSql = @"SELECT * FROM Products order by Name";
            var products = conn.Query<Product>(strSql);
            return products;
        }
    }

    public Product GetById(int id)
    {
        using(SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            var strSql = @"SELECT * FROM Products WHERE ProductID = @ProductID";
            var param = new{ProductID = id};
            var product = conn.QueryFirstOrDefault<Product>(strSql, param);
            if (product == null){
                throw new ArgumentException("Data tidak di temukan");
            }
            return product;
        }
    }

    public IEnumerable<Product> GetByName(string name)
    {
        using(SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
           var strSql = @"SELECT * FROM Products WHERE Name LIKE @Name";
            var param = new{Name = $"%{name}%"};
            var products = conn.Query<Product>(strSql, param);
            return products;
        }
    }

    public IEnumerable<VProduct> GetByCategory(string name)
    {
        using(SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
        //    var strSql = @"SELECT * FROM View_prod_category WHERE CategoryName LIKE @Name";
           var strSql = @"SELECT dbo.Categories.CategoryName, dbo.Products.ProductID, dbo.Products.Name, dbo.Products.Description, dbo.Products.Price, dbo.Products.Quantity
FROM     dbo.Categories INNER JOIN
                  dbo.Products ON dbo.Categories.CategoryID = dbo.Products.CategoryID WHERE CategoryName LIKE @Name";
           var param = new{Name = $"%{name}%"};
           var products = conn.Query<VProduct>(strSql, param);
           return products;
        } 
    }

    public void Insert(Product obj)
    {
        using(SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            var strSql = @"INSERT INTO Products (CategoryID, Name, Description, Price, Quantity) VALUES (@CategoryID, @Name, @Description, @Price, @Quantity)";
            var param = new {Name = obj.Name, CategoryID = obj.CategoryID, Description = obj.Description, Price = obj.Price, Quantity = obj.Quantity};
            try
            {
                conn.Execute(strSql, param);
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

    public void Update(Product obj)
    {
        using(SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            var strSql = @"UPDATE Products SET CategoryID=@CategoryID, Name=@Name, Description=@Description, Price=@Price, Quantity=@Quantity WHERE ProductID = @ProductID";
            var param = new {CategoryID = obj.CategoryID, Name = obj.Name, Description = obj.Description, Price = obj.Price, Quantity = obj.Quantity, ProductID = obj.ProductID};
            try
            {
                conn.Execute(strSql, param);
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

    public void UpdateStockAfterOrder(ProductUpdateStockDTO productUpdateStockDTO)
    {
        var StrSql = @"UPDATE Products SET Quantity = Quantity - @Quantity WHERE ProductID = @ProductID";
        using (SqlConnection conn = new SqlConnection(GetConnectionString()))
        {
            var param = new {ProductID = productUpdateStockDTO.productID, Quantity = productUpdateStockDTO.quantity};
            try{
                conn.ExecuteAsync(StrSql, param);
            }catch (SqlException sqlEx)
            {
                throw new ArgumentException($"Error : {sqlEx.Message} - {sqlEx.Number}");
            }
            catch (Exception Ex)
            {
                throw new ArgumentException($"Error : {Ex.Message}");

            }
        }
    }
}
