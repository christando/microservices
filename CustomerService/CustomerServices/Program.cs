using CustomerServices.DAL;
using CustomerServices.DAL.interfaces;
using CustomerServices.DTO;
using CustomerServices.models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICustomer, CustomerDAL>();

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

app.MapGet("/API/Customer", (ICustomer customerDAL) => 
{
    List<CustomerDTO> customersDto = new List<CustomerDTO>();
    var customers = customerDAL.GetAll();
    foreach(var customer in customers)
    {
        customersDto.Add(new CustomerDTO
        {
            FullName = customer.FullName,
            Username = customer.Username,
            Password = customer.Password
        });
    }
    return Results.Ok(customersDto);
});
app.MapGet("/API/Customer/GetByUsername/{username}", (ICustomer customerDAL, string username) => 
{
    List<CustomerDTO> customersDto = new List<CustomerDTO>();
    var customers = customerDAL.GetByUsername(username);
    foreach(var customer in customers)
    {
        customersDto.Add(new CustomerDTO
        {
            FullName = customer.FullName,
            Username = customer.Username,
            Password = customer.Password
        });
    }
    return Results.Ok(customersDto);
});

app.MapPost("/API/Customer/insert/", (ICustomer customerDAL, Customer obj) =>
{
    try{
        var customer = customerDAL.Insert(obj);
        return Results.Created($"/API/Customer/{customer.CustomerID}", customer);
    }catch(Exception ex){
        throw new Exception($"Error: {ex.Message}");
    }
});
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
