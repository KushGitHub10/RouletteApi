using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace RouletteApi.Interface
{
    public interface IDapperRepository
    {
        Task ExecuteAsync(string query, DynamicParameters parameters = null);
        Task ExecuteAsync(string query, CommandType commandType, DynamicParameters parameters = null);
        Task<(int ReturnValue, DynamicParameters Parameters)> ExecuteOutParamAsync(string query, DynamicParameters parameters);
        Task<IEnumerable<T>> QueryAsync<T>(string query, DynamicParameters parameters = null);// where T : class;
        Task<IEnumerable<T>> QueryAsync<T>(string query, CommandType commandType, DynamicParameters parameters = null);// where T : class;
    }
}
