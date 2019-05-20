using System;
using System.Data;
using System.Threading.Tasks;

namespace CitizenBudget.Common.Dapper
{
    public interface IDapperWrapper
    {
        Task ScopeExecuteAsync(Func<IDbConnection, Task> funcAsync);
        Task<TResult> ScopeQueryAsync<TResult>(Func<IDbConnection, Task<TResult>> funcAsync);
    }
}