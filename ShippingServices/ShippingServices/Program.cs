using System.Text.Json;
using OrderServices.Services;
using Polly;
using ShippingServices.DAL;
using ShippingServices.DAL.Interfaces;
using ShippingServices.DTO;
using ShippingServices.Models;
using ShippingServices.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IShippings, ShippingsDAL>();

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

app.MapGet("/API/Shipping", (IShippings shippingDAL) =>
{
    List<Shippings> shippingsDto = new List<Shippings>();
    var shippings = shippingDAL.GetAll();
    foreach (var Shipping in shippings)
    {
        shippingsDto.Add (new Shippings
        {
            ShippingID = Shipping.ShippingID,
            ShippingVendor = Shipping.ShippingVendor,
            ShippingDate = Shipping.ShippingDate,
            ShippingStatus = Shipping.ShippingStatus,
            OrderHeaderID = Shipping.OrderHeaderID,
            BeratBarang = Shipping.BeratBarang,
            BiayaShipping = Shipping.BiayaShipping
            
        });
    }
    return Results.Ok(shippingsDto);
});

app.MapPost("/API/Shipping", async (IShippings shippingDAL, Shippings obj, IWalletService walletService) =>
{
    try
    {
        // Check if wallet exists
        var wallet = await walletService.GetWalletByUsername(obj.Username);
        if (wallet == null)
        {
            return Results.BadRequest("Wallet not found");
        }

        // Check if wallet has enough saldo
        var totalCost = obj.BiayaShipping * obj.BeratBarang;
        if (wallet.saldo < totalCost)
        {
            return Results.BadRequest("Saldo not enough");
        }
        obj.BiayaShipping = totalCost;
        obj.Username = wallet.username;

        // Insert shipping
        var shipping = shippingDAL.Insert(obj);
        if (shipping == null)
        {
            return Results.BadRequest("Failed to insert shipping");
        }


        // Update wallet saldo
        var walletUpdate = new WalletUpdateSaldoDTO
        {
            username = wallet.username,
            password = wallet.password,  // Assuming you have a password field
            saldo = totalCost,
            paymentWallet = wallet.paymentWallet
        };

        // Log JSON payload
        var jsonPayload = JsonSerializer.Serialize(walletUpdate);
        Console.WriteLine("Updating Saldo with payload: " + jsonPayload);

        await walletService.UpdateSaldo(walletUpdate);

        
        return Results.Created($"/API/Shipping/{shipping.ShippingID}", shipping);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("/API/Shipping/{id}", (IShippings shippingDAL, int id, Shippings obj) =>
{
    try
    {
        
        var shipping = new Shippings{
            ShippingID = id,
            ShippingStatus = obj.ShippingStatus
        };
        
        shippingDAL.Update(shipping);
        return Results.Ok(shipping);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
