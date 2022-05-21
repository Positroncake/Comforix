using System.Collections.Generic;
using System.Threading.Tasks;

namespace DatabaseAccess;

public interface IAccess
{
    Task<List<T>> QueryAsync<T, TU>(string sql, TU parameters);
    Task ExecuteAsync<T>(string sql, T parameters);
}