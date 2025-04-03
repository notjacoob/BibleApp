using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.Models.DB;

[Table("key_english")]
[PrimaryKey("BookId")]
public class BookModel
{
    [Column("b")]
    [Key]
    required 
    public int BookId { get; init; }
    [Column("n")]
    required 
    public string BookName { get; init; }
    [Column("t")]
    required 
    public string BookTestament { get; init; }
    [Column("g")]
    public int GenreId { get; init; }
    
    public required ICollection<VerseModel> Verses { get; init; }
    
}