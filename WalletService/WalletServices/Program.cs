using System.Text;
using WalletServices.DAL;
using WalletServices.DAL.Interfaces;
using WalletServices.DTO;
using WalletServices.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IWallet, WalletDAL>();

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

app.MapGet("/API/Wallet" , (IWallet walletDAL) =>
{
    List<Wallet> walletDto = new List<Wallet>();
    var wallets = walletDAL.GetAll();
    foreach(var wallet in wallets)
    {
        walletDto.Add(new Wallet
        {
            WalletId = wallet.WalletId,
            Username = wallet.Username,
            Password = wallet.Password,
            FullName = wallet.FullName,
            Saldo = wallet.Saldo
        });
    }
    return Results.Ok(walletDto);
});

app.MapGet("API/Wallet/GetByUsername/{username}", (IWallet walletDAL, string username) =>
{
    List<Wallet> walletDto = new List<Wallet>();
    var wallets = walletDAL.GetByUsername(username);
    foreach(var wallet in wallets)
    {
        walletDto.Add(new Wallet
        {
            WalletId = wallet.WalletId,
            Username = wallet.Username,
            Password = wallet.Password,
            FullName = wallet.FullName,
            Saldo = wallet.Saldo
        });
    }
    return Results.Ok(walletDto);
});
app.MapPost("/API/Wallet/insert/", (IWallet walletDAL, WalletDTO walletDto) =>
{
    try
    {
        Wallet wallet = new Wallet
        {
            Username = walletDto.Username,
            Password = walletDto.Password,
            FullName = walletDto.FullName,
            Saldo = walletDto.Saldo
        };
        walletDAL.Insert(wallet);
        return Results.Created($"/API/Wallet/{wallet.WalletId}", wallet);
    }catch(Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});
app.MapPut("/API/Wallet/TopUp/{id}", (IWallet walletDAL, WalletTopUpDTO walletTopUpDto, int id) =>
{
    try
    {
        var wallet = walletDAL.TopUpSaldo(id, walletTopUpDto.Saldo);
        return Results.Ok(wallet);
    }catch(Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.MapPut("API/Wallet/UpdateSaldoAfterOrder", (IWallet walletDAL, WalletUpdateSaldoDTO walletUpdateSaldoDTO) =>
{
    try
    {
        walletDAL.UpdateSaldoAfterOrder(walletUpdateSaldoDTO);
        return Results.Ok();
    }catch(Exception ex)
    {
        return Results.BadRequest(ex.Message);
    }
});

app.Run();
