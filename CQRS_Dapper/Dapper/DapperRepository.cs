using Dapper;
using Oracle.ManagedDataAccess.Client;
using Npgsql;
using System.Data;
namespace CQRS_Dapper.Dapper
{
    public class DapperRepository : IDapperRepository
    {
        private readonly string _connectionString; 
        private readonly DatabaseType _databaseType = DatabaseType.Oracle; 
        public DapperRepository(string connectionString, DatabaseType databaseType)
        {
            _connectionString = connectionString;
            _databaseType = databaseType;
        } 

        private IDbConnection CreateConnection()
        {
            IDbConnection connection;

            switch (_databaseType)
            {
                case DatabaseType.Oracle:
                    connection = new OracleConnection(_connectionString);
                    break;
                case DatabaseType.PostgreSQL:
                    connection = new NpgsqlConnection(_connectionString);
                    break;
                default:
                    throw new NotSupportedException($"Database type '{_databaseType}' is not supported.");
            }

            return connection;
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null)
        {

            using (var connection = CreateConnection())
            {
                connection.Open();

                return await connection.QueryAsync<T>(sql, param).ConfigureAwait(true);
            }
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object param = null)
        {

            using (var connection = CreateConnection())
            {
                connection.Open();
                return await connection.QuerySingleOrDefaultAsync<T>(sql, param).ConfigureAwait(true);
            }
        }

        public async Task<int> ExecuteAsync(string sql, object param = null)
        {

            using (var connection = CreateConnection())
            {
                connection.Open();
                return await connection.ExecuteAsync(sql, param).ConfigureAwait(true);
            }
        }


        public async Task< T> GetAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                var res= await connection.QueryAsync<T>(sp, param, commandType: commandType).ConfigureAwait(true);
                return res.FirstOrDefault();
            }

        }

        public async Task<List<T>> GetAllAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                var res = await connection.QueryAsync<T>(sp, param, commandType: commandType).ConfigureAwait(true);
                return res.ToList();
            }
        }

        public DataTable GetDataTable(string sp, object param = null, CommandType commandType = CommandType.Text)
        {

            using (var connection = CreateConnection())
            {
                if (_databaseType == DatabaseType.Oracle)
                {
                    return GetDataTableFromOracle(sp);

                }
                else
                {
                    return GetDataTableFromPostgreSQL(sp);
                }
            }
        }


        private DataTable GetDataTableFromOracle(string query)
        {
            using (var connection = new OracleConnection(_connectionString))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                using var command = new OracleCommand(query, connection);
                var adapter = new OracleDataAdapter(command);
                var dataTable = new DataTable();
                adapter.Fill(dataTable);
                return dataTable;
            }
        }
        private DataTable GetDataTableFromPostgreSQL(string query)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                using var command = new NpgsqlCommand(query, connection);
                var adapter = new NpgsqlDataAdapter(command);
                var dataTable = new DataTable();

                adapter.Fill(dataTable);

                return dataTable;
            }
               
        }

        public async Task<int> ExecuteAsync(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return await connection.ExecuteAsync(sp, param, commandType: commandType).ConfigureAwait(true);
            }
        }

        public async Task<T> InsertAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            T result;
            using (var connection = CreateConnection())
            {
                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            var res = await connection.QueryAsync<T>(sp, param, commandType: commandType, transaction: tran).ConfigureAwait(true);

                            result = res.FirstOrDefault();

                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

                return result;
            }
        }

        public async Task<T> UpdateAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            T result;
            using (var connection = CreateConnection())
            {
                connection.Open();
                try
                {
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    using (var tran = connection.BeginTransaction())
                    {
                        try
                        {
                            var res = await connection.QueryAsync<T>(sp, param, commandType: commandType, transaction: tran).ConfigureAwait(true);

                            result = res.FirstOrDefault();
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

                return result;
            }
        }

        public async Task<List<T>> CountAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                var res = await connection.QueryAsync<T>(sp, param, commandType: commandType).ConfigureAwait(true);

                return res.ToList();
            }
        }



        public void GetDataFromOracle(string connectionString, string query)
        {
            using var connection = new OracleConnection(connectionString);
            using var command = new OracleCommand(query, connection);
            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                // do something with each row of data
            }
        }

        public void GetDataFromPostgreSQL(string connectionString, string query)
        {
            using var connection = new NpgsqlConnection(connectionString);
            using var command = new NpgsqlCommand(query, connection);
            connection.Open();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                // do something with each row of data
            }
        }
    }
}