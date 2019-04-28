using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Xml;

namespace EduardoBotv2.Core.Database
{
    public class AsyncDataReader
    {
        private readonly List<SqlParameter> _parameters = new List<SqlParameter>();

        public string StoredProcedure { get; }

        public string ConnectionString { get; }

        public AsyncDataReader(string storedProcedure, string connectionString)
        {
            ConnectionString = connectionString;
            StoredProcedure = storedProcedure;
        }

        public void AddParameter(string parameterName, object value) => AddParameter(new SqlParameter(parameterName, value));

        public void AddParameter(SqlParameter parameter) => _parameters.Add(parameter);

        public void ClearParameters() => _parameters.Clear();

        public async Task ExecuteReaderAsync(ProcessRecordAsyncDelegate processor)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = SetupCommand(new SqlCommand(StoredProcedure, conn)))
                {
                    IDataReader reader = await cmd.ExecuteReaderAsync();

                    while (reader.Read())
                    {
                        await processor(reader);
                    }
                }
            }
        }

        public async Task ExecuteNonQueryAsync()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = SetupCommand(new SqlCommand(StoredProcedure, conn)))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task<object> ExecuteScalarAsync()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = SetupCommand(new SqlCommand(StoredProcedure, conn)))
                {
                    return await cmd.ExecuteScalarAsync();
                }
            }
        }

        public async Task<object> ExecuteWithReturnValue()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = SetupCommand(new SqlCommand(StoredProcedure, conn)))
                {
                    cmd.Parameters.Add(new SqlParameter("@retVal", null)
                    {
                        Direction = ParameterDirection.ReturnValue
                    });

                    await cmd.ExecuteNonQueryAsync();

                    return cmd.Parameters["@retVal"].Value;
                }
            }
        }

        public async Task<XmlReader> ExecuteXmlReaderAsync()
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = SetupCommand(new SqlCommand(StoredProcedure, conn)))
                {
                    return await cmd.ExecuteXmlReaderAsync();
                }
            }
        }

        private SqlCommand SetupCommand(SqlCommand cmd,
            CommandType commandType = CommandType.StoredProcedure)
        {
            cmd.CommandType = commandType;
            cmd.Parameters.AddRange(_parameters.ToArray());

            return cmd;
        }


        public delegate Task ProcessRecordAsyncDelegate(IDataReader reader);
    }
}