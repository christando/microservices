namespace CatalogServices;

public class ProductByNameDTO
{
    public string? CategoryName { get; set; } 
    public string? Name {get;set;}
    public string? Description {get;set;}
    public decimal Price {get;set;}
    public int Quantity {get;set;}
}
