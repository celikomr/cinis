using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinis.Oracle.Tests.Models;

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
