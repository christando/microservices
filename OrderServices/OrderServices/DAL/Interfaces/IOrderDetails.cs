using OrderServices.Model;

namespace OrderServices;

public interface IOrderDetails : ICrud<OrderDetail>
{
    IEnumerable<OrderDetail> GetByName(string name);
}
