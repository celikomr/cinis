using Cinis.Oracle.Tests.Models;
using Oracle.ManagedDataAccess.Client;

namespace Cinis.Oracle.Tests;

public partial class DapperExtensions
{
    [Fact]
    public void Create_WithTransaction()
    {
        string connStr = "";
        using var connection = new OracleConnection(connStr);
        connection.Open();
        using OracleTransaction transaction = connection.BeginTransaction();
        try
        {
            connection.Create(new Person("John", "Cartem", 21, DateTime.Now));
            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            transaction.Rollback();
            throw;
        }
    }
}
