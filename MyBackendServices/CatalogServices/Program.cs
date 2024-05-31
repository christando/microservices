using System.ComponentModel;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using CatalogServices;
using CatalogServices.DAL.Interfaces;
using CatalogServices.DAL;
using System.Reflection.Metadata.Ecma335;
using CatalogServices.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICategory, CategoryDapper>();
builder.Services.AddScoped<IProduct, ProductDapper>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


// var Products = new List<Product>
// {
//     new Product { ProductID = 1, Name = "Apple", Description = "Red Apple", Price = 2.99M, CategoryID = 1, Quantity = 400},
//     new Product { ProductID = 2, Name = "Banana", Description = "Yellow Banana", Price = 3.99M, CategoryID = 1, Quantity = 400},
//     new Product { ProductID = 3, Name = "Orange", Description = "Orange Orange", Price = 3.99M, CategoryID = 1, Quantity = 400},
//     new Product { ProductID = 4, Name = "Pineapple", Description = "Sweet Pineapple", Price = 3.99M, CategoryID = 1, Quantity = 400},
//     new Product { ProductID = 5, Name = "Watermelon", Description = "Juicy Watermelon", Price = 3.99M, CategoryID = 1, Quantity = 400}
// };



// var Products = new[]
// {
//     "Milk", "Bread", "Banana"
// };

app.MapGet("/API/categories", (ICategory categoryDAL) =>
{
    List<CategoryDTO>categoriesDto = new List<CategoryDTO>();
    var categories = categoryDAL.GetAll();
    foreach(var category in categories)
    {
        categoriesDto.Add(new CategoryDTO
        {
            CategoryName = category.CategoryName
        });
    }
    return Results.Ok(categoriesDto);
   
    // var forecast =  Enumerable.Range(1, 5).Select(index =>
    //     new WeatherForecast
    //     (
    //         DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
    //         Random.Shared.Next(-20, 55),
    //         summaries[Random.Shared.Next(summaries.Length)]
    //     ))
    //     .ToArray();
    // return Products;
});

app.MapGet("/API/categories/GetById/{id}", (ICategory categoryDAL, int id ) =>
{
    CategoryDTO categoryDto = new CategoryDTO();
    var category = categoryDAL.GetById(id);
    if (category == null)
    {
        return Results.NotFound();
    }
    categoryDto.CategoryName = category.CategoryName;
    return Results.Ok(categoryDto);
});

app.MapGet("/API/categories/GetByName/{CategoryName}", (ICategory categoryDAL, string categoryName) =>
{
    List<CategoryDTO>categoriesDto = new List<CategoryDTO>();
    var categories = categoryDAL.GetByName(categoryName);
    foreach(var category in categories)
    {
        categoriesDto.Add(new CategoryDTO
        {
            CategoryName = category.CategoryName
        });
    }
    return Results.Ok(categoriesDto);
});

