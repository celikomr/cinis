using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinis.Oracle.Samples.Models;

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
