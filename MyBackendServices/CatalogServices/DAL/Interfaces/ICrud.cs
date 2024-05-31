namespace CatalogServices;

public interface ICrud<T>
{
    IEnumerable<T> GetAll();
    T GetById(int id);
    void Insert(T obj);
    void Update(T obj);
    void Delete(int id);
}
