using System.Text.Json;
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
builder.Services.AddHttpClient<ICustomerService, CustomerService>()
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
            OrderDate = orderheader.OrderDate,
            Username = orderheader.Username,
            Password = orderheader.Password
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

app.MapPost("/API/OrderHeader/Insert/", async (IOrderHeaders orderheaderDAL, OrderHeader obj, ICustomerService customerService)=>
{
    try
    {

        var Customer = await customerService.GetCustomerByUsername(obj.Username);
        if(Customer == null)
        {
            return Results.BadRequest("Customer not found");
        }
        
        obj.Username = Customer.username;
        obj.Password = Customer.password;

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

app.MapPost("/OrderDetails/insert", async (IOrderDetails orderDetail, IProductService productService, OrderDetail obj, IWalletService walletService) =>
{
    try
    {
        // Check if product exists
        var product = await productService.GetProductById(obj.ProductId);
        if (product == null)
        {
            return Results.BadRequest("Product not found");
        }

        // Check product stock
        if (product.quantity < obj.Quantity)
        {
            return Results.BadRequest("Stock not enough");
        }

        // Check if wallet exists
        var wallet = await walletService.GetWalletByUsername(obj.Username);
        if (wallet == null)
        {
            return Results.BadRequest("Wallet not found");
        }

        // Check if wallet has enough saldo
        var totalCost = obj.Quantity * product.price;
        if (wallet.saldo < totalCost)
        {
            return Results.BadRequest("Saldo not enough");
        }

        // Set price and username in order detail object
        obj.Price = product.price;
        obj.Username = wallet.username;

        // Insert order detail
        var order = orderDetail.Insert(obj);
        if (order == null)
        {
            return Results.BadRequest("Failed to insert order detail");
        }

        // Update product stock
        var productUpdate = new ProductUpdateStockDTO
        {
            productID = obj.ProductId,
            quantity = obj.Quantity
        };
        await productService.UpdateProductStock(productUpdate);

        // Update wallet saldo
        var walletUpdate = new WalletUpdateSaldoDTO
        {
            username = wallet.username,
            password = wallet.password,
            saldo = totalCost,
            paymentWallet = wallet.paymentWallet
        };

        // Log JSON payload
        var jsonPayload = JsonSerializer.Serialize(walletUpdate);
        Console.WriteLine("Updating Saldo with payload: " + jsonPayload);

        await walletService.UpdateSaldo(walletUpdate);

        return Results.Created($"/OrderDetails/insert/{order.OrderDetailId}", order);
    }
    catch (Exception ex)
    {
        // Log detailed error
        var errorMessage = $"Error: {ex.Message} - Source: {ex.Source} - StackTrace: {ex.StackTrace}";
        Console.WriteLine(errorMessage);
        return Results.BadRequest(errorMessage);
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
