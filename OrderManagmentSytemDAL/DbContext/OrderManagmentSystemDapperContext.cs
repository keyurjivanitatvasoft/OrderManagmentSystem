using Dapper;
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
        public int Execute(string query, object parameters = null)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                int rowAffected = connection.Execute(query,parameters);
                connection.Close();
                return rowAffected;
            }
        }

        public IEnumerable<T> Query<T>(string query)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                IEnumerable<T> Entities = connection.Query<T>(query);
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
