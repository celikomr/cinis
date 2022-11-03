# Cinis.MsSql Samples

Many examples for Dapper MsSql database operations are discussed below.

You can send your questions and suggestions by opening an issue or by writing from the Discussions tab.

## Entities

```cs
[Table("Post")]
public class Post
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("Title")]
    public string? Title { get; set; }

    [Column("Body")]
    public string? Body { get; set; }

    public List<Comment>? Comments { get; set; }

    public Post() { }

    public Post(string? title, string? body)
    {
        Title = title;
        Body = body;
    }

    public Post(int id, string? title, string? body)
    {
        Id = id;
        Title = title;
        Body = body;
    }
}
```

```cs
[Table("Comment")]
public class Comment
{
    [Key]
    [Column("Id")]
    public int Id { get; set; }

    [Column("PostId")]
    public int PostId { get; set; }

    [Column("Name")]
    public string? Name { get; set; }

    [Column("Email")]
    public string? Email { get; set; }

    [Column("Body")]
    public string? Body { get; set; }

    public Comment() { }

    public Comment(int postId, string? name, string? email, string? body)
    {
        PostId = postId;
        Name = name;
        Email = email;
        Body = body;
    }

    public Comment(int id, int postId, string? name, string? email, string? body)
    {
        Id = id;
        PostId = postId;
        Name = name;
        Email = email;
        Body = body;
    }
}
```

## Basics

### Create & CreateAsync

```cs
string connectionString = "Server=<host>;Database=<database>;User Id=<username>;Password=<password>";

using var connection = new SqlConnection(connectionString);
connection.Open();
// connection.OpenAsync(); // For Async Usage
try
{
    int postId = connection.Create(new Post("Test title", "Test body"));
    // int postId = await connection.CreateAsync(new Post("Test title", "Test body")); // For Async Usage
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    throw;
}
```

### Read & ReadAsync (By Id & By WhereClause)

```cs
string connectionString = "Server=<host>;Database=<database>;User Id=<username>;Password=<password>";

using var connection = new SqlConnection(connectionString);
connection.Open();
// connection.OpenAsync(); // For Async Usage
try
{
    Post? post = connection.Read<Post>(<key>).FirstOrDefault(); // Read By Id
    // var posts = await connection.ReadAsync<Post>(<key>); // For Async Usage
    // Post? post = posts.FirstOrDefault(); // For Async Usage
    if (post != null)
    {
        post.Comments = connection.Read<Comment>(whereClause: $"POST_ID = '{post.Id}'"); // By WhereClause
        // post.Comments = await connection.ReadAsync<Comment>(whereClause: $"POST_ID = '{post.Id}'"); // For Async Usage
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    throw;
}
```

### Update & UpdateAsync (By Id & By WhereClause)

```cs
string connectionString = "Server=<host>;Database=<database>;User Id=<username>;Password=<password>";

using var connection = new SqlConnection(connectionString);
connection.Open();
// connection.OpenAsync(); // For Async Usage
try
{
    Post? post = connection.Read<Post>(<key>).FirstOrDefault();
    // var posts = await connection.ReadAsync<Post>(<key>); // For Async Usage
    // Post? post = posts.FirstOrDefault(); // For Async Usage
    if (post != null)
    {
        post.Title = "Updated Test Title";
        connection.Update(post); // Update By Id (nullable = false)
        // connection.Update(post, true); // Update By Id (nullable = true)
        // connection.Update(post, whereClause: $"ID = '{post.Id}'"); // Update By WhereClause (nullable = false)
        // connection.Update(post, true, $"ID = '{post.Id}'"); // Update By WhereClause (nullable = true)

        // await connection.UpdateAsync(post, true); // For Async Usage
        // await connection.UpdateAsync(post, whereClause: $"ID = '{post.Id}'"); // For Async Usage
        // await connection.UpdateAsync(post, true, $"ID = '{post.Id}'"); // For Async Usage
    }
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    throw;
}
```

### Delete & DeleteAsync (By Id & By WhereClause)

```cs
string connectionString = "Server=<host>;Database=<database>;User Id=<username>;Password=<password>";

using var connection = new SqlConnection(connectionString);
connection.Open();
// connection.OpenAsync(); // For Async Usage
try
{
    connection.Delete<Post>(<key>); // Delete By Id - <key> is dynamic
    // connection.Delete<Post>(whereClause: $"ID = '{<key>}'"); // Delete By WhereClause

    // await connection.DeleteAsync<Post>(<key>); // For Async Usage
    // await connection.DeleteAsync<Post>(whereClause: $"ID = '{<key>}'"); // For Async Usage
}
catch (Exception ex)
{
    Console.WriteLine(ex.ToString());
    throw;
}
```

## Advanced Usages

### CRUD Sample With Transaction

```cs
string connectionString = "Server=<host>;Database=<database>;User Id=<username>;Password=<password>";

using var connection = new SqlConnection(connectionString);
connection.Open();
// connection.OpenAsync(); // For Async Usage
using SqlTransaction transaction = connection.BeginTransaction();
try
{
    // Read Operation(s)
    Post? post = connection.Read<Post>(<key>, transaction: transaction).FirstOrDefault(); 
    // var posts = await connection.ReadAsync<Post>(<key>, transaction: transaction); // For Async Usage
    // Post? post = posts.FirstOrDefault(); // For Async Usage

    // Update Operation(s)
    post.Title = "Updated Test Title";
    connection.Update(post);
    // await connection.UpdateAsync(post, transaction: transaction); // For Async Usage

    // Delete Operation(s)
    foreach (Comment comment in post.Comments)
    {
        connection.Delete<Comment>(comment.Id, transaction: transaction);
        // await connection.DeleteAsync<Comment>(comment.Id, transaction: transaction); // For Async Usage
    }

    // Create Operation(s)
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
```
