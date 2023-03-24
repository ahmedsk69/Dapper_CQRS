using System.Data;

namespace CQRS_Dapper.Dapper
{
    public interface IDapperRepository
    { 
        Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null);
        Task<T> QuerySingleAsync<T>(string sql, object param = null);
        Task<int> ExecuteAsync(string sql, object param = null);
        Task<T> GetAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text);
        Task<List<T>> GetAllAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text);
        DataTable GetDataTable(string sp, object param = null, CommandType commandType = CommandType.Text);
        Task<int> ExecuteAsync(string sp, object param = null, CommandType commandType = CommandType.Text);
        Task<T> InsertAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text);
        Task<T> UpdateAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text);
        Task<List<T>> CountAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text);
    }

}
