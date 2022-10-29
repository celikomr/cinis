using Cinis.MsSql.Tests.Models;
using System.Data.SqlClient;

namespace Cinis.MsSql.Tests;

public partial class DapperExtensions
{
    [Fact]
    public async Task CreateAsync_WithoutTransaction()
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
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
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        using SqlTransaction transaction = connection.BeginTransaction();
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

    [Fact]
    public async Task ReadAsync_ById()
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        try
        {
            var posts = await connection.ReadAsync<Post>(14);
            Post? post = posts.FirstOrDefault();
            if (post != null)
            {
                post.Comments = await connection.ReadAsync<Comment>(whereClause: $"post_id = '{post.Id}'");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    [Fact]
    public async Task ReadAsync_ByWhereClause()
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        try
        {
            List<Post> posts = await connection.ReadAsync<Post>(whereClause: $"title like '%Test title%'");
            foreach (Post post in posts)
            {
                post.Comments = await connection.ReadAsync<Comment>(whereClause: $"post_id = '{post.Id}'");
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
    public async Task UpdateAsync_ById()
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        try
        {
            var posts = await connection.ReadAsync<Post>(1000001591);
            Post? post = posts.FirstOrDefault();
            if (post != null)
            {
                post.Title = "Updated Test Title";
                await connection.UpdateAsync(post); // Update By Id
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    [Fact]
    public async Task UpdateAsync_ByWhereClause()
    {
        using var connection = new SqlConnection(ConnectionString);
        connection.Open();
        try
        {
            var posts = await connection.ReadAsync<Post>(22);
            Post? post = posts.FirstOrDefault();
            if (post != null)
            {
                post.Title = "Updated Test Title";
                post.Body = null;
                await connection.UpdateAsync(post, true, $"id = '{post.Id}'"); // Update By WhereClause
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    [Fact]
    public async void DeleteAsync_ById()
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        try
        {
            await connection.DeleteAsync<Post>(6); // dynamic id = '1000001633'
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }

    [Fact]
    public async Task DeleteAsync_ByWhereClause()
    {
        using var connection = new SqlConnection(ConnectionString);
        await connection.OpenAsync();
        try
        {
            await connection.DeleteAsync<Post>(whereClause: "id = '24'"); // Delete By WhereClause
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
            throw;
        }
    }
}
