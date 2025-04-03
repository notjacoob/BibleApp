using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BibleApp.Models;
using BibleApp.Models.DB;
using BibleApp.Models.View;
using BibleApp.util;

namespace BibleApp.Controllers;
/// <summary>
/// the primary controller for the app
/// </summary>
/// <param name="verseUtil">injected</param>
/// <param name="commentUtil">injected</param>
/// <param name="bookUtil">injected</param>
public class HomeController(VerseUtil verseUtil, CommentUtil commentUtil, BookUtil bookUtil) : Controller
{   
    /// <summary>
    /// the landing page of the app
    /// </summary>
    /// <returns>Index.cshtml</returns>
    public IActionResult Index()
    {
        return View();
    }
    /// <summary>
    /// The pages privacy policy (current unused)
    /// </summary>
    /// <returns>Index.cshtml</returns>
    public IActionResult Privacy()
    {
        return RedirectToAction("Index", "Home");
    }
    /// <summary>
    /// reroute error requests
    /// </summary>
    /// <returns>Error.cshtml</returns>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    /// <summary>
    /// View empty search page
    /// URL loading is done clientside
    /// </summary>
    /// <returns>Search.cshtml</returns>
    public IActionResult Search()
    {
        return View();
    }
    /// <summary>
    /// View the reference page
    /// URL loading is done clientside
    /// </summary>
    /// <returns>Reference.cshtml</returns>
    public async Task<IActionResult> Reference()
    {
        // a list of current books is required
        var books = await bookUtil.GetAll();
        return View("Reference", new ReferenceViewModel{Books = books});
    }
    /// <summary>
    /// Search verses and return a result
    /// </summary>
    /// <param name="verseSearchModel">The search options viewmodel from Search.cshtml</param>
    /// <returns>_VerseSearch.cshtml</returns>
    [HttpPost]
    public async Task<IActionResult> PostSearch([FromBody] VerseSearchModel verseSearchModel)
    {
        List<VerseViewModel> searchResults = [];
        // add verse name results to general search results
        if (verseSearchModel.SearchInVerseName)
        {
            searchResults.AddRange(await verseUtil.SearchInTitle(verseSearchModel.SearchTerm, verseSearchModel.Testament, verseSearchModel.MatchBy));
        }
        // mark the point where name results end and text results start
        var flipIndex = searchResults.Count - 1;
        // add verse text results to general search results
        if (verseSearchModel.SearchInVerseText)
        {
            searchResults.AddRange(await verseUtil.SearchInVerse(verseSearchModel.SearchTerm, verseSearchModel.Testament, verseSearchModel.MatchBy));
        }
        // map VerseViewModel to VerseSearchViewModel
        var toModel = verseUtil.MapToSearchModel(searchResults, flipIndex);
        // formatted UI string for the name & text searched in params
        var searchedIn = "";
        if (verseSearchModel.SearchInVerseText) searchedIn += "Text";
        if (verseSearchModel.SearchInVerseText && verseSearchModel.SearchInVerseName) searchedIn += ", ";
        if (verseSearchModel.SearchInVerseName) searchedIn += "Name";
        // header view model
        var verseHeader = new VerseSearchHeaderModel
            { ResultCount = toModel.Count, SearchedIn = searchedIn == "" ? "Nothing" : searchedIn };
        return PartialView("_VerseSearch", new VerseSearchUlModel{VerseHeader = verseHeader, VerseSearches = toModel});
    }
    /// <summary>
    /// get the comments modal for a verse
    /// </summary>
    /// <param name="verseId">the Id of the verse with comments</param>
    /// <returns>_VerseComments.cshtml</returns>
    [HttpGet]
    public async Task<IActionResult> GetComments([FromQuery] int verseId)
    {
        var vm = await verseUtil.FromId(verseId);
        // if the verse doesn't exist, 404
        if (vm == null) return NotFound();
        return PartialView("_VerseComments", vm);
    }
    /// <summary>
    /// add a comment to a verse
    /// </summary>
    /// <param name="addCommentModel">view model</param>
    /// <returns>_CommentLi.cshtml</returns>
    [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] AddCommentModel addCommentModel)
    {
        // add the comment to the business layer
        var c = await commentUtil.Create(addCommentModel);
        return PartialView("_CommentLi", c);
    }
    /// <summary>
    /// delete a comment from a verse
    /// </summary>
    /// <param name="model">view model</param>
    /// <returns>200 if successful, 500 otherwise</returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteComment([FromBody] DeleteCommentModel model)
    {
        // delete comment from business layer
        await commentUtil.Delete(model.CommentId);
        return Ok();
    }
    /// <summary>
    /// load the chapter selector dropdown
    /// </summary>
    /// <param name="bookId">the book from which to select chapters</param>
    /// <returns>_ChapterSelector.cshtml</returns>
    [HttpGet]
    public async Task<IActionResult> LoadBook([FromQuery] int bookId)
    {
        var b = await bookUtil.FromId(bookId);
        // if the book is null, 404
        if (b==null) return NotFound();
        return PartialView("_ChapterSelector", new ChapterSelectorViewModel{Book = b, ChapterCount = await bookUtil.ChapterCount(b)});
        
    }
    /// <summary>
    /// load the verses from a chapter and book
    /// </summary>
    /// <param name="bookId">the id of the book</param>
    /// <param name="chapterId">the id of the chapter</param>
    /// <returns>_VerseRef.cshtml</returns>
    [HttpGet]
    public async Task<IActionResult> LoadChapter([FromQuery] int bookId, [FromQuery] int chapterId)
    {
        // generate a header for the chapter
        var header = await bookUtil.GetHeader(bookId, chapterId);
        // get the chapter from the book
        var c = await bookUtil.GetChapter(bookId, chapterId);
        // map verses in chapter to view model
        var cc = c.verses.Select(v =>
                new VerseViewModel
            {
                VerseId = v.Id, VerseTitle = verseUtil.FullVerseName(v), VerseText = v.VerseText,
                Testament = VerseUtil.ToFormattedString(v.Book.BookTestament),
                CommentCount = c.commentsCounts.Dequeue()
            }
        );
        // map header to view model
        var headerModel = new VerseHeaderModel { HeaderText = header.text, HeaderTestament = header.testament };
        return PartialView("_VerseRef", new VerseRefUlModel{VerseHeader = headerModel, VerseRefs = cc});
    }
}