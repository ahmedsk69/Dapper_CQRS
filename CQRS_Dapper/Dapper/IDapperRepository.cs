using Dapper;
using System.Data;

namespace CQRS_Dapper.Dapper
{
    public interface IDapperRepository
    {  
        Task<T> GetAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text);
        Task<List<T>> GetAllAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text);
        DataTable GetDataTable(string sp, object param = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteAsync(string sp, object param = null, CommandType commandType = CommandType.Text);
        Task<List<T>> CountAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text);
        Task<T> QueryAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text); 
        Task<object> ExecuteScalarAsync(string sp, object param = null, CommandType commandType = CommandType.Text);
        object ExecuteReader(string sp, object param = null, CommandType commandType = CommandType.Text);
        Task<SqlMapper.GridReader> QueryMultipleAsync(string sp, object param = null, CommandType commandType = CommandType.Text);
    }

}
