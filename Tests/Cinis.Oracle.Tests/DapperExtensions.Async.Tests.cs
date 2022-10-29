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

    [Fact]
    public async Task CreateAsync_WithTransaction()
    {
        using var connection = new OracleConnection(ConnectionString);
        connection.Open();
        using OracleTransaction transaction = connection.BeginTransaction();
        try
        {
            int postId = await connection.CreateAsync(new Post("Test title - 1", "Test body - 1"), transaction);
            await connection.CreateAsync(new Comment(postId, "Test name - 1", "Test email - 1", "Test body - 1"), transaction);
            await connection.CreateAsync(new Comment(postId, "Test name - 2", "Test email - 2", "Test body - 2"), transaction);
            await connection.CreateAsync(new Comment(postId, "Test name - 3", "Test email - 3", "Test body - 3"), transaction);
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
