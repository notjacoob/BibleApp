using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.Models.DB;

[Table("t_asv")]
[PrimaryKey("Id")]
public class VerseModel
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [ScaffoldColumn(false)]
    [Key]
    [Column("id")]
    public int Id { get; init; }
    [Column("b")]
    [Required]
    public required int VerseBookId { get; init; }    
    [Column("c")]
    [Required]
    public required int VerseChapter { get; init; }
    [Column("v")]
    [Required]
    public required int VerseNumber { get; init; }
    [Column("t")]
    [Required]
    public required string VerseText { get; init; }
    [ForeignKey("VerseBookId")]
    public required BookModel Book { get; init; }
    // fk CommentModel.VerseId
    public required ICollection<CommentModel> Comments { get; init; }
}