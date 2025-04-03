using BibleApp.data;
using BibleApp.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.util;

public class BookUtil(IConfiguration config, VerseUtil verseUtil)
{

    public async Task<int> ChapterCount(BookModel book)
    {
        await using var verseCtx = new AsvDataContext(config);
        var c = await verseCtx.Verses.Where(b=>b.VerseBookId==book.BookId).Select(e => e.VerseChapter).Distinct().CountAsync();
        return c;
    }

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

    public async Task<IEnumerable<BookModel>> GetAll()
    {
        await using var booksCtx = new BooksDataContext(config);
        return await booksCtx.Books.ToListAsync();
    }

    public async Task<BookModel?> FromId(int id)
    {
        await using var booksCtx = new BooksDataContext(config);
        return await booksCtx.Books.FirstOrDefaultAsync(b => b.BookId == id);
    }

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

    public async Task<(string text, string testament)> GetHeader(int book, int chapter)
    {
        await using var booksCtx = new BooksDataContext(config);
        var bookModel = await booksCtx.Books.Include(b => b.Verses).FirstOrDefaultAsync(b => b.BookId == book);
        if (bookModel == null) return ("error", "error");
        var verseCount = bookModel.Verses.Count(v => v.VerseChapter == chapter);
        return ($"{bookModel.BookName} {chapter}:{1}-{verseCount}", VerseUtil.ToFormattedString(bookModel.BookTestament));
    }
}