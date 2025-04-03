namespace BibleApp.Models.View;

/// <summary>
/// model for displaying a verse
/// </summary>
public class VerseViewModel
{
    public string VerseTitle { get; set; }
    public string VerseText { get; set; }
    public string Testament { get; set; }
    public int VerseId { get; set; }
    public int CommentCount { get; set; }

}