using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace RestaurantManager.Services
{
    public class DatabaseService
    {
        private readonly string _connectionString;

        public DatabaseService()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["RestaurantDB"]?.ConnectionString;
            _connectionString = connectionString ?? throw new InvalidOperationException("Connection string 'RestaurantDB' not found");
        }

        // Execute a stored procedure that returns a DataSet
        public async Task<DataSet> ExecuteDataSetAsync(string storedProcedure, Dictionary<string, object>? parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    // Add parameters if any
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.AddWithValue($"@{parameter.Key}", parameter.Value ?? DBNull.Value);
                        }
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataSet dataSet = new DataSet();
                        await Task.Run(() => adapter.Fill(dataSet));
                        return dataSet;
                    }
                }
            }
        }

        // Execute a stored procedure that returns a single value
        public async Task<object?> ExecuteScalarAsync(string storedProcedure, Dictionary<string, object>? parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    // Add parameters if any
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.AddWithValue($"@{parameter.Key}", parameter.Value ?? DBNull.Value);
                        }
                    }

                    await connection.OpenAsync();
                    return await command.ExecuteScalarAsync();
                }
            }
        }

        // Execute a stored procedure that doesn't return anything
        public async Task<int> ExecuteNonQueryAsync(string storedProcedure, Dictionary<string, object>? parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    // Add parameters if any
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.AddWithValue($"@{parameter.Key}", parameter.Value ?? DBNull.Value);
                        }
                    }

                    await connection.OpenAsync();
                    return await command.ExecuteNonQueryAsync();
                }
            }
        }

        // Execute a stored procedure with output parameters
        public async Task<Dictionary<string, object>> ExecuteWithOutputAsync(string storedProcedure, 
            Dictionary<string, object>? inputParameters = null, 
            Dictionary<string, SqlDbType>? outputParameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(storedProcedure, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    
                    // Add input parameters if any
                    if (inputParameters != null)
                    {
                        foreach (var parameter in inputParameters)
                        {
                            command.Parameters.AddWithValue($"@{parameter.Key}", parameter.Value ?? DBNull.Value);
                        }
                    }

                    // Add output parameters if any
                    if (outputParameters != null)
                    {
                        foreach (var parameter in outputParameters)
                        {
                            var param = command.Parameters.Add($"@{parameter.Key}", parameter.Value);
                            param.Direction = ParameterDirection.Output;
                        }
                    }

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    // Collect output parameter values
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    if (outputParameters != null)
                    {
                        foreach (var parameter in outputParameters)
                        {
                            result[parameter.Key] = command.Parameters[$"@{parameter.Key}"].Value;
                        }
                    }

                    return result;
                }
            }
        }

        // Execute a parameterized SQL query that returns a DataTable
        public async Task<DataTable> ExecuteQueryAsync(string sql, Dictionary<string, object>? parameters = null)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    // Add parameters if any
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            command.Parameters.AddWithValue($"@{parameter.Key}", parameter.Value ?? DBNull.Value);
                        }
                    }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        DataTable dataTable = new DataTable();
                        await Task.Run(() => adapter.Fill(dataTable));
                        return dataTable;
                    }
                }
            }
        }
    }
}