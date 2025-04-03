namespace BibleApp.Models.View;

public class AddCommentModel
{
    public required string Content { get; set; }
    public required int VerseId { get; set; }
}