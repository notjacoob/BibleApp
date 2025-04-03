using BibleApp.Models.DB;

namespace BibleApp.Models.View;

public class ReferenceViewModel
{
    public required IEnumerable<BookModel> Books { get; set; }
}