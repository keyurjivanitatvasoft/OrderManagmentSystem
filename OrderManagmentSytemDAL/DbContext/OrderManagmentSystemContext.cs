using Microsoft.Extensions.Options;
using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.DbContext
{
    public class OrderManagmentSystemContext
    {
        private readonly string _connectionString;

        public OrderManagmentSystemContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public int ExecuteNonQuery(string query,SqlParameter[] parameters,bool IsStoreProcedure)
        {
            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                if(IsStoreProcedure)
                {
                    command.CommandType = CommandType.StoredProcedure;
                }
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();
                return rowsAffected;
            }
        }

        public DataTable ExecuteQuery(string query, params SqlParameter[] parameters)
        {
            var dataTable = new DataTable();

            using (var connection = new SqlConnection(_connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    connection.Open();
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        adapter.Fill(dataTable);
                    }
                    connection.Close();
                }
            }

            return dataTable;
        }
        public DataTable ExecuteReader(string query)
        {
            var dataTable = new DataTable();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
                connection.Close() ;
            }
            return dataTable;
        }
        public DataTable ExecuteReader(string query, SqlParameter[] parameters,bool IsStoreProcedure)
        {
            var dataTable = new DataTable();

            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(query, connection))
                {
                    if (IsStoreProcedure)
                    {
                        command.CommandType = CommandType.StoredProcedure;
                    }
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        if (parameters != null)
                        {
                            command.Parameters.AddRange(parameters);
                        }

                        adapter.Fill(dataTable);
                    }
                }
                connection.Close() ;
            }
            return dataTable;
        }

    }
}
