using System.Data;
using Dapper;
using MySql.Data.MySqlClient;

namespace DatabaseAccess;

public class Access : IAccess
{
    public async Task<List<T>> QueryAsync<T, TU>(string sql, TU parameters, string connectionString)
    {
        using IDbConnection connection = new MySqlConnection("Server=127.0.0.1;Port=3306;Database=comforix;Uid=comforix_db;Pwd=aKrL72CMwomSkoMzvufz3CfmuUzuWT5o");
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        IEnumerable<T> rows = await connection.QueryAsync<T>(sql, parameters);
        return rows.ToList();
    }
    
    public Task ExecuteAsync<T>(string sql, T parameters, string connectionString)
    {
        using IDbConnection connection = new MySqlConnection("Server=127.0.0.1;Port=3306;Database=comforix;Uid=comforix_db;Pwd=aKrL72CMwomSkoMzvufz3CfmuUzuWT5o");
        // ReSharper disable once HeapView.PossibleBoxingAllocation
        return connection.ExecuteAsync(sql, parameters);
    }
}