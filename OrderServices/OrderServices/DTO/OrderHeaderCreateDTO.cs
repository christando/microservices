namespace OrderServices;

public class OrderHeaderCreateDTO
{
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Username { get; set; }
}
