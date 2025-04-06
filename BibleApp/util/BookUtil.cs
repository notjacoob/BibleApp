using BibleApp.data;
using BibleApp.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.util;

/// <summary>
/// book business layer
/// </summary>
/// <param name="config">injected</param>
/// <param name="verseUtil">injected</param>
public class BookUtil(DbAccessor config, VerseUtil verseUtil)
{
    /// <summary>
    /// get the number of chapters within a book
    /// </summary>
    /// <param name="book">the book of which to count</param>
    /// <returns>the number of chapters in the book</returns>
    public async Task<int> ChapterCount(BookModel book)
    {
        await using var verseCtx = new AsvDataContext(config);
        var c = await verseCtx.Verses.Where(b=>b.VerseBookId==book.BookId).Select(e => e.VerseChapter).Distinct().CountAsync();
        return c;
    }
    /// <summary>
    /// take a list of books and divide them by their testaments, starting with the old testament
    /// </summary>
    /// <param name="all"></param>
    /// <returns>Ot: a list of books in the old testament. Nt: a list of books in the new testament</returns>
    public (List<BookModel> Ot, List<BookModel> Nt) DivideByTestaments(IEnumerable<BookModel> all)
    {
        List<BookModel> ot = [];
        List<BookModel> nt = [];
        foreach (var book in all)
        {
            if (book.BookTestament == "OT") ot.Add(book);
            else if (book.BookTestament == "NT") nt.Add(book);
        }

        return (ot, nt);
    }
    /// <summary>
    /// get every book
    /// </summary>
    /// <returns>a collection of all books</returns>
    public async Task<IEnumerable<BookModel>> GetAll()
    {
        await using var booksCtx = new BooksDataContext(config);
        return await booksCtx.Books.ToListAsync();
    }
    /// <summary>
    /// get a book by its ID
    /// </summary>
    /// <param name="id">the ID of the book</param>
    /// <returns>the found book or null</returns>
    public async Task<BookModel?> FromId(int id)
    {
        await using var booksCtx = new BooksDataContext(config);
        return await booksCtx.Books.FirstOrDefaultAsync(b => b.BookId == id);
    }
    /// <summary>
    /// get a list of verses in a chapter, as well as a queue of their respective comment counts
    /// </summary>
    /// <param name="b">book id</param>
    /// <param name="chapterId">chapter id</param>
    /// <returns>verses: collection of verse models within the chapter, commentsCounts: a queue of comment counts mapped to the verses. dequeue 1 of commentsCounts = element 0 of verses</returns>
    public async Task<(IEnumerable<VerseModel> verses, Queue<int> commentsCounts)> GetChapter(int b, int chapterId)
    {
        var q = new Queue<int>();
        await using var verseCtx = new AsvDataContext(config);
        var verses = await verseCtx.Verses.Where(v => v.VerseBookId == b && v.VerseChapter == chapterId).Include(v => v.Book)
            .OrderBy(v => v.VerseNumber).ToListAsync();
        foreach (var verseModel in verses)
        {
            q.Enqueue(await verseUtil.CommentCount(verseModel));
        }
        return (verses, q);
    }
    /// <summary>
    /// get the header for a chapter
    /// </summary>
    /// <param name="book">book id</param>
    /// <param name="chapter">chapter id</param>
    /// <returns>text: the verse passage. e.g.: Genesis 1:1-50, testament: The testament of the book</returns>
    public async Task<(string text, string testament)> GetHeader(int book, int chapter)
    {
        await using var booksCtx = new BooksDataContext(config);
        var bookModel = await booksCtx.Books.Include(b => b.Verses).FirstOrDefaultAsync(b => b.BookId == book);
        if (bookModel == null) return ("error", "error");
        var verseCount = bookModel.Verses.Count(v => v.VerseChapter == chapter);
        return ($"{bookModel.BookName} {chapter}:{1}-{verseCount}", VerseUtil.ToFormattedString(bookModel.BookTestament));
    }
}