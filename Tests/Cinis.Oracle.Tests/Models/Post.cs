using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinis.Oracle.Tests.Models;

[Table("post", Schema = "oclk")]
public class Post
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("title")]
    public string? Title { get; set; }

    [Column("body")]
    public string? Body { get; set; }

    public List<Comment>? Comments { get; set; }
}
