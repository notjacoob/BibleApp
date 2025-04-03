using BibleApp.Models.DB;

namespace BibleApp.Models.View;

public class ChapterSelectorViewModel
{
    public required BookModel Book { get; set; }
    public int ChapterCount { get; set; }
}