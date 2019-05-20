using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace CitizenBudget.Common.Dapper
{
    public class DbConnectionFactory : IDbConnectionFactory
    {
        public string ConnectionString { get; }

        public DbConnectionFactory(MssqlConnectionEntity item)
        {
            ConnectionString = $"Data Source={item.ServerName};" +
                               $"Initial Catalog={item.DataBaseName};" +
                               $"User id={item.UserName};" +
                               $"Password={item.Password};";
        }

        public SqlConnection GetConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}
