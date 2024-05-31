using OrderServices.Model;

namespace OrderServices.DAL.Interfaces{
    public interface IOrderHeaders : ICrud<OrderHeader>
    {
        IEnumerable<OrderHeader> GetByDate(string date);
    }
}


