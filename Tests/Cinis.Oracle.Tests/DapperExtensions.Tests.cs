using Cinis.Oracle.Tests.Models;
using Oracle.ManagedDataAccess.Client;

namespace Cinis.Oracle.Tests;

public partial class DapperExtensions
{
    public string ConnectionString { get; set; }

    public DapperExtensions()
    {
        ConnectionString = "Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521)) (CONNECT_DATA =(SERVICE_NAME = xe) (SERVER = DEDICATED) ) ); User ID = OCLK; Password = OCLK;";
    }

    [Fact]
    public void Create_WithoutTransaction()
    {
        using var connection = new OracleConnection(ConnectionString);
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

    [Fact]
    public void Create_WithTransaction()
    {
        using var connection = new OracleConnection(ConnectionString);
        connection.Open();
        using OracleTransaction transaction = connection.BeginTransaction();
        try
        {
            int postId = connection.Create(new Post("Test title - 1", "Test body - 1"), transaction);
            connection.Create(new Comment(postId, "Test name - 1", "Test email - 1", "Test body - 1"), transaction);
            connection.Create(new Comment(postId, "Test name - 2", "Test email - 2", "Test body - 2"), transaction);
            connection.Create(new Comment(postId, "Test name - 3", "Test email - 3", "Test body - 3"), transaction);
            transaction.Commit();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            transaction.Rollback();
            throw;
        }
    }

    [Fact]
    public void Read_ById()
    {
        using var connection = new OracleConnection(ConnectionString);
        connection.Open();
        try
        {
            Post? post = connection.Read<Post>(1000001541).FirstOrDefault();
            if (post != null)
            {
                post.Comments = connection.Read<Comment>(whereClause: $"POST_ID = '{post.Id}'");
            }
            Assert.NotNull(post);
            Assert.NotNull(post?.Comments);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    [Fact]
    public void Read_ByWhereClause()
    {
        using var connection = new OracleConnection(ConnectionString);
        connection.Open();
        try
        {
            List<Post> posts = connection.Read<Post>(whereClause: $"TITLE LIKE '%Test title%'");
            foreach (Post post in posts)
            {
                post.Comments = connection.Read<Comment>(whereClause: $"POST_ID = '{post.Id}'");
            }
            Assert.NotNull(posts);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    [Fact]
    public void Update_ById()
    {
        using var connection = new OracleConnection(ConnectionString);
        connection.Open();
        try
        {
            Post? post = connection.Read<Post>(1000001541).FirstOrDefault();
            if (post != null)
            {
                post.Title = "Updated Test Title";
                connection.Update(post); // Update By Id
            }
            Assert.NotNull(post);
            Assert.NotNull(post?.Comments);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}
