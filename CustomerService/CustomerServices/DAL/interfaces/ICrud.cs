using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerServices.DAL.interfaces
{
    public interface ICrud<T>
    {
    IEnumerable<T> GetAll();
    T GetById(int id);
    T Insert(T obj);
    void Update(T obj);
    void Delete(int id);
    }
}