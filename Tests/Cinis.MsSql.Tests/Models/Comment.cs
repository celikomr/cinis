using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinis.MsSql.Tests.Models;

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
