using BibleApp.Models.DB;

namespace BibleApp.Models.View;

/// <summary>
/// model for selecting a chapter from a book
/// </summary>
public class ChapterSelectorViewModel
{
    public required BookModel Book { get; set; }
    public int ChapterCount { get; set; }
}