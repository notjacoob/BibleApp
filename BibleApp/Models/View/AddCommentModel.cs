namespace BibleApp.Models.View;

/// <summary>
/// model for adding a comment to a verse
/// </summary>
public class AddCommentModel
{
    public required string Content { get; set; }
    public required int VerseId { get; set; }
}