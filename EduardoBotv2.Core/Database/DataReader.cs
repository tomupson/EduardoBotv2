using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace EduardoBotv2.Core.Database
{
    public class DataReader
    {
        private readonly List<SqlParameter> parameters = new List<SqlParameter>();

        public string StoredProcedure { get; }

        public string ConnectionString { get; }

        public DataReader(string storedProcedure, string connectionString)
        {
            ConnectionString = connectionString;
            StoredProcedure = storedProcedure;
        }

        public void AddParameter(string parameterName, object value) => AddParameter(new SqlParameter(parameterName, value));

        public void AddParameter(SqlParameter parameter) => parameters.Add(parameter);

        public void ClearParameters() => parameters.Clear();

        public async Task ExecuteReaderAsync(ProcessRecordAsyncDelegate processor)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand(StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters.ToArray());

                    IDataReader reader = await cmd.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        await processor(reader);
                    }
                }
            }
        }

        public void ExecuteReader(ProcessRecordDelegate processor)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters.ToArray());

                    IDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        processor(reader);
                    }
                }
            }
        }

        public async Task ExecuteNonQueryAsync()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = new SqlCommand(StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters.ToArray());
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public object ExecuteScalar()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                using (SqlCommand cmd = new SqlCommand(StoredProcedure, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddRange(parameters.ToArray());
                    return cmd.ExecuteScalar();
                }
            }
        }

        public delegate void ProcessRecordDelegate(IDataReader reader);

        public delegate Task ProcessRecordAsyncDelegate(IDataReader reader);
    }
}