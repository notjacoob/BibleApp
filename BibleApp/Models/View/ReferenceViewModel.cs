using BibleApp.Models.DB;

namespace BibleApp.Models.View;

/// <summary>
/// model for loading the book selector dropdown
/// </summary>
public class ReferenceViewModel
{
    public required IEnumerable<BookModel> Books { get; set; }
}