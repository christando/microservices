using OrderServices;
using OrderServices.DAL;
using OrderServices.DAL.Interfaces;
using OrderServices.DTO;
using OrderServices.Model;
using OrderServices.Services;
using Polly;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICustomers, CustomerDAL>();
builder.Services.AddScoped<IOrderHeaders, OrderHeaderDAL>();
builder.Services.AddScoped<IOrderDetails, OrderDetailDAL>();

//Register Service Products
builder.Services.AddHttpClient<IProductService, ProductService>()
.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));
builder.Services.AddHttpClient<IWalletService, WalletService>()
.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(600)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/API/Customers", (ICustomers customerDAL) =>
{
    List<CustomerDTO> customersDto = new List<CustomerDTO>();
    var customers = customerDAL.GetAll();
    foreach (var customer in customers)
    {
        customersDto.Add(new CustomerDTO 
        { 
            CustomerName = customer.CustomerName    
        });
    }
    return Results.Ok(customersDto);
});
app.MapGet("/API/Customers/GetById/{id}", (ICustomers customerDAL, int id) =>
{
    var customer = customerDAL.GetById(id);
    if (customer == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(customer);
});

app.MapPost("/API/Customers/insert/", (ICustomers customerDAL, Customer obj) =>
{
    try{
        var customer = customerDAL.Insert(obj);
        return Results.Created($"/API/Customers/insert/{customer.CustomerId}", customer);
    }
    catch(Exception ex){
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/API/Customers/update/{id}", (ICustomers customerDAL, int id, Customer obj) =>
{
    try{
        var customer = customerDAL.GetById(id);
        if(customer == null)
        {
            return Results.NotFound();
        }
        customer.CustomerName = obj.CustomerName;
        customerDAL.Update(customer);
        return Results.Ok(customer);
    }
    catch(Exception ex){
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/API/OrderHeader", (IOrderHeaders orderHeadersDAL) => 
{
    List<OrderHeaderDTO> orderheaderDto = new List<OrderHeaderDTO>();
    var orderheaders = orderHeadersDAL.GetAll();
    foreach (var orderheader in orderheaders)
    {
        orderheaderDto.Add(new OrderHeaderDTO
        {
            OrderHeaderId = orderheader.OrderHeaderId,
            CustomerId = orderheader.CustomerId,
            OrderDate = orderheader.OrderDate,
            Username = orderheader.Username
        });
    }
    return Results.Ok(orderheaderDto);
});

app.MapGet("/API/OrderHeader/GetById/{id}", (IOrderHeaders orderheaderDAL, int id) =>
{
    var orderheader = orderheaderDAL.GetById(id);
    if (orderheader == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(orderheader);
});

app.MapPost("/API/OrderHeader/Insert/", (IOrderHeaders orderheaderDAL, OrderHeader obj)=>
{
    try
    {
        var orderheader = orderheaderDAL.Insert(obj);
        return Results.Created($"/API/OrderHeader/Insert/{orderheader.OrderHeaderId}", orderheader);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);   
    }
});

app.MapPut("/API/OrderHeader/Update/{id}", (IOrderHeaders orderheaderDAL, int id, OrderHeaderUpdate OHDTO) =>
{
    try
    {
        var orderheader = new OrderHeader{
        OrderHeaderId = id,
        CustomerId = OHDTO.CustomerId,
        OrderDate = OHDTO.OrderDate,
        Username = OHDTO.Username
        };
        orderheaderDAL.Update(orderheader);
        return Results.Ok(orderheader);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapGet("/OrderDetails", (IOrderDetails orderDetail )=>
{
    return Results.Ok(orderDetail.GetAll());
});
app.MapGet("/OrderDetails/GetById/{id}", (IOrderDetails orderDetail, int id) =>
{
    var order = orderDetail.GetById(id);
    if (orderDetail == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(order);
});

app.MapPost("/OrderDetails/insert", async (IOrderDetails orderDetail,IProductService productService ,OrderDetail obj, IWalletService walletService) =>
{
    try
    {
        //cek apakah product ada pada service product
        var Product = await productService.GetProductById(obj.ProductId);
        if(Product == null)
        {
            return Results.BadRequest("Product not found");
        }
        if(Product.quantity < obj.Quantity)
        {
            return Results.BadRequest("Stock not enough");
        }
        var Wallet = await walletService.GetWalletByUsername(obj.Username);
        if(Wallet == null)
        {
            return Results.BadRequest("Wallet not found");
        }
        if(Wallet.saldo < (obj.Quantity * Product.price))
        {
            return Results.BadRequest("Saldo not enough");
        }


        obj.Price = Product.price;
        obj.Username = Wallet.username;
        var order = orderDetail.Insert(obj);
        
        // update stock product
        var productupdate = new ProductUpdateStockDTO
        {
            productID = obj.ProductId,
            quantity = obj.Quantity
        };
        await productService.UpdateProductStock(productupdate);
        
        // update saldo wallet
        var walletUpdate = new WalletUpdateSaldoDTO
        {
            username = obj.Username,
            saldo = obj.Quantity * Product.price
        };
        await walletService.UpdateSaldo(walletUpdate);

        return Results.Created($"/OrderDetails/insert/{order.OrderDetailId}", order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Error: {ex.Message} - {ex.Source} - {ex.StackTrace}");
    }
});

app.MapDelete("/OrderDetails/delete/{id}", (IOrderDetails orderDetail, int id) =>
{
    try
    {
        var order = orderDetail.GetById(id);
        if(order == null)
        {
            return Results.NotFound();
        }
        orderDetail.Delete(id);
        return Results.Ok();
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/OrderDetails/update/{id}", async (IOrderDetails orderDetail,IProductService productService,int id, OrderDetail obj) =>
{
    try{
        var order = orderDetail.GetById(id);
        if(order == null){
            return Results.NotFound();
        }
        var Product = await productService.GetProductById(obj.ProductId);
        if(Product == null){
            return Results.BadRequest("Product not found");
        }
        if(Product.quantity < obj.Quantity){
            return Results.BadRequest("Stock not enough");
        }
        obj.Price = Product.price;
        obj.OrderDetailId = id;
        orderDetail.Update(obj);
        return Results.Ok(obj);
    }catch(Exception ex){
        return Results.BadRequest(ex.Message);
    }
});

app.Run();