app.MapPost("/API/categories/Insert/", (ICategory categoryDAL, CategoryCreateDTO categoryCreateDto ) =>
{
    try{
         Category category = new Category
         {
            CategoryName = categoryCreateDto.CategoryName
        };
        categoryDAL.Insert(category);

        return Results.Created($"/API/category/Insert/{category.CategoryID}", category);

    }catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/API/category/Update",(ICategory categoryDAL, CategoryUpdateDTO categoryUpdateDto)=>
{
    try{
        var category = new Category
        {
            CategoryID= categoryUpdateDto.CategoryID,
            CategoryName = categoryUpdateDto.CategoryName
        };
        categoryDAL.Update(category);
        return Results.Ok();
    }
    catch(Exception ex){
        return Results.BadRequest(ex.Message);
    }
});

app.MapDelete("/API/categories/{id}",(ICategory categoryDAL, int id)=>{
    try
    {
        categoryDAL.Delete(id);
        return Results.Ok();
    }catch(Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

// app.MapGet("/API/products/{index}", (int index) =>
// {

//     // if(index < 0 && index>= Products.Length){
//     //     return Results.NotFound();
//     // }
//     // return Results.Ok(Products[index]);

//     // var product = Products.FirstOrDefault(p => p.ProductID == index);
//     // return Results.Ok(product);

//     // var forecast =  Enumerable.Range(1, 5).Select(index =>
//     //     new WeatherForecast
//     //     (
//     //         DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//     //         Random.Shared.Next(-20, 55),
//     //         summaries[Random.Shared.Next(summaries.Length)]
//     //     ))
//     //     .ToArray();
//     // return Products;
// });
// .WithName("GetWeatherForecast")
// .WithOpenApi();

// Contoh penggunaan Query String parameter
// app.MapGet("/API/Products/getbyname", (HttpRequest request)=>
// {
//     var name = request.Query["name"].ToString().ToLower();
//     var results = Products.Where(p => p.Name.ToLower().Contains(name.ToLower()));
//     return Results.Ok(results);
// });

// app.MapGet("/API/Products/getbyname", (string name)=>
// {
//     var results = Products.Where(p => p.Name.ToLower().Contains(name.ToLower()));
//     return Results.Ok(results);
// });

app.MapGet("/api/products", (IProduct productDAL) =>
{
    List<ProductDTO>ProductsDto = new List<ProductDTO>();
    var products = productDAL.GetAll();
    foreach(var product in products)
    {
        ProductsDto.Add(new ProductDTO
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity
        });
    }
    return Results.Ok(ProductsDto);
});

app.MapGet("/API/product/GetById/{id}", (IProduct productDAL, int id ) =>
{
    ProductDTO productDto = new ProductDTO();
    var product = productDAL.GetById(id);
    if (product == null)
    {
        return Results.NotFound();
    }
    productDto.Name = product.Name;
    productDto.Description = product.Description;
    productDto.Price = product.Price;
    productDto.Quantity = product.Quantity;
    return Results.Ok(productDto);
});

app.MapGet("/API/products/GetByName/{Name}", (IProduct productDAL, string Name) =>
{
    List<ProductDTO>ProductDto = new List<ProductDTO>();
    var products = productDAL.GetByName(Name);
    foreach(var product in products)
    {
        ProductDto.Add(new ProductDTO
        {
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity
        });
    }
    return Results.Ok(ProductDto);
});

app.MapGet("/API/products/GetByCategory/{Name}", (IProduct productDAL, string Name) =>
{
    List<ProductByNameDTO>ProductDto = new List<ProductByNameDTO>();
    var products = productDAL.GetByCategory(Name);
    foreach(var product in products)
    {
        ProductDto.Add(new ProductByNameDTO
        {
            Name = product.Name,
            CategoryName = product.CategoryName,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity
        });
    }
    return Results.Ok(ProductDto);
});

app.MapPost("/API/products/Insert/", (IProduct productDAL, ProductCreateDTO productCreateDto ) =>
{
    try{
         Product product = new Product
        {
            CategoryID = productCreateDto.CategoryID,
            Name = productCreateDto.Name,
            Description = productCreateDto.Description,
            Price = productCreateDto.Price,
            Quantity = productCreateDto.Quantity
        };
        productDAL.Insert(product);

        return Results.Created($"/API/product/Insert/{product.ProductID}", product);

    }catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
app.MapPut("/API/product/Update",(IProduct productDAL, ProductUpdateDTO productUpdateDto)=>
{
    try{
        var product = new Product
        {
            ProductID = productUpdateDto.ProductID,
            CategoryID= productUpdateDto.CategoryID,
            Name = productUpdateDto.Name,
            Description = productUpdateDto.Description,
            Price = productUpdateDto.Price,
            Quantity = productUpdateDto.Quantity,

        };
        productDAL.Update(product);
        return Results.Ok();
    }
    catch(Exception ex){
        return Results.BadRequest(ex.Message);
    }
});
app.MapDelete("/API/product/{id}",(IProduct productDAL, int id)=>{
    try
    {
        productDAL.Delete(id);
        return Results.Ok();
    }catch(Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

// update product after order
app.MapPut("/API/product/UpdateStock",(IProduct productDAL, ProductUpdateStockDTO productUpdateStockDto)=>
{
    try{
        productDAL.UpdateStockAfterOrder(productUpdateStockDto);
        return Results.Ok();
    }
    catch(Exception ex){
        return Results.BadRequest(ex.Message);
    }
});


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
