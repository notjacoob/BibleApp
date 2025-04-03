using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.Models.DB;

/// <summary>
/// database model for comments
/// </summary>
[Table("v_comments")]
[PrimaryKey("Id")]
public class CommentModel
{
    [Column("id")]
    [Key]
    public int Id { get; set; }
    [Column("t")]
    public required string Content { get; set; }
    [Column("v")]
    public required int VerseId { get; set; }
    [Column("c")]
    public required DateTime CreatedAt { get; set; }
    [ForeignKey("VerseId")]
    public VerseModel? Verse { get; set; }
}