using System;
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

        public async Task ExecuteReaderAsync(ProcessRecordAsyncDelegate processor, bool withTransaction = false)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = SetupCommand(new SqlCommand(StoredProcedure, conn)))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        if (withTransaction)
                        {
                            transaction = conn.BeginTransaction("DefaultTransaction");
                            cmd.Transaction = transaction;
                        }

                        IDataReader reader = await cmd.ExecuteReaderAsync();

                        transaction?.Commit();

                        while (reader.Read())
                        {
                            await processor(reader);
                        }
                    } catch
                    {
                        transaction?.Rollback();
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

        public async Task<object> ExecuteScalarAsync(bool withTransaction = false)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                await conn.OpenAsync();

                using (SqlCommand cmd = SetupCommand(new SqlCommand(StoredProcedure, conn)))
                {
                    SqlTransaction transaction = null;
                    try
                    {
                        if (withTransaction)
                        {
                            transaction = conn.BeginTransaction("DefaultTransaction");
                            cmd.Transaction = transaction;
                        }

                        object result = await cmd.ExecuteScalarAsync();
                        transaction?.Commit();
                        return result;
                    } catch
                    {
                        transaction?.Rollback();
                        throw;
                    }
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