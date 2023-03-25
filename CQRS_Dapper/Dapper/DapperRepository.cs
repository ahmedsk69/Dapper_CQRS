using Dapper;
using Oracle.ManagedDataAccess.Client;
using Npgsql;
using System.Data;
using static Dapper.SqlMapper;

namespace CQRS_Dapper.Dapper
{
    public class DapperRepository : IDapperRepository
    {
        private readonly string _connectionString; 
        private readonly IDbTransaction _dbTransaction; 
        private readonly DatabaseType _databaseType = DatabaseType.Oracle; 
        public DapperRepository(string connectionString, DatabaseType databaseType = DatabaseType.Oracle)
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
         

        public async Task<T> QuerySingleAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text)
        {

            using (var connection = CreateConnection())
            {
                connection.Open();
                return await connection.QuerySingleOrDefaultAsync<T>(sp, param, commandType: commandType).ConfigureAwait(true);
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

        public async Task<int> ExecuteAsync(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return await connection.ExecuteAsync(sp, param, commandType: commandType).ConfigureAwait(true);
            }
        }
        public async Task<object> ExecuteScalarAsync(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return await connection.ExecuteScalarAsync(sp, param, commandType: commandType).ConfigureAwait(true);
            }
        }
        public  object ExecuteReader(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return   connection.ExecuteReader(sp, param, commandType: commandType);
            }
        }
        public async Task<dynamic> QuerySingleAsync(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return await connection.QuerySingleAsync(sp, param, commandType: commandType).ConfigureAwait(true);
            }
        }
        public Task<GridReader> QueryMultipleAsync(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return connection.QueryMultipleAsync(sp, param, commandType: commandType);
            }
        }
        public async Task<T> QueryAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text)
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
         
        public async Task<List<T>> CountAsync<T>(string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                var res = await connection.QueryAsync<T>(sp, param, commandType: commandType).ConfigureAwait(true);

                return res.ToList();
            }
        }

        #region Transaction
        public Task<int> ExecuteAsync(IDbTransaction dbTransaction, string sp, object param = null, CommandType commandType = CommandType.Text)
        {
           return dbTransaction.Connection.ExecuteAsync(sp, param, commandType: commandType, transaction: dbTransaction);
        }
        public Task<dynamic> QuerySingleAsync(IDbTransaction dbTransaction, string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            return  dbTransaction.Connection.QuerySingleAsync(sp, param, commandType: commandType, transaction: dbTransaction);
        }

        public async Task<T> QueryAsync<T>(IDbTransaction dbTransaction, string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            var res = await dbTransaction.Connection.QueryAsync<T>(sp, param, commandType: commandType, transaction: dbTransaction).ConfigureAwait(true);
            return  res.FirstOrDefault();
        }

        public Task<object> ExecuteScalarAsync(IDbTransaction dbTransaction, string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            return dbTransaction.Connection.ExecuteScalarAsync(sp, param, commandType: commandType);
        }
        public object ExecuteReader(IDbTransaction dbTransaction, string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            return dbTransaction.Connection.ExecuteReader(sp, param, commandType: commandType);
        }
        public Task< GridReader> QueryMultipleAsync(IDbTransaction dbTransaction, string sp, object param = null, CommandType commandType = CommandType.Text)
        {
            return  dbTransaction.Connection.QueryMultipleAsync(sp, param, commandType: commandType);
        }
        public IDbConnection CreateTranConnection(DatabaseType databaseType)
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
        public void CommitTransaction()
        {
            _dbTransaction.Commit();
        }
        public void RollbackTransaction()
        {
            _dbTransaction.Rollback();
        }

        #endregion


        #region Ado.Net
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
        #endregion
    }
}