using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BibleApp.Models;
using BibleApp.Models.DB;
using BibleApp.Models.View;
using BibleApp.util;

namespace BibleApp.Controllers;

public class HomeController(VerseUtil verseUtil, CommentUtil commentUtil, BookUtil bookUtil) : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    public IActionResult Search()
    {
        return View();
    }

    public async Task<IActionResult> Reference()
    {
        var books = await bookUtil.GetAll();
        return View("Reference", new ReferenceViewModel{Books = books});
    }

    [HttpPost]
    public async Task<IActionResult> PostSearch([FromBody] VerseSearchModel verseSearchModel)
    {
        List<VerseViewModel> searchResults = [];
        if (verseSearchModel.SearchInVerseName)
        {
            searchResults.AddRange(await verseUtil.SearchInTitle(verseSearchModel.SearchTerm, verseSearchModel.Testament, verseSearchModel.MatchBy));
        }

        var flipIndex = searchResults.Count - 1;
        if (verseSearchModel.SearchInVerseText)
        {
            searchResults.AddRange(await verseUtil.SearchInVerse(verseSearchModel.SearchTerm, verseSearchModel.Testament, verseSearchModel.MatchBy));
        }
        var toModel = verseUtil.CompressList(searchResults, flipIndex);
        var searchedIn = "";
        if (verseSearchModel.SearchInVerseText) searchedIn += "Text";
        if (verseSearchModel.SearchInVerseText && verseSearchModel.SearchInVerseName) searchedIn += ", ";
        if (verseSearchModel.SearchInVerseName) searchedIn += "Name";
        var verseHeader = new VerseSearchHeaderModel
            { ResultCount = toModel.Count, SearchedIn = searchedIn == "" ? "Nothing" : searchedIn };
        return PartialView("_VerseSearch", new VerseSearchUlModel{VerseHeader = verseHeader, VerseSearches = toModel});
    }

    [HttpGet]
    public async Task<IActionResult> GetComments([FromQuery] int verseId)
    {
        var vm = await verseUtil.FromId(verseId);
        if (vm == null) return NotFound();
        return PartialView("_VerseComments", vm);
    }

    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] AddCommentModel addCommentModel)
    {
        var c = await commentUtil.Create(addCommentModel);
        return PartialView("_CommentLi", c);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteComment([FromBody] DeleteCommentModel model)
    {
        await commentUtil.Delete(model.CommentId);
        return Ok();
    }

    [HttpGet]
    public async Task<IActionResult> LoadBook([FromQuery] int bookId)
    {
        var b = await bookUtil.FromId(bookId);
        if (b==null) return NotFound();
        return PartialView("_ChapterSelector", new ChapterSelectorViewModel{Book = b, ChapterCount = await bookUtil.ChapterCount(b)});
        
    }

    [HttpGet]
    public async Task<IActionResult> LoadChapter([FromQuery] int bookId, [FromQuery] int chapterId)
    {
        var header = await bookUtil.GetHeader(bookId, chapterId);
        var c = await bookUtil.GetChapter(bookId, chapterId);
        var cc = c.verses.Select(v =>
                new VerseViewModel
            {
                VerseId = v.Id, VerseTitle = verseUtil.FullVerseName(v), VerseText = v.VerseText,
                Testament = VerseUtil.ToFormattedString(v.Book.BookTestament),
                CommentCount = c.commentsCounts.Dequeue()
            }
        );
        var headerModel = new VerseHeaderModel { HeaderText = header.text, HeaderTestament = header.testament };
        return PartialView("_VerseRef", new VerseRefUlModel{VerseHeader = headerModel, VerseRefs = cc});
    }
}