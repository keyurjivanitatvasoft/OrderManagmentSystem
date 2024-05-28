using Dapper;
using Microsoft.Extensions.Options;
using OrderManagmentSytemDAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagmentSytemDAL.DbContext
{
    public  class OrderManagmentSystemDapperContext
    {
        private readonly string connectionString;

        public OrderManagmentSystemDapperContext(string connectionString)
        {
            this.connectionString = connectionString;
        }
        private IDbConnection CreateConnection()
        {
            return new SqlConnection(connectionString);
        }
        public int Execute(string query, object parameters = null,bool IsStoreProcedure=false)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                int rowAffected = 0;
                if (IsStoreProcedure)
                {
                    rowAffected = connection.Execute(query, parameters,commandType: CommandType.StoredProcedure);
                }
                else
                {
                    rowAffected = connection.Execute(query, parameters);
                }
                connection.Close();
                return rowAffected;
            }
        }

        public IEnumerable<T> Query<T>(string query,object parameters, bool IsStoreProcedure=false)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                IEnumerable<T> Entities;
                if (IsStoreProcedure)
                {
                    Entities = connection.Query<T>(query, parameters,commandType: CommandType.StoredProcedure);
                }
                else
                {
                    Entities= connection.Query<T>(query, parameters);
                }
                   
                connection.Close();
                return Entities;
            }
        }
        public T QuerySingle<T>(string query,object parameters=null)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                T entity = connection.QuerySingle<T>(query, parameters);
                connection.Close();
                return entity;
            }
        }


    }
}
