namespace CatalogServices;

public interface IProduct : ICrud<Product>
{
    IEnumerable<Product> GetByName(string name);
    IEnumerable<VProduct> GetByCategory(string name);
    void UpdateStockAfterOrder(ProductUpdateStockDTO productUpdateStockDTO);
}
