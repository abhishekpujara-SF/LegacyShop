using System.Collections.Generic;
using System.Linq;
using Dapper;
using LegacyShop.Web.Models;

namespace LegacyShop.Web.Data
{
    /// <summary>Products and categories via Dapper 1.x.</summary>
    public class ProductRepository
    {
        private const string SelectSql =
            "SELECT p.ProductId, p.CategoryId, c.Name AS CategoryName, p.Sku, p.Name, p.Price, p.Stock, p.IsActive " +
            "FROM dbo.Products p JOIN dbo.Categories c ON c.CategoryId = p.CategoryId ";

        public List<Category> GetCategories()
        {
            using (var conn = Db.Open())
                return conn.Query<Category>("SELECT CategoryId, Name FROM dbo.Categories ORDER BY Name").ToList();
        }

        public List<Product> GetAll(int? categoryId)
        {
            using (var conn = Db.Open())
            {
                if (categoryId.HasValue)
                    return conn.Query<Product>(SelectSql + "WHERE p.CategoryId = @categoryId ORDER BY p.Name",
                        new { categoryId = categoryId.Value }).ToList();
                return conn.Query<Product>(SelectSql + "ORDER BY p.Name").ToList();
            }
        }

        public Product GetById(int id)
        {
            using (var conn = Db.Open())
                return conn.Query<Product>(SelectSql + "WHERE p.ProductId = @id", new { id }).FirstOrDefault();
        }

        public List<Product> GetLowStock(int threshold)
        {
            using (var conn = Db.Open())
                return conn.Query<Product>(SelectSql + "WHERE p.IsActive = 1 AND p.Stock <= @threshold ORDER BY p.Stock",
                    new { threshold }).ToList();
        }

        public int Save(Product p)
        {
            using (var conn = Db.Open())
            {
                if (p.ProductId == 0)
                {
                    return conn.Query<int>(
                        "INSERT dbo.Products(CategoryId,Sku,Name,Price,Stock,IsActive) " +
                        "VALUES(@CategoryId,@Sku,@Name,@Price,@Stock,@IsActive); SELECT CAST(SCOPE_IDENTITY() AS INT)", p).Single();
                }
                conn.Execute(
                    "UPDATE dbo.Products SET CategoryId=@CategoryId, Sku=@Sku, Name=@Name, Price=@Price, " +
                    "Stock=@Stock, IsActive=@IsActive WHERE ProductId=@ProductId", p);
                return p.ProductId;
            }
        }

        public void Delete(int id)
        {
            using (var conn = Db.Open())
                conn.Execute("DELETE FROM dbo.Products WHERE ProductId = @id", new { id });
        }

        public int Count()
        {
            using (var conn = Db.Open())
                return conn.ExecuteScalar<int>("SELECT COUNT(*) FROM dbo.Products");
        }
    }
}
