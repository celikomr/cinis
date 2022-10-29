using Cinis.MySql.Tests.Models;
using MySql.Data.MySqlClient;

namespace Cinis.MySql.Tests;

public partial class DapperExtensions
{
    public string ConnectionString { get; set; }

    public DapperExtensions()
    {
        ConnectionString = "Server=localhost;User ID=root;Password=root;Database=mydb";
    }

    [Fact]
    public void Create_WithoutTransaction()
    {
        using var connection = new MySqlConnection(ConnectionString);
        connection.Open();
        try
        {
            int postId = connection.Create(new Post("Test title - 1", "Test body - 1"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}
