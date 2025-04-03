using System.Diagnostics;
using BibleApp.data;
using BibleApp.Models;
using BibleApp.Models.DB;
using BibleApp.Models.View;
using Microsoft.EntityFrameworkCore;

namespace BibleApp.util;

public class VerseUtil(IConfiguration config)
{
    public async Task<VerseModel?> FromId(int id)
    {
        await using var dc = new AsvDataContext(config);
        var verse = await dc.Verses.Where(v => v.Id == id).Include(v => v.Comments).Include(v => v.Book).FirstOrDefaultAsync();
        return verse;
    }

    private static bool Match(string searchTerm, string matchTo, string matchBy) => matchBy switch
    {
        "any" => MatchByAnyText(searchTerm, matchTo),
        "word" => MatchByWord(searchTerm, matchTo),
        "phrase" => MatchByPhrase(searchTerm, matchTo),
        _ => false
    };

    private static bool MatchByPhrase(string searchTerm, string matchTo)
    {
        return matchTo.ToLower().Contains(searchTerm.ToLower());
    }
    
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
    
     public string FullVerseName(VerseModel verse)
     { 
        var book = verse.Book;
        return $"{book.BookName} {verse.VerseChapter}:{verse.VerseNumber}";
    }

    public async Task<IEnumerable<VerseViewModel>> SearchInTitle(string search, SearchInTestament testament, string matchBy)
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

    public async Task<IEnumerable<VerseViewModel>> SearchInVerse(string search, SearchInTestament testament, string matchBy)
    {
        await using var verseData = new AsvDataContext(config);
        List<VerseViewModel> verses = [];
        var versesDb = await verseData.Verses.Include(e => e.Book).Include(e => e.Comments).ToListAsync();
        foreach (var verseDataVerse in versesDb)
        {
            if (testament == SearchInTestament.Both || verseDataVerse.Book.BookTestament == testament.ToString())
            {
                
                if (Match(search, verseDataVerse.VerseText, matchBy))
                {
                    var verseTitle = FullVerseName(verseDataVerse);
                    var cc = verseDataVerse.Comments.Count;
                    var viewModel = new VerseViewModel{VerseTitle = verseTitle, VerseText = verseDataVerse.VerseText, Testament = ToFormattedString(verseDataVerse.Book.BookTestament), VerseId = verseDataVerse.Id, CommentCount = cc};
                    verses.Add(viewModel);
                }
            }
        }
        return verses;
    }

    public VerseViewModel FromSearchModel(VerseSearchViewModel search)
    {
        return search.VerseVm;
    }

    public List<VerseSearchViewModel> CompressList(List<VerseViewModel> verses, int flipIndex)
    {
        List<string> existingNames = [];
        List<VerseSearchViewModel> verseSearchResults = [];
        var i = 0;
        foreach (var v in verses)
        {
            if (!existingNames.Contains(v.VerseTitle))
            {
                string foundIn;
                existingNames.Add(v.VerseTitle);
                if (i <= flipIndex)
                {
                    foundIn = "Verse Name";
                }
                else
                {
                    foundIn = "Verse Text";
                }
                verseSearchResults.Add(new VerseSearchViewModel{FoundIn = foundIn, VerseVm = v});
            }
            i++;
        }
        return verseSearchResults;
    }

    public static string ToFormattedString(string testament)
    {
        if (testament == "NT") return "New Testament";
        return "Old Testament";
    }

    public async Task<int> CommentCount(VerseModel verse)
    {
        await using var ctx = new CommentsDataContext(config);
        var c = await ctx.Comments.Where(c => c.VerseId == verse.Id).CountAsync();
        return c;
    }
}