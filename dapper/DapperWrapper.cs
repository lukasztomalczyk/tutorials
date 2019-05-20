using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Threading.Tasks;

namespace CitizenBudget.Common.Dapper
{
    public class DapperWrapper : IDapperWrapper
    {
        public IDbConnection _connection { get; }

        public DapperWrapper(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<TResult> ScopeQueryAsync<TResult>(Func<IDbConnection, Task<TResult>> funcAsync)
        {
            using (_connection)
            {
                _connection.Open();
                return await funcAsync(_connection);
            }
        }
        public async Task ScopeExecuteAsync(Func<IDbConnection, Task> funcAsync)
        {
            using (_connection)
            {
                _connection.Open();
                await funcAsync(_connection);
            }
        }
    }
}
