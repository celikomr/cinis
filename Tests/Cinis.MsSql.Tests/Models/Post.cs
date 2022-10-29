using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cinis.MsSql.Tests.Models;

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
