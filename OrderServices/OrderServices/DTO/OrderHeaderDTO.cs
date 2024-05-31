namespace OrderServices;

public class OrderHeaderDTO
{
    public int OrderHeaderId { get; set; }
    public int CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public string Username { get; set; }
}
