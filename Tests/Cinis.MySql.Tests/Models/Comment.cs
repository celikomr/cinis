using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinis.MySql.Tests.Models;

[Table("comment", Schema = "mydb")]
public class Comment
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("post_id")]
    public int PostId { get; set; }

    [Column("name")]
    public string? Name { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("body")]
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
