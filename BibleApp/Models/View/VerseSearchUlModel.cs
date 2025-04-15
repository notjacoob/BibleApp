namespace BibleApp.Models.View;

/// <summary>
/// model for the search page verse list group
/// </summary>
public class VerseSearchUlModel
{
    public required VerseSearchHeaderModel VerseHeader { get; set; }
    public required IEnumerable<VerseSearchViewModel> VerseSearches { get; set; }
    public required int Page { get; set; } = 0;
}