using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Api.Shared
{
    public abstract class SqlOperation
    {
        private readonly string _connectionString;

        public SqlOperation(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected void ExecuteNonQuery(string cmdText, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand(cmdText, connection))
                {
                    cmd.Parameters.AddRange(parameters);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        protected IEnumerable<T> ExecuteReader<T>(
            string queryText, 
            Func<SqlDataReader, T> mapper,
            params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var cmd = new SqlCommand(queryText, connection))
                {
                    if(parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            yield return mapper(reader);
                        }
                    }
                }
            }
        }

        protected T ExecuteReaderOnce<T>(
            string queryText,
            Func<SqlDataReader, T> mapper,
            Func<Exception> notFound,
            params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var cmd = new SqlCommand(queryText, connection))
                {
                    if (parameters.Length > 0)
                    {
                        cmd.Parameters.AddRange(parameters);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.HasRows)
                        {
                            throw notFound();
                        }

                        reader.Read();
                        return mapper(reader);
                    }
                }
            }
        }
    }
}
