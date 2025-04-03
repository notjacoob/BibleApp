namespace BibleApp.Models.View;

public class VerseSearchUlModel
{
    public required VerseSearchHeaderModel VerseHeader { get; set; }
    public required IEnumerable<VerseSearchViewModel> VerseSearches { get; set; }
}