using System.ComponentModel.DataAnnotations;

namespace BibleApp.Models.View;

/// <summary>
/// model for submitting a verse search
/// </summary>
public class VerseSearchModel
{
    [Required]
    public SearchInTestament Testament { get; set; }
    [Required]
    public required string MatchBy { get; set; }
    [Required]
    [Length(1, 100)]
    public required string SearchTerm { get; set; }
    [Required]
    [Range(1, int.MaxValue)]
    public required int PageNum { get; set; }
}