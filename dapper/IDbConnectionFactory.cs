using System.Data.SqlClient;

namespace CitizenBudget.Common.Dapper
{
    public interface IDbConnectionFactory
    {
        SqlConnection GetConnection();
    }
}