# Cinis Samples

## Installation

```
NuGet\Install-Package Cinis.MsSql -Version 1.0.0-beta.1
```

```
NuGet\Install-Package Cinis.MySql -Version 1.0.0-beta.1
```

```
NuGet\Install-Package Cinis.Oracle -Version 1.0.0-beta.2
```

```
NuGet\Install-Package Cinis.PostgreSql -Version 1.0.0-beta.1
```

## Usage

The basic and advanced examples for all modules of the Cinis are listed in order.

### Entities

Use DataAnnotations for every entity

```cs
[Table("POST", Schema = "OCLK")]
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
[Table("\"COMMENT\"", Schema = "oclk")]
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

### Basic Connection Strings

```cs
string connectionString = "Server=<host>;Database=<database>;User Id=<username>;Password=<password>"; // For MsSql
string connectionString = "Server=<host>;User ID=<username>;Password=<password>;Database=<database>"; // For MySql
string connectionString = "Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = <host>)(PORT = 1521)) (CONNECT_DATA =(SERVICE_NAME = <service_name>) (SERVER = DEDICATED) ) ); User ID = <username>; Password = <password>;"; // For Oracle
string connectionString = "Host=<host>; Database=<database>; Username=<username>; Password=<password>"; // For PostgreSql
```

### Basic Feture(s) of Cinis