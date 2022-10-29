using Cinis.MsSql.Tests.Models;
using System.Data.SqlClient;

namespace Cinis.MsSql.Tests;

public partial class DapperExtensions
{
    public string ConnectionString { get; set; }

    public DapperExtensions()
    {
        ConnectionString = "Host=localhost; Database=mydb; Username=postgres; Password=postgres";
    }

    [Fact]
    public void Create_WithoutTransaction()
    {
        using var connection = new SqlConnection(ConnectionString);
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
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        using SqlTransaction transaction = connection.BeginTransaction();
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
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        try
        {
            Post? post = connection.Read<Post>(20).FirstOrDefault();
            if (post != null)
            {
                post.Comments = connection.Read<Comment>(whereClause: $"post_id = '{post.Id}'");
            }
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
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        try
        {
            List<Post> posts = connection.Read<Post>(whereClause: $"title like '%Test title%'");
            foreach (Post post in posts)
            {
                post.Comments = connection.Read<Comment>(whereClause: $"post_id = '{post.Id}'");
            }
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
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        try
        {
            Post? post = connection.Read<Post>(21).FirstOrDefault();
            if (post != null)
            {
                post.Title = "Updated Test Title";
                connection.Update(post); // Update By Id
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    [Fact]
    public void Update_ByWhereClause()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        try
        {
            Post? post = connection.Read<Post>(3).FirstOrDefault();
            if (post != null)
            {
                post.Title = "Updated Test Title";
                post.Body = null;
                connection.Update(post, true, $"id = '{post.Id}'"); // Update By WhereClause
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    [Fact]
    public void Delete_ById()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        try
        {
            connection.Delete<Post>(23); // dynamic id = '1000001633'
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    [Fact]
    public void Delete_ByWhereClause()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        try
        {
            connection.Delete<Post>(whereClause: "id = '3'"); // Delete By WhereClause
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}
