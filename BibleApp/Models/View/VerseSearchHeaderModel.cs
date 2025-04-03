namespace BibleApp.Models.View;

/// <summary>
/// header model for searched verses
/// </summary>
public class VerseSearchHeaderModel
{
    public int ResultCount { get; set; }
    public string SearchedIn { get; set; } = "None";
}