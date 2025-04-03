namespace BibleApp.Models.View;

public class VerseRefUlModel
{
    public required VerseHeaderModel VerseHeader { get; set; }
    public required IEnumerable<VerseViewModel> VerseRefs { get; set; }
}