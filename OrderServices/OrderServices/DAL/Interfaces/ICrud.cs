namespace OrderServices;

public interface ICrud<T>
{
    IEnumerable<T> GetAll();
    T GetById(int id);
    T Insert(T obj);
    void Update(T obj);
    void Delete(int id);
}
