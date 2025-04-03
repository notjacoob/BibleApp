using System.ComponentModel.DataAnnotations;

namespace BibleApp.Models.View;

public class VerseSearchModel
{
    [Required]
    public bool SearchInVerseName { get; set; }
    [Required]
    public bool SearchInVerseText { get; set; }
    [Required]
    public SearchInTestament Testament { get; set; }
    [Required]
    public required string MatchBy { get; set; }
    [Required]
    [Length(1, 100)]
    public required string SearchTerm { get; set; }
}