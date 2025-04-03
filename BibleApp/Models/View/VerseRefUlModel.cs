namespace BibleApp.Models.View;

/// <summary>
/// model for the reference page verse list group
/// </summary>
public class VerseRefUlModel
{
    public required VerseHeaderModel VerseHeader { get; set; }
    public required IEnumerable<VerseViewModel> VerseRefs { get; set; }
}