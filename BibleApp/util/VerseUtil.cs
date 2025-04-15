using System.Diagnostics;
using BibleApp.data;
using BibleApp.Models;
using BibleApp.Models.DB;
using BibleApp.Models.View;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.util;
/// <summary>
/// Verse business layer
/// </summary>
/// <param name="config">injected</param>
public class VerseUtil(DbAccessor config)
{
    /// <summary>
    /// Get a verse by its ID
    /// </summary>
    /// <param name="id">The ID of the verse</param>
    /// <returns>The found verse or null</returns>
    public async Task<VerseModel?> FromId(int id)
    {
        await using var dc = new AsvDataContext(config);
        var verse = await dc.Verses.Where(v => v.Id == id).Include(v => v.Comments).Include(v => v.Book).FirstOrDefaultAsync();
        return verse;
    }
    /// <summary>
    /// match a searchTerm to a string
    /// </summary>
    /// <param name="searchTerm">the term to match</param>
    /// <param name="matchTo">the string to match against</param>
    /// <param name="matchBy">the method of matching</param>
    /// <returns>true if match, false if not</returns>
    private static bool Match(string searchTerm, string matchTo, string matchBy) => matchBy switch
    {
        "any" => MatchByAnyText(searchTerm, matchTo),
        "word" => MatchByWord(searchTerm, matchTo),
        "phrase" => MatchByPhrase(searchTerm, matchTo),
        _ => false
    };
    /// <summary>
    /// match a full phrase
    /// </summary>
    /// <param name="searchTerm">the phrase to match</param>
    /// <param name="matchTo">the string to match against</param>
    /// <returns>true if matched, false if not</returns>
    private static bool MatchByPhrase(string searchTerm, string matchTo)
    {
        return matchTo.ToLower().Contains(searchTerm.ToLower());
    }
    /// <summary>
    /// match all full words
    /// </summary>
    /// <param name="searchTerm">the word to match</param>
    /// <param name="matchTo">the string to match against</param>
    /// <returns>true if matched, false if not</returns>
    private static bool MatchByWord(string searchTerm, string matchTo)
    {
        var split = searchTerm.ToLower().Split(" ");
        var matchToLower = matchTo.ToLower().Split(" ");
        var i = 0;
        foreach (var s in split)
        {
            if (matchToLower.Contains(s)) i++;
        }

        return i == split.Length;
    }
    /// <summary>
    /// match any text
    /// </summary>
    /// <param name="searchTerm">the text to match</param>
    /// <param name="matchTo">the string to match against</param>
    /// <returns>true if matched, false if not</returns>
    private static bool MatchByAnyText(string searchTerm, string matchTo)
    {
        var split = searchTerm.ToLower().Split(" ");
        var matchToLower = matchTo.ToLower();
        var i = 0;
        foreach (var s in split)
        {
            if (matchToLower.Contains(s))
            {
                i++;
            }
        }

        return i == split.Length;
    }
    /// <summary>
    /// Get the full verse name including book
    /// </summary>
    /// <param name="verse">the verse to name</param>
    /// <returns>the name of the verse</returns>
     public string FullVerseName(VerseModel verse)
     { 
        var book = verse.Book;
        return $"{book.BookName} {verse.VerseChapter}:{verse.VerseNumber}";
    }
    /// <summary>
    /// search a verse by its title
    /// </summary>
    /// <param name="search">the search term</param>
    /// <param name="testament">the testaments to search in</param>
    /// <param name="matchBy">the method of matching</param>
    /// <returns>a view model of all the matched verses</returns>
    public async Task<IEnumerable<VerseViewModel>> SearchInTitle(string search, SearchInTestament testament, string matchBy, int pageNum)
    {
        await using var verseData = new AsvDataContext(config);
        List<VerseViewModel> verses = [];
        var versesDb = await verseData.Verses.Include(e => e.Book).Include(e => e.Comments).ToListAsync();
        foreach (var verseDataVerse in versesDb)
        {
            if (testament == SearchInTestament.Both || verseDataVerse.Book.BookTestament == testament.ToString())
            {
                var verseTitle = FullVerseName(verseDataVerse);
                if (Match(search, verseTitle, matchBy))
                {
                    var cc = verseDataVerse.Comments.Count;
                    var viewModel = new VerseViewModel{VerseTitle = verseTitle, VerseText = verseDataVerse.VerseText, Testament = ToFormattedString(verseDataVerse.Book.BookTestament), VerseId = verseDataVerse.Id, CommentCount = cc};
                    verses.Add(viewModel);
                }
            }
        }

        return verses;
    }
    /// <summary>
    /// search a verse by its text
    /// </summary>
    /// <param name="search">the search term</param>
    /// <param name="testament">the testaments to search in</param>
    /// <param name="matchBy">the method of matching</param>
    /// <returns>a view model of all the matched verses</returns>
    public async Task<(List<VerseViewModel>, int)> SearchInVerse(string search, SearchInTestament testament, string matchBy, int pageNum)
    {
        await using var verseData = new AsvDataContext(config);
        var versesDb = verseData.Verses;
        IQueryable<VerseModel> versesWithSearch;
        switch (matchBy)
        {
            case "any":
                versesWithSearch = versesDb.Where(v => EF.Functions.Like(v.VerseText, $"%{search}%"));
                break;
            case "word":
                versesWithSearch = versesDb.Where(v => 
                    EF.Functions.Like(v.VerseText, $"% {search} %")
                    || EF.Functions.Like(v.VerseText, $"% {search}")
                    || EF.Functions.Like(v.VerseText, $"{search} %")
                    || EF.Functions.Like(v.VerseText, $"{search}"));
                break;
            default:
                versesWithSearch = versesDb.Where(v => v.VerseText == search);
                break;
        }
        
        var vc = versesWithSearch.Count();
        
        var versesFromDb = await versesWithSearch.Skip(pageNum*25).Take(25).Include(v => v.Comments).Include(v => v.Book).ToListAsync();

        var filteredVerses = versesFromDb.Select(v =>
        {
            var verseTitle = FullVerseName(v);
            var cc = v.Comments.Count;
            return new VerseViewModel
            {
                VerseTitle = verseTitle, VerseText = v.VerseText, Testament = ToFormattedString(v.Book.BookTestament),
                VerseId = v.Id, CommentCount = cc
            };
        }).ToList();
        return (filteredVerses, vc);
    }
    /// <summary>
    /// map a list of verse models to searchable models using a radix
    /// </summary>
    /// <param name="verses">the verses to map</param>
    /// <param name="flipIndex">the radix at which old testament switches to new testament</param>
    /// <returns>a list of mapped verses</returns>
    public List<VerseSearchViewModel> MapToSearchModel(List<VerseViewModel> verses)
    {
        List<string> existingNames = [];
        List<VerseSearchViewModel> verseSearchResults = [];
        foreach (var v in verses)
        {
            if (!existingNames.Contains(v.VerseTitle))
            {
                existingNames.Add(v.VerseTitle);
                verseSearchResults.Add(new VerseSearchViewModel{VerseVm = v});
            }
        }
        return verseSearchResults;
    }
    /// <summary>
    /// convert a DB testament string to a readable string
    /// </summary>
    /// <param name="testament">the db string</param>
    /// <returns>the readable string</returns>
    public static string ToFormattedString(string testament)
    {
        if (testament == "NT") return "New Testament";
        return "Old Testament";
    }
    /// <summary>
    /// get the number of comments on a verse
    /// </summary>
    /// <param name="verse">the verse to count</param>
    /// <returns>the number of comments on the verse</returns>
    public async Task<int> CommentCount(VerseModel verse)
    {
        await using var ctx = new CommentsDataContext(config);
        var c = await ctx.Comments.Where(c => c.VerseId == verse.Id).CountAsync();
        return c;
    }
}