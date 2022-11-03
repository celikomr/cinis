# Cinis.Oracle Samples

Are you ready to take the Dapper implementation to the next level? :rocket:

Database operations are now very easy with Cinis.Oracle. You will get rid of the cost of writing sql.

Cinis.Oracle not only produces readable code blocks, but also speeds up your development.

## Entities

```cs
[Table("POST", Schema = "MYDB")]
public class Post
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("TITLE")]
    public string? Title { get; set; }

    [Column("BODY")]
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
[Table("\"COMMENT\"", Schema = "MYDB")]
public class Comment
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("POST_ID")]
    public int PostId { get; set; }

    [Column("NAME")]
    public string? Name { get; set; }

    [Column("EMAIL")]
    public string? Email { get; set; }

    [Column("BODY")]
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
string connectionString = "Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = <host>)(PORT = 1521)) +
                (CONNECT_DATA =(SERVICE_NAME = <service_name>) (SERVER = DEDICATED) ) ); User ID = <username>; Password = <password>;";

using var connection = new OracleConnection(connectionString);
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
string connectionString = "Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = <host>)(PORT = 1521)) +
                (CONNECT_DATA =(SERVICE_NAME = <service_name>) (SERVER = DEDICATED) ) ); User ID = <username>; Password = <password>;";

using var connection = new OracleConnection(connectionString);
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
string connectionString = "Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = <host>)(PORT = 1521)) +
                (CONNECT_DATA =(SERVICE_NAME = <service_name>) (SERVER = DEDICATED) ) ); User ID = <username>; Password = <password>;";

using var connection = new OracleConnection(connectionString);
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
string connectionString = "Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = <host>)(PORT = 1521)) +
                (CONNECT_DATA =(SERVICE_NAME = <service_name>) (SERVER = DEDICATED) ) ); User ID = <username>; Password = <password>;";

using var connection = new OracleConnection(connectionString);
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
