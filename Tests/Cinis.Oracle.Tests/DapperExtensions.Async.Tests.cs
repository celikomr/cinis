using Cinis.Oracle.Tests.Models;
using Oracle.ManagedDataAccess.Client;

namespace Cinis.Oracle.Tests;

public partial class DapperExtensions
{
    [Fact]
    public async Task CreateAsync_WithoutTransaction()
    {
        using var connection = new OracleConnection(ConnectionString);
        connection.Open();
        try
        {
            int postId = await connection.CreateAsync(new Post("Test title - 1", "Test body - 1"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}
